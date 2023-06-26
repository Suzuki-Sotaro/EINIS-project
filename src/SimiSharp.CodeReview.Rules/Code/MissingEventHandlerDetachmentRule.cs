// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MissingEventHandlerDetachmentRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MissingEventHandlerDetachmentRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class MissingEventHandlerDetachmentRule : CodeEvaluationBase
	{
		public override string ID => "AM0027";

		public override SyntaxKind EvaluatedKind => SyntaxKind.ClassDeclaration;

		public override string Title => "Event Handler not Detached";

		public override string Suggestion => "Unassign all event handlers.";

		public override CodeQuality Quality => CodeQuality.Broken;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality;

		public override ImpactLevel ImpactLevel => ImpactLevel.Type;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var declarationSyntax = (TypeDeclarationSyntax)node;
			var addAssignments = declarationSyntax
				.DescendantNodes()
				.Where(predicate: x => x.Kind() == SyntaxKind.AddAssignmentExpression)
				.Cast<AssignmentExpressionSyntax>()
				.AsArray();
			var subtractAssignments = declarationSyntax.DescendantNodes()
				.Where(predicate: x => x.Kind() == SyntaxKind.SubtractAssignmentExpression)
				.Cast<AssignmentExpressionSyntax>()
				.AsArray();

			var assignmentExpressionSyntaxes = addAssignments.DistinctBy(func: x => x.ToFullString()).AsArray();

			if (assignmentExpressionSyntaxes.Count() != subtractAssignments.DistinctBy(func: x => x.ToFullString()).Count())
			{
				var unmatched = assignmentExpressionSyntaxes.Where(predicate: x => !MatchingAssignmentExpressionExists(addAssignment: x, subtractAssignments: subtractAssignments));
				var snippet = string.Join(separator: Environment.NewLine, values: unmatched.Select(selector: x => x.ToFullString()));

				return new EvaluationResult
						   {
							   Snippet = snippet
						   };
			}

			return null;
		}

		private bool MatchingAssignmentExpressionExists(
			AssignmentExpressionSyntax addAssignment,
			IEnumerable<AssignmentExpressionSyntax> subtractAssignments)
		{
			var changedAssignment = SyntaxFactory.AssignmentExpression(
				kind: SyntaxKind.SubtractAssignmentExpression,
				left: addAssignment.Left,
				right: addAssignment.Right);

			return subtractAssignments.Any(predicate: x => x.IsEquivalentTo(other: changedAssignment));
		}
	}
}