// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeReviewer.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NodeReviewer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeAnalysis
{
    public class NodeReviewer : INodeInspector
    {
        private readonly Dictionary<SyntaxKind, ITriviaEvaluation[]> _triviaEvaluations;
        private readonly Dictionary<SyntaxKind, ICodeEvaluation[]> _codeEvaluations;
        private readonly Dictionary<SyntaxKind, ISemanticEvaluation[]> _semanticEvaluations;
        private readonly Dictionary<SymbolKind, ISymbolEvaluation[]> _symbolEvaluations;

        public NodeReviewer(IEnumerable<IEvaluation> evaluations, IEnumerable<ISymbolEvaluation> symbolEvaluations)
        {
            var allEvaluations = evaluations.AsArray();
            _triviaEvaluations = allEvaluations.OfType<ITriviaEvaluation>().GroupBy(keySelector: x => x.EvaluatedKind).ToDictionary(keySelector: x => x.Key, elementSelector: x => x.AsArray());
            _codeEvaluations = allEvaluations.OfType<ICodeEvaluation>().GroupBy(keySelector: x => x.EvaluatedKind).ToDictionary(keySelector: x => x.Key, elementSelector: x => x.AsArray());
            _semanticEvaluations = allEvaluations.OfType<ISemanticEvaluation>().GroupBy(keySelector: x => x.EvaluatedKind).ToDictionary(keySelector: x => x.Key, elementSelector: x => x.AsArray());
            _symbolEvaluations = symbolEvaluations.GroupBy(keySelector: x => x.EvaluatedKind).ToDictionary(keySelector: x => x.Key, elementSelector: x => x.AsArray());
        }

        public async Task<IEnumerable<EvaluationResult>> Inspect(Solution solution, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (solution == null)
            {
                return Enumerable.Empty<EvaluationResult>();
            }

            var dataTasks = from project in solution.Projects
                            where project.HasDocuments
                            let compilation = project.GetCompilationAsync(cancellationToken: cancellationToken)
                            from doc in project.Documents
                            let root = doc.SupportsSyntaxTree ? doc.GetSyntaxRootAsync(cancellationToken: cancellationToken) : Task.FromResult<SyntaxNode>(result: null)
                            select GetInspections(filePath: project.FilePath, projectName: project.Name, compilation: compilation, root: root, solution: solution);

            var results = await Task.WhenAll(tasks: dataTasks).ConfigureAwait(continueOnCapturedContext: false);

            return results.SelectMany(selector: x => x).AsArray();
        }

        public async Task<IEnumerable<EvaluationResult>> Inspect(string projectPath, string projectName, SyntaxNode node, SemanticModel semanticModel, Solution solution)
        {
            var inspector = new InnerInspector(triviaEvaluations: _triviaEvaluations, codeEvaluations: _codeEvaluations, semanticEvaluations: _semanticEvaluations, model: semanticModel, solution: solution);
            var inspectionTasks = inspector.Visit(node: node);
            var symbolInspectionTasks = Task.FromResult(result: Enumerable.Empty<EvaluationResult>());

            if (semanticModel != null)
            {
                var symbolInspector = new InnerSymbolAnalyzer(evaluations: _symbolEvaluations, model: semanticModel);
                symbolInspectionTasks = symbolInspector.Visit(node: node);
            }

            await Task.WhenAll(inspectionTasks, symbolInspectionTasks).ConfigureAwait(continueOnCapturedContext: false);

            var inspectionResults = inspectionTasks.Result;
            var symbolInspectionResults = symbolInspectionTasks.Result;
            var allResults = inspectionResults.Concat(second: symbolInspectionResults).AsArray();
            foreach (var result in allResults)
            {
                result.ProjectName = projectName;
                result.ProjectPath = projectPath;
            }

            return allResults.AsEnumerable();
        }

        private async Task<IEnumerable<EvaluationResult>> GetInspections(
            string filePath,
            string projectName,
            Task<Compilation> compilation,
            Task<SyntaxNode> root,
            Solution solution)
        {
            if (root == null || compilation == null || solution == null)
            {
                return Enumerable.Empty<EvaluationResult>();
            }

            var c = await compilation.ConfigureAwait(continueOnCapturedContext: false);
            var r = await root.ConfigureAwait(continueOnCapturedContext: false);
            var model = c.GetSemanticModel(syntaxTree: r.SyntaxTree);
            return await Inspect(projectPath: filePath, projectName: projectName, node: r, semanticModel: model, solution: solution).ConfigureAwait(continueOnCapturedContext: false);
        }

        private class InnerInspector : CSharpSyntaxVisitor<Task<IEnumerable<EvaluationResult>>>
        {
            private readonly IList<SyntaxKind> _supportedSyntaxKinds;
            private readonly IDictionary<SyntaxKind, ITriviaEvaluation[]> _triviaEvaluations;
            private readonly IDictionary<SyntaxKind, ICodeEvaluation[]> _codeEvaluations;
            private readonly IDictionary<SyntaxKind, ISemanticEvaluation[]> _semanticEvaluations;
            private readonly SemanticModel _model;
            private readonly Solution _solution;

            public InnerInspector(IDictionary<SyntaxKind, ITriviaEvaluation[]> triviaEvaluations, IDictionary<SyntaxKind, ICodeEvaluation[]> codeEvaluations, IDictionary<SyntaxKind, ISemanticEvaluation[]> semanticEvaluations, SemanticModel model, Solution solution)
            {
                _supportedSyntaxKinds = codeEvaluations.Select(selector: _ => _.Key).Concat(second: semanticEvaluations.Select(selector: _ => _.Key)).Distinct().AsArray();
                _triviaEvaluations = triviaEvaluations;
                _codeEvaluations = codeEvaluations;
                _semanticEvaluations = semanticEvaluations;
                _model = model;
                _solution = solution;
            }

            public override async Task<IEnumerable<EvaluationResult>> Visit(SyntaxNode node)
            {
                if (node == null)
                {
                    return Enumerable.Empty<EvaluationResult>();
                }

                var nodeChecks = CheckNodes(nodes: node.DescendantNodesAndSelf().Where(predicate: x => x.Kind().In(collection: _supportedSyntaxKinds)).AsArray());
                var tokenResultTasks = node.DescendantTokens().SelectMany(selector: VisitToken);
                var nodeResultTasks = await Task.WhenAll(nodeChecks).ConfigureAwait(continueOnCapturedContext: false);

                var baseResults = nodeResultTasks.SelectMany(selector: x => x).Concat(second: tokenResultTasks);
                return baseResults;
            }

            public override Task<IEnumerable<EvaluationResult>> DefaultVisit(SyntaxNode node)
            {
                return Task.FromResult(result: Enumerable.Empty<EvaluationResult>());
            }

            private static IEnumerable<EvaluationResult> GetTriviaEvaluations(SyntaxTrivia trivia, IEnumerable<ITriviaEvaluation> nodeEvaluations)
            {
                var results = nodeEvaluations.Select(
                    selector: x =>
                    {
                        try
                        {
                            return x.Evaluate(trivia: trivia);
                        }
                        catch (Exception ex)
                        {
                            return new EvaluationResult
                            {
                                Title = ex.Message,
                                Suggestion = ex.StackTrace,
                                ErrorCount = 1,
                                Snippet = trivia.ToFullString(),
                                Quality = CodeQuality.Broken
                            };
                        }
                    })
                        .Where(predicate: x => x != null && x.Quality != CodeQuality.Good)
                        .AsArray();
                return results;
            }

            private static IEnumerable<EvaluationResult> GetCodeEvaluations(SyntaxNode node, IEnumerable<ICodeEvaluation> nodeEvaluations)
            {
                var results = nodeEvaluations
                    .Select(selector: x =>
                    {
                        try
                        {
                            return x.Evaluate(node: node);
                        }
                        catch (Exception ex)
                        {
                            return new EvaluationResult
                            {
                                Title = ex.Message,
                                Suggestion = ex.StackTrace,
                                ErrorCount = 1,
                                Snippet = node.ToFullString(),
                                Quality = CodeQuality.Broken
                            };
                        }
                    })
                    .Where(predicate: x => x != null && x.Quality != CodeQuality.Good)
                    .AsArray();
                return results;
            }

            private static async Task<IEnumerable<EvaluationResult>> GetSemanticEvaluations(SyntaxNode node, IEnumerable<ISemanticEvaluation> nodeEvaluations, SemanticModel model, Solution solution)
            {
                if (model == null || solution == null)
                {
                    return Enumerable.Empty<EvaluationResult>();
                }

                var tasks = nodeEvaluations
                    .Select(selector: async x =>
                        {
                            try
                            {
                                return await x.Evaluate(node: node, semanticModel: model, solution: solution).ConfigureAwait(continueOnCapturedContext: false);
                            }
                            catch (Exception ex)
                            {
                                return new EvaluationResult
                                {
                                    Title = ex.Message,
                                    Suggestion = ex.StackTrace,
                                    ErrorCount = 1,
                                    Snippet = node.ToFullString(),
                                    Quality = CodeQuality.Broken
                                };
                            }
                        });
                var results = (await Task.WhenAll(tasks: tasks).ConfigureAwait(continueOnCapturedContext: false))
                    .Where(predicate: x => x != null && x.Quality != CodeQuality.Good)
                    .AsArray();
                return results;
            }

            private IEnumerable<EvaluationResult> VisitToken(SyntaxToken token)
            {
                var results = token.LeadingTrivia.Concat(second: token.TrailingTrivia)
                    .Where(predicate: x => _triviaEvaluations.ContainsKey(key: x.Kind()))
                    .SelectMany(selector: trivia => GetTriviaEvaluations(trivia: trivia, nodeEvaluations: _triviaEvaluations[key: trivia.Kind()]));

                return results;
            }

            private async Task<IEnumerable<EvaluationResult>> CheckNodes(SyntaxNode[] nodes)
            {
                var semanticResultTasks = nodes.Where(predicate: x => _semanticEvaluations.ContainsKey(key: x.Kind()))
                    .Select(selector: x => CheckSemantics(node: x, kind: x.Kind()));
                var codeResults = nodes.Where(predicate: x => _codeEvaluations.ContainsKey(key: x.Kind()))
                    .SelectMany(selector: x => CheckCode(node: x, kind: x.Kind()));
                var semanticResults = await Task.WhenAll(tasks: semanticResultTasks).ConfigureAwait(continueOnCapturedContext: false);

                return semanticResults.SelectMany(selector: x => x).Concat(second: codeResults);
            }

            private IEnumerable<EvaluationResult> CheckCode(SyntaxNode node, SyntaxKind kind)
            {
                var codeResults = GetCodeEvaluations(node: node, nodeEvaluations: _codeEvaluations[key: kind]);
                return codeResults;
            }

            private async Task<IEnumerable<EvaluationResult>> CheckSemantics(SyntaxNode node, SyntaxKind kind)
            {
                var semanticResults = await GetSemanticEvaluations(node: node, nodeEvaluations: _semanticEvaluations[key: kind], model: _model, solution: _solution).ConfigureAwait(continueOnCapturedContext: false);

                return semanticResults;
            }
        }

        private class InnerSymbolAnalyzer : CSharpSyntaxVisitor<Task<IEnumerable<EvaluationResult>>>
        {
            private readonly IDictionary<SymbolKind, ISymbolEvaluation[]> _evaluations;
            private readonly SemanticModel _model;

            public InnerSymbolAnalyzer(IDictionary<SymbolKind, ISymbolEvaluation[]> evaluations, SemanticModel model)
            {
                _evaluations = evaluations;
                _model = model;
            }

            public override Task<IEnumerable<EvaluationResult>> Visit(SyntaxNode node)
            {
                var results = Task.Run(function: () => node.DescendantNodesAndSelf()
                    .Select(selector: x => _model.GetDeclaredSymbol(declaration: x))
                    .Where(predicate: x => x != null)
                    .Where(predicate: x => x.Kind.In(collection: _evaluations.Keys))
                    .Select(selector: x => new
                    {
                        Symbol = x,
                        Evaluations = _evaluations[key: x.Kind]
                    })
                    .SelectMany(selector: x => x.Evaluations.Select(selector: _ => _.Evaluate(symbol: x.Symbol, semanticModel: _model)))
                    .AsArray()
                    .AsEnumerable());

                return results;
            }
        }
    }
}
