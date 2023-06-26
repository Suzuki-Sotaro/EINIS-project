// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoNotDestroyStackTraceRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DoNotDestroyStackTraceRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class DoNotDestroyStackTraceRule : CodeEvaluationBase
	{
		public override string ID => "AM0006";

		public override SyntaxKind EvaluatedKind => SyntaxKind.CatchClause;

		public override string Title => "Stack Trace Destroyed";

		public override string Suggestion => "Use only 'throw' to rethrow the original stack trace.";

		public override CodeQuality Quality => CodeQuality.NeedsCleanup;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var catchClause = (CatchClauseSyntax)node;
			var catchesException = catchClause
				.DescendantNodesAndSelf()
				.OfType<CatchDeclarationSyntax>()
				.SelectMany(selector: x => x.DescendantNodes())
				.OfType<IdentifierNameSyntax>()
				.Any(predicate: x => x.Identifier.ValueText == "Exception");
			var throwsSomething = catchClause
				.DescendantNodes()
				.OfType<ThrowStatementSyntax>()
				.Any(predicate: x => x.Expression != null);
			if (catchesException && throwsSomething)
			{
				var result = new EvaluationResult
							 {
								 Snippet = catchClause.ToFullString()
							 };
				return result;
			}

			return null;
		}
	}
}