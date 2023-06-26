// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.Metrics;

namespace SimiSharp.CodeAnalysis.Metrics
{
    public sealed class MemberMetricsCalculator : SemanticModelMetricsCalculator
    {
        private readonly CyclomaticComplexityCounter _counter = new CyclomaticComplexityCounter();
        private readonly LinesOfCodeCalculator _locCalculator = new LinesOfCodeCalculator();
        private readonly MemberNameResolver _nameResolver;
        private readonly Solution _solution;
        private readonly string _rootFolder;
        private readonly IAsyncFactory<ISymbol, IMemberDocumentation> _documentationFactory;

        public MemberMetricsCalculator(SemanticModel semanticModel, Solution solution, string rootFolder, IAsyncFactory<ISymbol, IMemberDocumentation> documentationFactory)
            : base(semanticModel: semanticModel)
        {
            _solution = solution;
            _rootFolder = rootFolder;
            _documentationFactory = documentationFactory;
            _nameResolver = new MemberNameResolver(semanticModel: Model);
        }

        public async Task<IEnumerable<IMemberMetric>> Calculate(TypeDeclarationSyntaxInfo typeNode)
        {
            var walker = new MemberCollector();
            var members = walker.GetMembers(type: typeNode).AsArray();
            if ((typeNode.Syntax is ClassDeclarationSyntax
                || typeNode.Syntax is StructDeclarationSyntax)
                && members.All(predicate: m => m.Kind() != SyntaxKind.ConstructorDeclaration))
            {
                var defaultConstructor = SyntaxFactory.ConstructorDeclaration(identifier: typeNode.Name)
                                               .WithModifiers(modifiers: SyntaxFactory.TokenList(token: SyntaxFactory.Token(kind: SyntaxKind.PublicKeyword)))
                                               .WithBody(body: SyntaxFactory.Block());
                members = members.Concat(second: new[] { defaultConstructor }).AsArray();
            }

            var metrics = await CalculateMemberMetrics(nodes: members).ConfigureAwait(continueOnCapturedContext: false);
            return metrics.AsArray();
        }

        public IMemberMetric CalculateSlim(MethodDeclarationSyntax methodDeclaration)
        {
            return CalculateMemberMetricSlim(syntaxNode: methodDeclaration);
        }

        private static double CalculateMaintainablityIndex(double cyclomaticComplexity, double linesOfCode, IHalsteadMetrics halsteadMetrics)
        {
            if (linesOfCode.Equals(obj: 0.0) || halsteadMetrics.NumberOfOperands.Equals(obj: 0) || halsteadMetrics.NumberOfOperators.Equals(obj: 0))
            {
                return 100.0;
            }

            var num = Math.Log(d: halsteadMetrics.GetVolume());
            var mi = ((171 - (5.2 * num) - (0.23 * cyclomaticComplexity) - (16.2 * Math.Log(d: linesOfCode))) * 100) / 171;

            return Math.Max(val1: 0.0, val2: mi);
        }

        private int CalculateLinesOfCode(SyntaxNode node)
        {
            return _locCalculator.Calculate(node: node);
        }

        private int CalculateCyclomaticComplexity(SyntaxNode node)
        {
            return _counter.Calculate(node: node, semanticModel: Model);
        }

        private IEnumerable<ITypeCoupling> CalculateClassCoupling(SyntaxNode node)
        {
            var provider = new MemberClassCouplingAnalyzer(semanticModel: Model);
            return provider.Calculate(syntaxNode: node);
        }

        private async Task<IEnumerable<IMemberMetric>> CalculateMemberMetrics(IEnumerable<SyntaxNode> nodes)
        {
            var tasks = nodes.Select(selector: CalculateMemberMetric);

            var metrics = await Task.WhenAll(tasks: tasks).ConfigureAwait(continueOnCapturedContext: false);
            return from metric in metrics
                   where metric != null
                   select metric;
        }

        private async Task<IMemberMetric> CalculateMemberMetric(SyntaxNode syntaxNode)
        {
            var analyzer = new HalsteadAnalyzer();
            var halsteadMetrics = analyzer.Calculate(syntax: syntaxNode);
            var memberName = _nameResolver.TryResolveMemberSignatureString(syntaxNode: syntaxNode);
            var source = CalculateClassCoupling(node: syntaxNode);
            var complexity = CalculateCyclomaticComplexity(node: syntaxNode);
            var linesOfCode = CalculateLinesOfCode(node: syntaxNode);
            var numberOfParameters = CalculateNumberOfParameters(node: syntaxNode);
            var numberOfLocalVariables = CalculateNumberOfLocalVariables(node: syntaxNode);
            var maintainabilityIndex = CalculateMaintainablityIndex(cyclomaticComplexity: complexity, linesOfCode: linesOfCode, halsteadMetrics: halsteadMetrics);
            var afferentCoupling = await CalculateAfferentCoupling(node: syntaxNode).ConfigureAwait(continueOnCapturedContext: false);
            var location = syntaxNode.GetLocation();
            var lineNumber = location.GetLineSpan().StartLinePosition.Line;
            var filePath = location.SourceTree == null ? string.Empty : location.SourceTree.FilePath;
            filePath = filePath.GetPathRelativeTo(other: _rootFolder);
            var accessModifier = GetAccessModifier(node: syntaxNode);
            IMemberDocumentation documentation = null;

            if (syntaxNode.SyntaxTree == Model.SyntaxTree)
            {
                var symbol = Model.GetDeclaredSymbol(declaration: syntaxNode);
                documentation = await _documentationFactory.Create(memberSymbol: symbol, cancellationToken: CancellationToken.None);
            }

            return new MemberMetric(codeFile: filePath,
                accessModifier: accessModifier,
                halstead: halsteadMetrics,
                lineNumber: lineNumber,
                linesOfCode: linesOfCode,
                maintainabilityIndex: maintainabilityIndex,
                cyclomaticComplexity: complexity,
                name: memberName,
                classCouplings: source.AsArray(),
                numberOfParameters: numberOfParameters,
                numberOfLocalVariables: numberOfLocalVariables,
                afferentCoupling: afferentCoupling,
                documentation: documentation);
        }

