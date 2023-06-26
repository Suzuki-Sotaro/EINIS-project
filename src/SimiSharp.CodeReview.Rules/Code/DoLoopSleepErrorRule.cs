// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoLoopSleepErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DoLoopSleepErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class DoLoopSleepErrorRule : CodeEvaluationBase
	{
		public override string ID => "AM0005";

		public override SyntaxKind EvaluatedKind => SyntaxKind.DoStatement;

		public override string Title => "Do Statement Sleep Loop";

		public override string Suggestion => "Use a wait handle to synchronize timing issues.";

		public override CodeQuality Quality => CodeQuality.NeedsCleanup;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality | QualityAttribute.Testability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var statement = (DoStatementSyntax)node;

			var sleepLoopFound = statement.DescendantNodes()
											.OfType<MemberAccessExpressionSyntax>()
											.Select(selector: x => new Tuple<SimpleNameSyntax, SimpleNameSyntax>(item1: x.Expression as SimpleNameSyntax, item2: x.Name))
											.Where(predicate: x => x.Item1 != null)
											.Any(predicate: x => x.Item1.Identifier.ValueText == "Thread" && x.Item2.Identifier.ValueText == "Sleep");

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
