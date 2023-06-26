// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnreadValueRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnreadValueRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Semantic
{
	internal abstract class UnreadValueRule : SemanticEvaluationBase
	{
		public override string ID => "AM0060";

		public override CodeQuality Quality => CodeQuality.NeedsReview;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality | QualityAttribute.Maintainability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Type;

		protected abstract IEnumerable<ISymbol> GetSymbols(SyntaxNode node, SemanticModel semanticModel);

		protected override async Task<EvaluationResult> EvaluateImpl(SyntaxNode node, SemanticModel semanticModel, Solution solution)
		{
			var referenceTasks = GetSymbols(node: node, semanticModel: semanticModel)
				.Select(selector: solution.FindReferences);
			var references = (await Task.WhenAll(tasks: referenceTasks).ConfigureAwait(continueOnCapturedContext: false))
				.SelectMany(selector: x => x.Locations)
				.Select(selector: x => x.Location.SourceTree.GetRoot().FindToken(position: x.Location.SourceSpan.Start))
				.Select(selector: x => x.Parent)
				.Where(predicate: x => x != null)
				.Select(selector: x => new { Value = x, Parent = x.Parent })
				.Where(predicate: x => IsNotAssignment(syntax: x.Parent, value: x.Value))
				.AsArray();

			if (!references.Any())
			{
				return new EvaluationResult
					   {
						   Snippet = node.ToFullString()
					   };
			}

			return null;
		}

		private static bool IsNotAssignment(SyntaxNode syntax, SyntaxNode value)
		{
			if (syntax.IsKind(kind: SyntaxKind.SimpleAssignmentExpression))
			{
				var binaryExpression = (AssignmentExpressionSyntax)syntax;
				return binaryExpression.Right == value;
			}

			var expression = syntax as EqualsValueClauseSyntax;
			if (expression != null)
			{
				return expression.Value == value;
			}

			return true;
		}
	}
}