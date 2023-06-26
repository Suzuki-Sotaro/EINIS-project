// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooHighCyclomaticComplexityRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TooHighCyclomaticComplexityRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;
using SimiSharp.CodeAnalysis.Metrics;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class TooHighCyclomaticComplexityRule : CodeEvaluationBase
	{
		private const int Limit = 10;
		private readonly CyclomaticComplexityCounter _counter = new CyclomaticComplexityCounter();

		public override SyntaxKind EvaluatedKind => SyntaxKind.MethodDeclaration;

		public override string ID => "AM0046";

		public override string Title => "Method Too Complex.";

		public override string Suggestion => "Refactor to reduce number of code paths through method.";

		public override CodeQuality Quality => CodeQuality.NeedsRefactoring;

		public override QualityAttribute QualityAttribute => QualityAttribute.Testability | QualityAttribute.Maintainability | QualityAttribute.Modifiability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var methodDeclaration = (MethodDeclarationSyntax)node;
			var complexity = _counter.Calculate(node: methodDeclaration, semanticModel: null);
			if (complexity >= Limit)
			{
				return new EvaluationResult
						   {
							   Snippet = node.ToFullString()
						   };
			}

			return null;
		}
	}
}
