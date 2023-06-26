// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyWhileErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EmptyWhileErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class EmptyWhileErrorRule : CodeEvaluationBase
	{
		public override string ID => "AM0010";

		public override SyntaxKind EvaluatedKind => SyntaxKind.WhileStatement;

		public override string Title => "Empty While Statement";

		public override string Suggestion => "Use a wait handle to synchronize asynchronous flows, or let the thread sleep.";

		public override CodeQuality Quality => CodeQuality.NeedsCleanup;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality | QualityAttribute.Testability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var whileStatement = (WhileStatementSyntax)node;

			var sleepLoopFound = whileStatement.DescendantNodes()
											   .OfType<BlockSyntax>()
											   .Any(predicate: s => !s.ChildNodes().Any());

			if (sleepLoopFound)
			{
				var snippet = FindMethodParent(node: node).ToFullString();

				return new EvaluationResult
					   {
						   Snippet = snippet
					   };
			}

			return null;
		}
	}
}
