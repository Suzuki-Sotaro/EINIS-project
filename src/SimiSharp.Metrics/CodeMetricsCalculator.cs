// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.Metrics;
using SimiSharp.CodeAnalysis.Metrics;

namespace SimiSharp.CodeAnalysis
{
    public class CodeMetricsCalculator : ICodeMetricsCalculator
    {
        private static readonly List<Regex> Patterns = new List<Regex>
                                                       {
                                                           new Regex(pattern: @".*\.g\.cs$", options: RegexOptions.Compiled),
                                                           new Regex(pattern: @".*\.g\.i\.cs$", options: RegexOptions.Compiled),
                                                           new Regex(pattern: @".*\.designer\.cs$", options: RegexOptions.Compiled)
                                                       };

        private readonly IAsyncFactory<ISymbol, ITypeDocumentation> _typeDocumentationFactory;
        private readonly IAsyncFactory<ISymbol, IMemberDocumentation> _memberDocumentationFactory;
        private readonly SyntaxCollector _syntaxCollector = new SyntaxCollector();

        public CodeMetricsCalculator()
            : this(typeDocumentationFactory: new TypeDocumentationFactory(), memberDocumentationFactory: new MemberDocumentationFactory())
        {
        }

        public CodeMetricsCalculator(IAsyncFactory<ISymbol, ITypeDocumentation> typeDocumentationFactory, IAsyncFactory<ISymbol, IMemberDocumentation> memberDocumentationFactory)
        {
            _typeDocumentationFactory = typeDocumentationFactory;
            _memberDocumentationFactory = memberDocumentationFactory;
        }

