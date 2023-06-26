// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmediateTaskWaitRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ImmediateTaskWaitRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class ImmediateTaskWaitRule : CodeEvaluationBase
	{
		public override string ID => "AM0017";

		public override SyntaxKind EvaluatedKind => SyntaxKind.SimpleMemberAccessExpression;

		public override string Title => "Immediate Task Wait.";

		public override string Suggestion => "Immediately awaiting a Task has same effect as executing code synchonously.";

		public override CodeQuality Quality => CodeQuality.NeedsCleanup;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var memberAccess = (MemberAccessExpressionSyntax)node;
			if (memberAccess.Expression.IsKind(kind: SyntaxKind.IdentifierName)
				&& memberAccess.Name.Identifier.ValueText == "Wait")
			{
				var invokedVariable = memberAccess.Expression as IdentifierNameSyntax;
				if (invokedVariable != null)
				{
					var variableName = invokedVariable.Identifier.ValueText;
					var methodParent = FindMethodParent(node: node);
					var variableAssignment = methodParent == null ? null : FindVariableAssignment(node: methodParent, variableName: variableName);
					if (variableAssignment != null)
					{
						var childNodes = memberAccess.Parent.Parent.Parent.ChildNodes().Select(selector: n => n.WithLeadingTrivia().WithTrailingTrivia().ToString()).AsArray();
						var assignmentIndex = Array.IndexOf(array: childNodes, value: variableAssignment.Parent.WithLeadingTrivia().WithTrailingTrivia() + ";");
						var invocationIndex = Array.IndexOf(array: childNodes, value: memberAccess.Parent.WithLeadingTrivia().WithTrailingTrivia() + ";");
						if (invocationIndex == assignmentIndex + 1)
						{
							var snippet = methodParent.ToFullString();

							return new EvaluationResult
									   {
										   Snippet = snippet
									   };
						}
					}
				}
			}

			return null;
		}

		private SyntaxNode FindVariableAssignment(SyntaxNode node, string variableName)
		{
			return node.DescendantNodes()
					   .Where(predicate: n => n.IsKind(kind: SyntaxKind.SimpleAssignmentExpression))
					   .OfType<AssignmentExpressionSyntax>()
					   .Select(selector: x => x.Left as IdentifierNameSyntax)
					   .Where(predicate: x => x != null).FirstOrDefault(predicate: x => x.Identifier.ValueText == variableName);
		}
	}
}
