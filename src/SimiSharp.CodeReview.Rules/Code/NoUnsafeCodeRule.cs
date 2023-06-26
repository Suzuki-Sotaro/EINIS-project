// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoUnsafeCodeRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NoUnsafeCodeRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class NoUnsafeCodeRule : CodeEvaluationBase
	{
		public override string ID => "AM0034";

		public override SyntaxKind EvaluatedKind => SyntaxKind.UnsafeStatement;

		public override string Title => "Unsafe Statement Detected";

		public override string Suggestion => "Avoid unsafe code.";

		public override CodeQuality Quality => CodeQuality.NeedsReEngineering;

		public override QualityAttribute QualityAttribute => QualityAttribute.Conformance | QualityAttribute.Security;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var snippet = node.ToFullString();
			return new EvaluationResult
				   {
					   Snippet = snippet
				   };
		}
	}
}