// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiskLocationDependencyRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DiskLocationDependencyRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class DiskLocationDependencyRule : CodeEvaluationBase
	{
		private static readonly Regex DiskLocationRegex = new Regex(pattern: @"\w:\\", options: RegexOptions.Compiled);

		public override string ID => "AM0004";

		public override SyntaxKind EvaluatedKind => SyntaxKind.SimpleAssignmentExpression;

		public override string Title => "Disk Location Dependency";

		public override string Suggestion => "Replace the dependency on a specific disk location with an abstraction.";

		public override CodeQuality Quality => CodeQuality.NeedsRefactoring;

		public override QualityAttribute QualityAttribute => QualityAttribute.Modifiability | QualityAttribute.Testability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Project;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var assignExpression = (AssignmentExpressionSyntax)node;
			var right = assignExpression.Right as LiteralExpressionSyntax;
			if (right != null)
			{
				var assignmentToken = right.Token.ToFullString();
				if (DiskLocationRegex.IsMatch(input: assignmentToken))
				{
					return new EvaluationResult
							   {
								   Snippet = FindMethodParent(node: node).ToFullString()
							   };
				}
			}

			return null;
		}
	}
}
