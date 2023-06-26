// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMustBeDeclaredInNamespaceRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeMustBeDeclaredInNamespaceRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class TypeMustBeDeclaredInNamespaceRule : CodeEvaluationBase
	{
		public override string ID => "AM0048";

		public override string Suggestion => "Move type declaration inside namespace.";

		public override CodeQuality Quality => CodeQuality.NeedsCleanup;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality;

		public override ImpactLevel ImpactLevel => ImpactLevel.Type;

		public override SyntaxKind EvaluatedKind => SyntaxKind.ClassDeclaration;

		public override string Title => "Declare Types Inside Namespace.";

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var ns = FindNamespaceParent(node: node);
			if (ns == null)
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