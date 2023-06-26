// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionToResolveMethodNameRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ReflectionToResolveMethodNameRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class ReflectionToResolveMethodNameRule : CodeEvaluationBase
	{
		public override string ID => "AM0038";

		public override SyntaxKind EvaluatedKind => SyntaxKind.SimpleMemberAccessExpression;

		public override string Title => "Using Reflection to Resolve Member Name";

		public override string Suggestion => "Consider using a string for the method name for performance and to make it readable after obfuscation.";

		public override CodeQuality Quality => CodeQuality.NeedsCleanup;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var memberAccess = (MemberAccessExpressionSyntax)node;
			if (memberAccess.Expression.IsKind(kind: SyntaxKind.InvocationExpression)
				&& memberAccess.Expression.GetText().ToString().Trim() == "MethodBase.GetCurrentMethod()")
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
