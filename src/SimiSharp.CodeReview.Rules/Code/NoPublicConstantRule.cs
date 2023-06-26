// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoPublicConstantRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NoPublicConstantRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class NoPublicConstantRule : CodeEvaluationBase
	{
		public override string ID => "AM0032";

		public override SyntaxKind EvaluatedKind => SyntaxKind.FieldDeclaration;

		public override string Title => "No Public Constants";

		public override string Suggestion => "Expose public constants as public static readonly instead in order to avoid that they get compiled into a calling assembly.";

		public override CodeQuality Quality => CodeQuality.Broken;

		public override QualityAttribute QualityAttribute => QualityAttribute.Modifiability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Project;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var syntax = (FieldDeclarationSyntax)node;
			if (syntax.Modifiers.Any(kind: SyntaxKind.PublicKeyword) && syntax.Modifiers.Any(kind: SyntaxKind.ConstKeyword))
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
