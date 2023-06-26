// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorResolvesContainerRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceLocatorResolvesContainerRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class ServiceLocatorResolvesContainerRule : CodeEvaluationBase
	{
		public override string ID => "AM0042";

		public override SyntaxKind EvaluatedKind => SyntaxKind.SimpleMemberAccessExpression;

		public override string Title => "ServiceLocator Resolves Container.";

		public override string Suggestion => "A ServiceLocator should never resolve its own DI container. Refactor to pass dependencies explicitly.";

		public override CodeQuality Quality => CodeQuality.Broken;

		public override QualityAttribute QualityAttribute => QualityAttribute.Testability | QualityAttribute.Maintainability | QualityAttribute.Modifiability | QualityAttribute.Security;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var memberAccess = (MemberAccessExpressionSyntax)node;
			if (memberAccess.Expression.IsKind(kind: SyntaxKind.SimpleMemberAccessExpression)
				&& ((MemberAccessExpressionSyntax)memberAccess.Expression).Expression.IsKind(kind: SyntaxKind.IdentifierName)
				&& ((IdentifierNameSyntax)((MemberAccessExpressionSyntax)memberAccess.Expression).Expression).Identifier.ValueText == "ServiceLocator"
				&& memberAccess.Name is GenericNameSyntax
				&& memberAccess.Name.Identifier.ValueText == "Resolve"
				&& ((GenericNameSyntax)memberAccess.Name).TypeArgumentList.Arguments.Any(predicate: a => a is SimpleNameSyntax && ((SimpleNameSyntax)a).Identifier.ValueText.Contains(value: "UnityContainer")))
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
