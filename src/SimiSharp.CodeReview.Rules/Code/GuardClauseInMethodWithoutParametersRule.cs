// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuardClauseInMethodWithoutParametersRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the GuardClauseInMethodWithoutParametersRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class GuardClauseInMethodWithoutParametersRule : CodeEvaluationBase
	{
		public override string ID => "AM0015";

		public override SyntaxKind EvaluatedKind => SyntaxKind.SimpleMemberAccessExpression;

		public override string Title => "Guard Clause in Method Without Parameters";

		public override string Suggestion => "Remove guard clause.";

		public override CodeQuality Quality => CodeQuality.Broken;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var memberAccess = (MemberAccessExpressionSyntax)node;
			if (memberAccess.Expression.IsKind(kind: SyntaxKind.IdentifierName)
				&& ((IdentifierNameSyntax)memberAccess.Expression).Identifier.ValueText == "Guard")
			{
				var methodParent = FindMethodParent(node: node) as MethodDeclarationSyntax;
				if (methodParent != null && (methodParent.ParameterList == null || methodParent.ParameterList.Parameters.Count == 0))
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
