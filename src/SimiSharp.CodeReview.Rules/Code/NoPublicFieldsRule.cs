// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoPublicFieldsRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NoPublicFieldsRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class NoPublicFieldsRule : CodeEvaluationBase
	{
		public override string ID => "AM0033";

		public override SyntaxKind EvaluatedKind => SyntaxKind.FieldDeclaration;

		public override string Title => "No Public Field";

		public override string Suggestion => "Encapsulate all public fields in properties, or internalize them.";

		public override CodeQuality Quality => CodeQuality.Broken;

		public override QualityAttribute QualityAttribute => QualityAttribute.Modifiability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Type;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var syntax = (FieldDeclarationSyntax)node;
			if (syntax.Modifiers.Any(kind: SyntaxKind.PublicKeyword))
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
