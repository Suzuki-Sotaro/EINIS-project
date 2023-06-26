// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VarDeclarationForNewVariableErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the VarDeclarationForNewVariableErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class VarDeclarationForNewVariableErrorRule : CodeEvaluationBase
	{
		public override string ID => "AM0050";

		public override SyntaxKind EvaluatedKind => SyntaxKind.VariableDeclaration;

		public override string Title => "Var Keyword Used in Variable Declaration";

		public override string Suggestion => "Consider using an explicit type for variable.";

		public override CodeQuality Quality => CodeQuality.NeedsReview;

		public override QualityAttribute QualityAttribute => QualityAttribute.Conformance;

		public override ImpactLevel ImpactLevel => ImpactLevel.Line;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var declaration = (VariableDeclarationSyntax)node;
			if (declaration.Type.IsVar && !declaration.Variables.All(predicate: x => x.Initializer.Value is ObjectCreationExpressionSyntax))
			{
				return new EvaluationResult
						   {
							   ErrorCount = declaration.Variables.Count, 
							   Snippet = declaration.ToFullString()
						   };
			}

			return null;
		}
	}
}
