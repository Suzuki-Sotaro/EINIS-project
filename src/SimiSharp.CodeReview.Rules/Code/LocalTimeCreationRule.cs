// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalTimeCreationRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LocalTimeCreationRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class LocalTimeCreationRule : CodeEvaluationBase
	{
		public override string ID => "AM0023";

		public override SyntaxKind EvaluatedKind => SyntaxKind.SimpleMemberAccessExpression;

		public override string Title => "Local Time Creation";

		public override string Suggestion => "Replace with call to DateTime.UtcNow";

		public override CodeQuality Quality => CodeQuality.NeedsReview;

		public override QualityAttribute QualityAttribute => QualityAttribute.Conformance;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var memberAccess = (MemberAccessExpressionSyntax)node;
			if (memberAccess.Expression.IsKind(kind: SyntaxKind.IdentifierName)
				&& ((IdentifierNameSyntax)memberAccess.Expression).Identifier.ValueText == "DateTime"
				&& memberAccess.Name.Identifier.ValueText == "Now")
			{
				var methodParent = FindMethodParent(node: node);
				var snippet = methodParent == null
								  ? node.ToFullString()
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
