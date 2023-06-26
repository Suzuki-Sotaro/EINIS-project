// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoProtectedFieldsInPublicClassesRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NoProtectedFieldsInPublicClassesRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class NoProtectedFieldsInPublicClassesRule : CodeEvaluationBase
	{
		public override string ID => "AM0031";

		public override SyntaxKind EvaluatedKind => SyntaxKind.FieldDeclaration;

		public override string Title => "No Protected Fields";

		public override string Suggestion => "Encapsulate all public fields in properties, or internalize them.";

		public override CodeQuality Quality => CodeQuality.Broken;

		public override QualityAttribute QualityAttribute => QualityAttribute.Modifiability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Type;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var classParent = FindClassParent(node: node);
			if (classParent != null && classParent.Modifiers.Any(kind: SyntaxKind.PublicKeyword))
			{
				var syntax = (FieldDeclarationSyntax)node;
				if (syntax.Modifiers.Any(kind: SyntaxKind.ProtectedKeyword))
				{
					return new EvaluationResult
							   {
								   Snippet = classParent.ToFullString()
							   };
				}
			}

			return null;
		}
	}
}
