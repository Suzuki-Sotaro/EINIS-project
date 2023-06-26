// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicVariableRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DynamicVariableRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class DynamicVariableRule : CodeEvaluationBase
	{
		public override string ID => "AM0007";

		public override SyntaxKind EvaluatedKind => SyntaxKind.VariableDeclaration;

		public override string Title => "Dynamic Variable";

		public override string Suggestion => "Consider using a typed variable.";

		public override CodeQuality Quality => CodeQuality.Broken;

		public override QualityAttribute QualityAttribute => QualityAttribute.Conformance;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var variableDeclaration = (VariableDeclarationSyntax)node;
			if (variableDeclaration.Type.GetText().ToString().Trim() == "dynamic")
			{
				var methodParent = FindMethodParent(node: node);
				var snippet = methodParent.ToFullString();

				return new EvaluationResult
						   {
							   Snippet = snippet
						   };
			}

			return null;
		}
	}
}
