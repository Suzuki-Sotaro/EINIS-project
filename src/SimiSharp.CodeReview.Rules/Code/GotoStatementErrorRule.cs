// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GotoStatementErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the GotoStatementErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class GotoStatementErrorRule : CodeEvaluationBase
	{
		public override string ID => "AM0014";

		public override SyntaxKind EvaluatedKind => SyntaxKind.GotoStatement;

		public override string Title => "Goto Statement";

		public override string Suggestion => "Refactor to use method calls.";

		public override CodeQuality Quality => CodeQuality.Broken;

		public override QualityAttribute QualityAttribute => QualityAttribute.Conformance;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			return new EvaluationResult
					   {
						   Snippet = node.ToFullString()
					   };
		}
	}
}
