// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooBigClassRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TooBigClassRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class TooBigClassRule : CodeEvaluationBase
	{
		private const int Limit = 300;

		public override string ID => "AM0043";

		public override SyntaxKind EvaluatedKind => SyntaxKind.ClassDeclaration;

		public override string Title => "Class Too Big";

		public override string Suggestion => "Refactor class to make it more manageable.";

		public override CodeQuality Quality => CodeQuality.NeedsRefactoring;

		public override QualityAttribute QualityAttribute => QualityAttribute.Testability | QualityAttribute.Maintainability | QualityAttribute.Modifiability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Type;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var declarationSyntax = (TypeDeclarationSyntax)node;
			var snippet = declarationSyntax.ToFullString();
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