        public virtual async Task<IEnumerable<INamespaceMetric>> Calculate(Project project, Solution solution)
        {
            var compilation = await project.GetCompilationAsync().ConfigureAwait(continueOnCapturedContext: false);
            var namespaceDeclarations = await GetNamespaceDeclarations(project: project).ConfigureAwait(continueOnCapturedContext: false);
            return await CalculateNamespaceMetrics(namespaceDeclarations: namespaceDeclarations, compilation: compilation, solution: solution).ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<IEnumerable<INamespaceMetric>> Calculate(IEnumerable<SyntaxTree> syntaxTrees, params Assembly[] references)
        {
            var trees = syntaxTrees.AsArray();
            var declarations = _syntaxCollector.GetDeclarations(trees: trees);
            var statementMembers = declarations.Statements.Select(selector: s =>
                s is StatementSyntax
                ? SyntaxFactory.MethodDeclaration(
                    returnType: SyntaxFactory.PredefinedType(keyword: SyntaxFactory.Token(kind: SyntaxKind.VoidKeyword)),
                    identifier: Guid.NewGuid().ToString(format: "N"))
                    .WithBody(body: SyntaxFactory.Block((StatementSyntax)s))
                    : s);
            var members = declarations.MemberDeclarations.Concat(second: statementMembers).AsArray();
            var anonClass = members.Any()
                                ? new[]
                                  {
                                      SyntaxFactory.ClassDeclaration(
                                          identifier: "UnnamedClass")
                                          .WithModifiers(
                                              modifiers: SyntaxFactory.TokenList(token: SyntaxFactory.Token(kind: SyntaxKind.PublicKeyword)))
                                          .WithMembers(members: SyntaxFactory.List(nodes: members))
                                  }
                                : new TypeDeclarationSyntax[0];
            var array = declarations.TypeDeclarations
                .Concat(second: anonClass)
                .Cast<MemberDeclarationSyntax>()
                .AsArray();
            var anonNs = array.Any()
                ? new[]
                          {
                              SyntaxFactory.NamespaceDeclaration(name: SyntaxFactory.ParseName(text: "Unnamed"))
                                  .WithMembers(members: SyntaxFactory.List(nodes: array))
                          }
                : new NamespaceDeclarationSyntax[0];
            var namespaceDeclarations = declarations
                .NamespaceDeclarations
                .Concat(second: anonNs)
                .Select(selector: x => new NamespaceDeclarationSyntaxInfo(codefile: null, name: x.GetName(rootNode: x), syntax: x))
                .GroupBy(keySelector: x => x.Name)
                .Select(selector: g => new NamespaceDeclaration
                {
                    Name = g.Key,
                    SyntaxNodes = g.AsArray()
                })
                .AsArray();

            var metadataReferences =
                (references.Any() ? references : new[] { typeof(object).GetTypeInfo().Assembly }).Select(
                    selector: a => MetadataReference.CreateFromFile(path: a.Location)).ToArray();
            var commonCompilation = CSharpCompilation.Create(assemblyName: "x", syntaxTrees: trees, references: metadataReferences);
            var namespaceMetrics = await CalculateNamespaceMetrics(namespaceDeclarations: namespaceDeclarations, compilation: commonCompilation, solution: null).ConfigureAwait(continueOnCapturedContext: false);
            return namespaceMetrics;
        }

        private static async Task<INamespaceMetric> CalculateNamespaceMetrics(Compilation compilation, NamespaceDeclaration namespaceNodes, IEnumerable<ITypeMetric> typeMetrics)
        {
            var namespaceNode = namespaceNodes.SyntaxNodes.FirstOrDefault();
            if (namespaceNode == null)
            {
                return null;
            }

            var tuple = await VerifyCompilation(compilation: compilation, namespaceNode: namespaceNode).ConfigureAwait(continueOnCapturedContext: false);
            compilation = tuple.Item1;
            var semanticModel = compilation.GetSemanticModel(syntaxTree: tuple.Item3);
            var calculator = new NamespaceMetricsCalculator(semanticModel: semanticModel);
            return calculator.CalculateFrom(namespaceNode: namespaceNode, metrics: typeMetrics);
        }

        private async Task<Tuple<Compilation, ITypeMetric>> CalculateTypeMetrics(Solution solution, Compilation compilation, TypeDeclaration typeNodes, IEnumerable<IMemberMetric> memberMetrics)
        {
            if (typeNodes.SyntaxNodes.Any())
            {
                var tuple = await VerifyCompilation(compilation: compilation, typeNode: typeNodes.SyntaxNodes.First()).ConfigureAwait(continueOnCapturedContext: false);
                var semanticModel = tuple.Item2;
                compilation = tuple.Item1;
                var typeNode = tuple.Item3;
                var calculator = new TypeMetricsCalculator(semanticModel: semanticModel, solution: solution, documentationFactory: _typeDocumentationFactory);
                var metrics = await calculator.CalculateFrom(typeNode: typeNode, metrics: memberMetrics).ConfigureAwait(continueOnCapturedContext: false);
                return new Tuple<Compilation, ITypeMetric>(
                    item1: compilation,
                    item2: metrics);
            }

            return null;
        }

        private static async Task<Tuple<Compilation, SemanticModel, TypeDeclarationSyntaxInfo>> VerifyCompilation(Compilation compilation, TypeDeclarationSyntaxInfo typeNode)
        {
            var tree = typeNode.Syntax.SyntaxTree;

            if (tree == null)
            {
                var cu = CSharpSyntaxTree.Create(
                    root: SyntaxFactory
                    .CompilationUnit()
                    .WithMembers(members: SyntaxFactory.List(nodes: new[] { (MemberDeclarationSyntax)typeNode.Syntax })));
                var root = await cu.GetRootAsync().ConfigureAwait(continueOnCapturedContext: false);
                typeNode.Syntax = (TypeDeclarationSyntax)root.ChildNodes().First();
                var newCompilation = compilation.AddSyntaxTrees(cu);
                var semanticModel = newCompilation.GetSemanticModel(syntaxTree: cu);
                return new Tuple<Compilation, SemanticModel, TypeDeclarationSyntaxInfo>(item1: newCompilation, item2: semanticModel, item3: typeNode);
            }

            var result = AddToCompilation(compilation: compilation, tree: tree);
            var childNodes = result.Item2.GetRoot().DescendantNodesAndSelf();
            typeNode.Syntax = childNodes.OfType<TypeDeclarationSyntax>().First();
            return new Tuple<Compilation, SemanticModel, TypeDeclarationSyntaxInfo>(
                item1: result.Item1,
                item2: result.Item1.GetSemanticModel(syntaxTree: result.Item2),
                item3: typeNode);
        }

        private static Tuple<Compilation, SyntaxTree> AddToCompilation(Compilation compilation, SyntaxTree tree)
        {
            if (!compilation.ContainsSyntaxTree(syntaxTree: tree))
            {
                var newTree = tree;
                if (!tree.HasCompilationUnitRoot)
                {
                    var childNodes = tree.GetRoot()
                        .ChildNodes()
                        .AsArray();
                    newTree = CSharpSyntaxTree.Create(root: SyntaxFactory.CompilationUnit()
                        .WithMembers(
                            members: SyntaxFactory.List(nodes: childNodes.OfType<MemberDeclarationSyntax>()))
                        .WithUsings(
                            usings: SyntaxFactory.List(nodes: childNodes.OfType<UsingDirectiveSyntax>()))
                        .WithExterns(
                            externs: SyntaxFactory.List(nodes: childNodes.OfType<ExternAliasDirectiveSyntax>())));
                }

                var comp = compilation.AddSyntaxTrees(newTree);
                return new Tuple<Compilation, SyntaxTree>(item1: comp, item2: newTree);
            }

            return new Tuple<Compilation, SyntaxTree>(item1: compilation, item2: tree);
        }

        private static async Task<Tuple<Compilation, SemanticModel, SyntaxTree, NamespaceDeclarationSyntaxInfo>> VerifyCompilation(Compilation compilation, NamespaceDeclarationSyntaxInfo namespaceNode)
        {
            SemanticModel semanticModel;
            var tree = namespaceNode.Syntax.SyntaxTree;
            if (tree == null)
            {
                var compilationUnit = SyntaxFactory.CompilationUnit()
                    .WithMembers(members: SyntaxFactory.List(nodes: new[] { (MemberDeclarationSyntax)namespaceNode.Syntax }));
                var cu = CSharpSyntaxTree.Create(root: compilationUnit);
                var root = await cu.GetRootAsync().ConfigureAwait(continueOnCapturedContext: false);
                namespaceNode.Syntax = root.ChildNodes().First();
                var newCompilation = compilation.AddSyntaxTrees(cu);
                semanticModel = newCompilation.GetSemanticModel(syntaxTree: cu);
                return new Tuple<Compilation, SemanticModel, SyntaxTree, NamespaceDeclarationSyntaxInfo>(item1: newCompilation, item2: semanticModel, item3: cu, item4: namespaceNode);
            }

            var result = AddToCompilation(compilation: compilation, tree: tree);
            compilation = result.Item1;
            tree = result.Item2;
            semanticModel = compilation.GetSemanticModel(syntaxTree: tree);
            return new Tuple<Compilation, SemanticModel, SyntaxTree, NamespaceDeclarationSyntaxInfo>(item1: compilation, item2: semanticModel, item3: tree, item4: namespaceNode);
        }

        private static async Task<IEnumerable<NamespaceDeclaration>> GetNamespaceDeclarations(Project project)
        {
            var namespaceDeclarationTasks = project.Documents
                .Select(selector: document => new { document, codeFile = document.FilePath })
                .Where(predicate: t => !IsGeneratedCodeFile(doc: t.document, patterns: Patterns))
                .Select(
                    selector: async t =>
                    {
                        var collector = new NamespaceCollector();
                        var root = await t.document.GetSyntaxRootAsync().ConfigureAwait(continueOnCapturedContext: false);
                        return new
                        {
                            t.codeFile,
                            namespaces = collector.GetNamespaces(commonNode: root)
                        };
                    })
                .Select(
                    selector: async t =>
                    {
                        var result = await t.ConfigureAwait(continueOnCapturedContext: false);
                        return result.namespaces
                            .Select(
                                selector: x => new NamespaceDeclarationSyntaxInfo(codefile: result.codeFile, name: x.GetName(rootNode: x.SyntaxTree.GetRoot()), syntax: x));
                    });
            var namespaceDeclarations = await Task.WhenAll(tasks: namespaceDeclarationTasks).ConfigureAwait(continueOnCapturedContext: false);
            return namespaceDeclarations
                .SelectMany(selector: x => x)
                .GroupBy(keySelector: x => x.Name)
                .Select(selector: y => new NamespaceDeclaration { Name = y.Key, SyntaxNodes = y });
        }

        private static bool IsGeneratedCodeFile(TextDocument doc, IEnumerable<Regex> patterns)
        {
            var path = doc.FilePath;
            return !string.IsNullOrWhiteSpace(value: path) && patterns.Any(predicate: x => x.IsMatch(input: path));
        }

        private static IEnumerable<TypeDeclaration> GetTypeDeclarations(NamespaceDeclaration namespaceDeclaration)
        {
            var collector = new TypeCollector();
            return namespaceDeclaration.SyntaxNodes
                .Select(selector: info =>
                {
                    Func<TypeDeclarationSyntax, TypeDeclarationSyntaxInfo> selector =
                        x => new TypeDeclarationSyntaxInfo(codeFile: info.CodeFile, name: x.SyntaxTree == null ? x.Identifier.ValueText : x.GetName(rootNode: x.SyntaxTree.GetRoot()), syntax: x);
                    return new { info, selector };
                })
                .SelectMany(selector: x => collector.GetTypes(namespaceNode: x.info.Syntax).Select(selector: x.selector))
                .GroupBy(keySelector: x => x.Name)
                .Select(selector: x => new TypeDeclaration { Name = x.Key, SyntaxNodes = x });
        }

        private async Task<IEnumerable<INamespaceMetric>> CalculateNamespaceMetrics(IEnumerable<NamespaceDeclaration> namespaceDeclarations, Compilation compilation, Solution solution)
        {
            var tasks = namespaceDeclarations.Select(
                selector: async arg =>
                {
                    var tuple = await CalculateTypeMetrics(compilation: compilation, namespaceNodes: arg, solution: solution).ConfigureAwait(continueOnCapturedContext: false);
                    return CalculateNamespaceMetrics(compilation: tuple.Item1, namespaceNodes: arg, typeMetrics: tuple.Item2.AsArray());
                })
                    .AsArray();
            var x = await Task.WhenAll(tasks: tasks).ConfigureAwait(continueOnCapturedContext: false);
            return await Task.WhenAll(tasks: x).ConfigureAwait(continueOnCapturedContext: false);
        }

        private async Task<Tuple<Compilation, IEnumerable<IMemberMetric>>> CalculateMemberMetrics(Compilation compilation, TypeDeclaration typeNodes, Solution solution)
        {
            var comp = compilation;
            var metrics = typeNodes.SyntaxNodes
                .Select(selector: async info =>
                {
                    var tuple = await VerifyCompilation(compilation: comp, typeNode: info).ConfigureAwait(continueOnCapturedContext: false);
                    var semanticModel = tuple.Item2;
                    comp = tuple.Item1;
                    var calculator = new MemberMetricsCalculator(semanticModel: semanticModel, solution: solution, rootFolder: solution?.FilePath.GetParentFolder(), documentationFactory: _memberDocumentationFactory);

                    return await calculator.Calculate(typeNode: info).ConfigureAwait(continueOnCapturedContext: false);
                });
            var results = await Task.WhenAll(tasks: metrics).ConfigureAwait(continueOnCapturedContext: false);
            return new Tuple<Compilation, IEnumerable<IMemberMetric>>(item1: comp, item2: results.SelectMany(selector: x => x).AsArray());
        }

        private async Task<Tuple<Compilation, IEnumerable<ITypeMetric>>> CalculateTypeMetrics(Compilation compilation, NamespaceDeclaration namespaceNodes, Solution solution)
        {
            var comp = compilation;
            var tasks = GetTypeDeclarations(namespaceDeclaration: namespaceNodes)
                .Select(selector: async typeNodes =>
                    {
                        var tuple = await CalculateMemberMetrics(compilation: comp, typeNodes: typeNodes, solution: solution).ConfigureAwait(continueOnCapturedContext: false);
                        var metrics = tuple.Item2;
                        comp = tuple.Item1;
                        return new
                        {
                            comp,
                            typeNodes,
                            solution,
                            memberMetrics = metrics
                        };
                    })
                    .AsArray();
            var data = await Task.WhenAll(tasks: tasks).ConfigureAwait(continueOnCapturedContext: false);
            var typeMetricsTasks = data
                .Select(selector: async item =>
                {
                    var tuple = await CalculateTypeMetrics(solution: item.solution, compilation: item.comp, typeNodes: item.typeNodes, memberMetrics: item.memberMetrics).ConfigureAwait(continueOnCapturedContext: false);
                    if (tuple == null)
                    {
                        return null;
                    }

                    comp = tuple.Item1;
                    return tuple.Item2;
                })
                .AsArray();

            var typeMetrics = await Task.WhenAll(tasks: typeMetricsTasks).ConfigureAwait(continueOnCapturedContext: false);
            var array = typeMetrics.Where(predicate: x => x != null).AsArray();
            return new Tuple<Compilation, IEnumerable<ITypeMetric>>(item1: comp, item2: array);
        }
    }
}
