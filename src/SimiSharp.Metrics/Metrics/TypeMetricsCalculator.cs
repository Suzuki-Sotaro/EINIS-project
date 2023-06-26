// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeMetricsCalculator type.
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
    internal sealed class TypeMetricsCalculator : SemanticModelMetricsCalculator
    {
        private readonly Solution _solution;
        private readonly IAsyncFactory<ISymbol, ITypeDocumentation> _documentationFactory;

        public TypeMetricsCalculator(SemanticModel semanticModel, Solution solution, IAsyncFactory<ISymbol, ITypeDocumentation> documentationFactory)
            : base(semanticModel: semanticModel)
        {
            _solution = solution;
            _documentationFactory = documentationFactory;
        }

        public async Task<ITypeMetric> CalculateFrom(TypeDeclarationSyntaxInfo typeNode, IEnumerable<IMemberMetric> metrics)
        {
            var memberMetrics = metrics.AsArray();
            var type = typeNode.Syntax;
            var symbol = Model.GetDeclaredSymbol(declarationSyntax: type);
            var documentation = await _documentationFactory.Create(memberSymbol: symbol, cancellationToken: CancellationToken.None);
            var metricKind = GetMetricKind(type: type);
            var source = CalculateClassCoupling(type: type, memberMetrics: memberMetrics);
            var depthOfInheritance = CalculateDepthOfInheritance(type: type);
            var cyclomaticComplexity = memberMetrics.Sum(selector: x => x.CyclomaticComplexity);
            var linesOfCode = memberMetrics.Sum(selector: x => x.LinesOfCode);
            var maintainabilityIndex = CalculateAveMaintainabilityIndex(memberMetrics: memberMetrics);
            var afferentCoupling = await CalculateAfferentCoupling(node: type);
            var efferentCoupling = GetEfferentCoupling(classDeclaration: type, sourceSymbol: symbol);
            var instability = (double)efferentCoupling / (efferentCoupling + afferentCoupling);
            var modifier = GetAccessModifier(tokenList: type.Modifiers);
            return new TypeMetric(
                isAbstract: symbol.IsAbstract,
                kind: metricKind,
                accessModifier: modifier,
                memberMetrics: memberMetrics,
                linesOfCode: linesOfCode,
                cyclomaticComplexity: cyclomaticComplexity,
                maintainabilityIndex: maintainabilityIndex,
                depthOfInheritance: depthOfInheritance,
                classCouplings: source,
                name: type.GetName(),
                afferentCoupling: afferentCoupling,
                efferentCoupling: efferentCoupling,
                instability: instability,
                documentation: documentation);
        }

        private static double CalculateAveMaintainabilityIndex(IEnumerable<IMemberMetric> memberMetrics)
        {
            var source = memberMetrics.Select(selector: x => new Tuple<int, double>(item1: x.LinesOfCode, item2: x.MaintainabilityIndex)).AsArray();
            if (source.Any())
            {
                var totalLinesOfCode = source.Sum(selector: x => x.Item1);
                return totalLinesOfCode == 0 ? 100.0 : source.Sum(selector: x => x.Item1 * x.Item2) / totalLinesOfCode;
            }

            return 100.0;
        }

        private static TypeMetricKind GetMetricKind(TypeDeclarationSyntax type)
        {
            switch (type.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return TypeMetricKind.Class;
                case SyntaxKind.StructDeclaration:
                    return TypeMetricKind.Struct;
                case SyntaxKind.InterfaceDeclaration:
                    return TypeMetricKind.Interface;
                default:
                    return TypeMetricKind.Unknown;
            }
        }

        private int GetEfferentCoupling(SyntaxNode classDeclaration, ISymbol sourceSymbol)
        {
            var typeSyntaxes = classDeclaration.DescendantNodesAndSelf().OfType<TypeSyntax>();
            var commonSymbolInfos = typeSyntaxes.Select(selector: x => Model.GetSymbolInfo(expression: x)).AsArray();
            var members = commonSymbolInfos
                .Select(selector: x => x.Symbol)
                .Where(predicate: x => x != null)
                .Select(selector: x =>
                    {
                        var typeSymbol = x as ITypeSymbol;
                        return typeSymbol == null ? x.ContainingType : x;
                    })
                .Cast<ITypeSymbol>()
                .WhereNotNull()
                .DistinctBy(func: x => x.ToDisplayString())
                .Count(predicate: x => !SymbolEqualityComparer.Default.Equals(x, sourceSymbol));

            return members;
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
                                         : _solution.FindReferences(symbol: symbol).ContinueWith(continuationFunction: t => t.Exception != null ? 0 : t.Result.Locations.Count());

                return await referenceTasks.ConfigureAwait(continueOnCapturedContext: false);
            }
            catch
            {
                // Some types are not present in syntax tree because they have been created for metrics calculation.
                return 0;
            }
        }

        private AccessModifierKind GetAccessModifier(SyntaxTokenList tokenList)
        {
            if (tokenList.Any(kind: SyntaxKind.PublicKeyword))
            {
                return AccessModifierKind.Public;
            }

            if (tokenList.Any(kind: SyntaxKind.PrivateKeyword))
            {
                return AccessModifierKind.Private;
            }

            return AccessModifierKind.Internal;
        }

        private IEnumerable<ITypeCoupling> CalculateClassCoupling(TypeDeclarationSyntax type, IEnumerable<IMemberMetric> memberMetrics)
        {
            var second = new TypeClassCouplingAnalyzer(semanticModel: Model).Calculate(typeNode: type);
            return memberMetrics.SelectMany(selector: x => x.Dependencies)
                .Concat(second: second)
                .GroupBy(keySelector: x => x.ToString())
                .Select(selector: x => new TypeCoupling(typeName: x.First().TypeName, namespaceName: x.First().Namespace, assemblyName: x.First().Assembly, usedMethods: x.SelectMany(selector: y => y.UsedMethods), usedProperties: x.SelectMany(selector: y => y.UsedProperties), useEvents: x.SelectMany(selector: y => y.UsedEvents)))
                .OrderBy(keySelector: x => x.TypeName)
                .AsArray();
        }

        private int CalculateDepthOfInheritance(TypeDeclarationSyntax type)
        {
            var analyzer = new DepthOfInheritanceAnalyzer(semanticModel: Model);
            return analyzer.Calculate(type: type);
        }
    }
}
