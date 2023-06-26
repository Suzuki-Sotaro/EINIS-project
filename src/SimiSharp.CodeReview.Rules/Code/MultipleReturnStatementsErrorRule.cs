// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleReturnStatementsErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MultipleReturnStatementsErrorRule type.
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
	internal class MultipleReturnStatementsErrorRule : CodeEvaluationBase
	{
		public override string ID => "AM0029";

		public override SyntaxKind EvaluatedKind => SyntaxKind.MethodDeclaration;

		public override string Title => "Multiple Return Statements";

		public override string Suggestion => "If your company's coding standards requires only a single exit point, then refactor method to have only single return statement.";

		public override CodeQuality Quality => CodeQuality.NeedsReview;

		public override QualityAttribute QualityAttribute => QualityAttribute.Conformance;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var methodDeclaration = (MethodDeclarationSyntax)node;
			var returnStatements = methodDeclaration.DescendantNodes().Where(predicate: n => n.IsKind(kind: SyntaxKind.ReturnStatement)).AsArray();
			if (returnStatements.Length > 1)
			{
				return new EvaluationResult
						   { 
							   Snippet = node.ToFullString(), 
							   ErrorCount = returnStatements.Length
						   };
			}

			return null;
		}
	}
}
