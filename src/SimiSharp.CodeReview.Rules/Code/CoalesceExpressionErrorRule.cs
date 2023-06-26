// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoalesceExpressionErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CoalesceExpressionErrorRule type.
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
	internal class CoalesceExpressionErrorRule : CodeEvaluationBase
	{
		public override string ID => "AM0001";

		public override SyntaxKind EvaluatedKind => SyntaxKind.MethodDeclaration;

		public override string Title => "Coalesce Expression";

		public override string Suggestion => "Use an explicit if statement to assign a value if it is null.";

		public override CodeQuality Quality => CodeQuality.NeedsReview;

		public override QualityAttribute QualityAttribute => QualityAttribute.Conformance;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var methodDeclaration = (MethodDeclarationSyntax)node;
			var conditionalExpressions = methodDeclaration.DescendantNodes()
														  .Where(predicate: n => n.IsKind(kind: SyntaxKind.CoalesceExpression))
														  .AsArray();
			if (conditionalExpressions.Any())
			{
				return new EvaluationResult
						   {
							   Snippet = string.Join(separator: "\r\n", values: conditionalExpressions.Select(selector: n => n.ToFullString())), 
							   ErrorCount = conditionalExpressions.Length
						   };
			}

			return null;
		}
	}
}
