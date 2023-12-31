// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyTestRule.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EmptyTestRule type.
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
	internal class EmptyTestRule : CodeEvaluationBase
	{
		public override string ID => "AM0009";

		public override SyntaxKind EvaluatedKind => SyntaxKind.MethodDeclaration;

		public override string Title => "No Assertion in Test";

		public override string Suggestion => "Add an assertion to the test.";

		public override CodeQuality Quality => CodeQuality.NeedsReview;

		public override QualityAttribute QualityAttribute => QualityAttribute.Testability | QualityAttribute.CodeQuality;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var methodParent = (MethodDeclarationSyntax)node;

			if (methodParent != null
				&& methodParent.AttributeLists.Any(
					predicate: l => l.Attributes.Any(predicate: a => a.Name is SimpleNameSyntax
					                                                 && ((SimpleNameSyntax)a.Name).Identifier.ValueText.IsKnownTestAttribute())))
			{
				if (methodParent.Body == null
					|| !methodParent.Body.ChildNodes().Any())
				{
					return new EvaluationResult
						   {
							   Snippet = (FindClassParent(node: node) ?? node).ToFullString()
						   };
				}
			}

			return null;
		}
	}
}