        private IMemberMetric CalculateMemberMetricSlim(SyntaxNode syntaxNode)
        {
            var analyzer = new HalsteadAnalyzer();
            var halsteadMetrics = analyzer.Calculate(syntax: syntaxNode);
            var memberName = _nameResolver.TryResolveMemberSignatureString(syntaxNode: syntaxNode);
            var source = Enumerable.Empty<ITypeCoupling>();
            var complexity = CalculateCyclomaticComplexity(node: syntaxNode);
            var linesOfCode = CalculateLinesOfCode(node: syntaxNode);
            const int NumberOfParameters = 0;
            const int NumberOfLocalVariables = 0;
            var maintainabilityIndex = CalculateMaintainablityIndex(cyclomaticComplexity: complexity, linesOfCode: linesOfCode, halsteadMetrics: halsteadMetrics);
            const int AfferentCoupling = 0;
            const int LineNumber = 0;
            var filePath = string.Empty;
            var accessModifier = GetAccessModifier(node: syntaxNode);
            return new MemberMetric(codeFile: filePath,
                accessModifier: accessModifier,
                halstead: halsteadMetrics,
                lineNumber: LineNumber,
                linesOfCode: linesOfCode,
                maintainabilityIndex: maintainabilityIndex,
                cyclomaticComplexity: complexity,
                name: memberName,
                classCouplings: source.AsArray(),
                numberOfParameters: NumberOfParameters,
                numberOfLocalVariables: NumberOfLocalVariables,
                afferentCoupling: AfferentCoupling,
                documentation: null);
        }

        private AccessModifierKind GetAccessModifier(SyntaxNode node)
        {
            var method = node as BaseMethodDeclarationSyntax;
            if (method != null)
            {
                return GetAccessModifier(tokenList: method.Modifiers);
            }

            var property = node as BasePropertyDeclarationSyntax;
            if (property != null)
            {
                return GetAccessModifier(tokenList: property.Modifiers);
            }

            return AccessModifierKind.Private;
        }

        private AccessModifierKind GetAccessModifier(SyntaxTokenList tokenList)
        {
            if (tokenList.Any(kind: SyntaxKind.PublicKeyword))
            {
                return AccessModifierKind.Public;
            }

            if (tokenList.Any(kind: SyntaxKind.ProtectedKeyword))
            {
                var isInternal = tokenList.Any(kind: SyntaxKind.InternalKeyword);
                return isInternal ? AccessModifierKind.Internal | AccessModifierKind.Protected : AccessModifierKind.Protected;
            }

            return AccessModifierKind.Private;
        }

        private async Task<int> CalculateAfferentCoupling(SyntaxNode node)
        {
            try
            {
                if (_solution == null)
                {
                    return 0;
                }

                if (node.SyntaxTree != Model.SyntaxTree)
                {
                    return 0;
                }

                var symbol = Model.GetDeclaredSymbol(declaration: node);
                var referenceTasks = symbol == null
                ? Task.FromResult(result: 0)
                : _solution.FindReferences(symbol: symbol)
                    .ContinueWith(continuationFunction: t => t.Exception != null ? 0 : t.Result.Locations.Count());

                return await referenceTasks.ConfigureAwait(continueOnCapturedContext: false);
            }
            catch
            {
                // Some constructors are not present in syntax tree because they have been created for metrics calculation.
                return 0;
            }
        }

        private int CalculateNumberOfLocalVariables(SyntaxNode node)
        {
            var analyzer = new MethodLocalVariablesAnalyzer();
            return analyzer.Calculate(node: node);
        }

        private int CalculateNumberOfParameters(SyntaxNode node)
        {
            var member = node as BaseMethodDeclarationSyntax;
            if (member != null)
            {
                return member.ParameterList.Parameters.Count;
            }

            var accessor = node as AccessorDeclarationSyntax;
            return accessor != null && accessor.IsKind(kind: SyntaxKind.SetAccessorDeclaration) ? 1 : 0;
        }
    }
}
