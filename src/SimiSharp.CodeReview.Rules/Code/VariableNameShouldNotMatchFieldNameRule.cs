// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VariableNameShouldNotMatchFieldNameRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the VariableNameShouldNotMatchFieldNameRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class VariableNameShouldNotMatchFieldNameRule : CodeEvaluationBase
	{
		public override string ID => "AM0051";

		public override SyntaxKind EvaluatedKind => SyntaxKind.SimpleAssignmentExpression;

		public override string Title => "Variable Name Should Not Match Field Name";

		public override string Suggestion => "Rename variable to avoid confusion with assigned field.";

		public override CodeQuality Quality => CodeQuality.NeedsReview;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality | QualityAttribute.Maintainability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var assignment = (AssignmentExpressionSyntax)node;
			var left = assignment.Left as MemberAccessExpressionSyntax;
			if (left == null || !left.Expression.IsKind(kind: SyntaxKind.ThisExpression))
			{
				return null;
			}

			var variable = left.Name as IdentifierNameSyntax;
			var right = assignment.Right as IdentifierNameSyntax;
			if (right == null || variable == null)
			{
				return null;
			}

			if (variable.Identifier.ValueText == right.Identifier.ValueText)
			{
				return new EvaluationResult
					   {
						   Snippet = assignment.ToFullString()
					   };
			}

			return null;
		}
	}
}