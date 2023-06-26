// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooBigMethodRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TooBigMethodRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class TooBigMethodRule : CodeEvaluationBase
	{
		private const int Limit = 30;

		public override string ID => "AM0044";

		public override SyntaxKind EvaluatedKind => SyntaxKind.MethodDeclaration;

		public override string Title => "Method Too Big";

		public override string Suggestion => "Refactor method to make it more manageable.";

		public override CodeQuality Quality => CodeQuality.NeedsRefactoring;

		public override QualityAttribute QualityAttribute => QualityAttribute.Testability | QualityAttribute.Maintainability | QualityAttribute.Modifiability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var methodDeclaration = (MethodDeclarationSyntax)node;
			var snippet = methodDeclaration.ToFullString();
			var linesOfCode = GetLinesOfCode(node: node);

			if (linesOfCode >= Limit)
			{
				return new EvaluationResult
						   {
							   LinesOfCodeAffected = linesOfCode, 
							   Snippet = snippet
						   };
			}

			return null;
		}
	}
}
