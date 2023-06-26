// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuardClauseInNonPublicMethodRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the GuardClauseInNonPublicMethodRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class GuardClauseInNonPublicMethodRule : CodeEvaluationBase
	{
		public override string ID => "AM0016";

		public override SyntaxKind EvaluatedKind => SyntaxKind.SimpleMemberAccessExpression;

		public override string Title => "Guard Clause in Non-Public Method.";

		public override string Suggestion => "Remove Guard clause and verify internal state by other means.";

		public override CodeQuality Quality => CodeQuality.NeedsCleanup;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var memberAccess = (MemberAccessExpressionSyntax)node;
			if (memberAccess.Expression.IsKind(kind: SyntaxKind.IdentifierName)
				&& ((IdentifierNameSyntax)memberAccess.Expression).Identifier.ValueText == "Guard")
			{
				var methodParent = FindMethodParent(node: node) as MethodDeclarationSyntax;
				if (methodParent != null && !methodParent.Modifiers.Any(kind: SyntaxKind.PublicKeyword))
				{
					return new EvaluationResult
							   {
								   Snippet = methodParent.ToFullString()
							   };
				}
			}

			return null;
		}
	}
}
