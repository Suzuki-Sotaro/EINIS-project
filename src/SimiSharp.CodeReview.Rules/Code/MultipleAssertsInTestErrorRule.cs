// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleAssertsInTestErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MultipleAssertsInTestErrorRule type.
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
	internal class MultipleAssertsInTestErrorRule : CodeEvaluationBase
	{
		public override string ID => "AM0028";

		public override SyntaxKind EvaluatedKind => SyntaxKind.MethodDeclaration;

		public override string Title => "Multiple Asserts in Test";

		public override string Suggestion => "Refactor tests to only have a single assert.";

		public override CodeQuality Quality => CodeQuality.Broken;

		public override QualityAttribute QualityAttribute => QualityAttribute.Testability | QualityAttribute.CodeQuality;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var methodDeclaration = (MethodDeclarationSyntax)node;

			if (methodDeclaration.AttributeLists.Any(predicate: l => l.Attributes.Any(predicate: a => a.Name is SimpleNameSyntax && ((SimpleNameSyntax)a.Name).Identifier.ValueText == "TestMethod")))
			{
				var accessExpressionSyntaxes = methodDeclaration.DescendantNodes()
					.OfType<MemberAccessExpressionSyntax>()
					.AsArray();

				var assertsFound = accessExpressionSyntaxes
													.Select(selector: x => x.Expression as SimpleNameSyntax)
													.Where(predicate: x => x != null)
													.Count(predicate: x => x.Identifier.ValueText == "Assert" || x.Identifier.ValueText == "ExceptionAssert");
				var mockVerifyFound = accessExpressionSyntaxes
													   .Count(predicate: x => x.Name.Identifier.ValueText == "Verify" || x.Name.Identifier.ValueText == "VerifySet" || x.Name.Identifier.ValueText == "VerifyGet");
				var expectedExceptions =
					methodDeclaration.AttributeLists.Count(
						predicate: l =>
						l.Attributes.Any(predicate: a => a.Name is SimpleNameSyntax && ((SimpleNameSyntax)a.Name).Identifier.ValueText == "ExpectedException"));

				var total = assertsFound + mockVerifyFound + expectedExceptions;
				return total != 1
						   ? new EvaluationResult
								 {
									 Snippet = node.ToFullString(),
									 ErrorCount = total
								 }
						   : null;
			}

			return null;
		}
	}
}
