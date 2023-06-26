// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorInvocationInTestRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceLocatorInvocationInTestRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class ServiceLocatorInvocationInTestRule : CodeEvaluationBase
	{
		public override string ID => "AM0040";

		public override SyntaxKind EvaluatedKind => SyntaxKind.SimpleMemberAccessExpression;

		public override string Title => "ServiceLocator Invocation in Test";

		public override string Suggestion => "Replace ServiceLocator with explicit setup using either a concrete instance, mock or fake.";

		public override CodeQuality Quality => CodeQuality.NeedsCleanup;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality | QualityAttribute.Maintainability | QualityAttribute.Modifiability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var memberAccess = (MemberAccessExpressionSyntax)node;
			if (memberAccess.Expression.IsKind(kind: SyntaxKind.SimpleMemberAccessExpression)
				&& ((MemberAccessExpressionSyntax)memberAccess.Expression).Expression.IsKind(kind: SyntaxKind.IdentifierName)
				&& ((IdentifierNameSyntax)((MemberAccessExpressionSyntax)memberAccess.Expression).Expression).Identifier.ValueText == "ServiceLocator"
				&& memberAccess.Name.Identifier.ValueText == "Resolve")
			{
				var methodParent = FindMethodParent(node: node) as MethodDeclarationSyntax;
				if (methodParent != null
					&& methodParent.AttributeLists.Any(predicate: l => l.Attributes.Any(predicate: a => a.Name is SimpleNameSyntax && ((SimpleNameSyntax)a.Name).Identifier.ValueText.IsKnownTestAttribute())))
				{
					var snippet = methodParent.ToFullString();

					return new EvaluationResult
							   {
								   Snippet = snippet, 
							   };
				}
			}

			return null;
		}
	}
}
