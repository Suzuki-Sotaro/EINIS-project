// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooLowMaintainabilityIndexRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TooLowMaintainabilityIndexRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.CodeReview;
using SimiSharp.CodeAnalysis.Metrics;

namespace SimiSharp.CodeReview.Rules.Semantic
{
    internal class TooLowMaintainabilityIndexRule : SemanticEvaluationBase
    {
        public TooLowMaintainabilityIndexRule()
        {
            Threshold = 40;
        }

        public override string ID => "AM0058";

        public override SyntaxKind EvaluatedKind => SyntaxKind.MethodDeclaration;

        public override string Title => "Method Unmaintainable";

        public override string Suggestion => "Refactor method to improve maintainability.";

        public override CodeQuality Quality => CodeQuality.NeedsRefactoring;

        public override QualityAttribute QualityAttribute => QualityAttribute.Testability | QualityAttribute.Maintainability | QualityAttribute.Modifiability;

        public override ImpactLevel ImpactLevel => ImpactLevel.Member;

        public int Threshold { get; }

        protected override Task<EvaluationResult> EvaluateImpl(SyntaxNode node, SemanticModel semanticModel, Solution solution)
        {
            var counter = new MemberMetricsCalculator(semanticModel: semanticModel, solution: solution, rootFolder: solution.FilePath.GetParentFolder(), documentationFactory: new MemberDocumentationFactory());

            var methodDeclaration = (MethodDeclarationSyntax)node;
            var metric = counter.CalculateSlim(methodDeclaration: methodDeclaration);
            return metric.MaintainabilityIndex <= Threshold
                       ? Task.FromResult(
                           result: new EvaluationResult
                           {
                               Snippet = node.ToFullString()
                           })
                       : Task.FromResult(result: (EvaluationResult)null);
        }
    }
}