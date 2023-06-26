// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorInvocationRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceLocatorInvocationRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class ServiceLocatorInvocationRule : CodeEvaluationBase
	{
		public override string ID => "AM0041";

		public override SyntaxKind EvaluatedKind => SyntaxKind.SimpleMemberAccessExpression;

		public override string Title => "ServiceLocator Invocation";

		public override string Suggestion => "Consider injecting needed dependencies explicitly.";

		public override CodeQuality Quality => CodeQuality.Broken;

		public override QualityAttribute QualityAttribute => QualityAttribute.Testability | QualityAttribute.Maintainability | QualityAttribute.Modifiability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Type;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var memberAccess = (MemberAccessExpressionSyntax)node;
			if (memberAccess.Expression.IsKind(kind: SyntaxKind.SimpleMemberAccessExpression)
				&& ((MemberAccessExpressionSyntax)memberAccess.Expression).Expression.IsKind(kind: SyntaxKind.IdentifierName)
				&& ((IdentifierNameSyntax)((MemberAccessExpressionSyntax)memberAccess.Expression).Expression).Identifier.ValueText == "ServiceLocator"
				&& memberAccess.Name.Identifier.ValueText == "Resolve")
			{
				var methodParent = FindMethodParent(node: node);
				var snippet = methodParent == null
					? FindClassParent(node: node).ToFullString()
					: methodParent.ToFullString();

				return new EvaluationResult
				{
					Snippet = snippet
				};
			}

			return null;
		}
	}
}
