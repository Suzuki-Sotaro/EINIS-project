// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnusedCodeRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnusedCodeRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SimiSharp.CodeAnalysis;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Semantic
{
	internal abstract class UnusedCodeRule : SemanticEvaluationBase
	{
		public override string Title => "Unused " + EvaluatedKind.ToString().ToTitleCase();

		public override string Suggestion => "Remove unused code.";

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		public override CodeQuality Quality => CodeQuality.NeedsCleanup;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality;

		protected override async Task<EvaluationResult> EvaluateImpl(SyntaxNode node, SemanticModel semanticModel, Solution solution)
		{
			var symbol = semanticModel.GetDeclaredSymbol(declaration: node);
			var callers = await solution.FindReferences(symbol: symbol).ConfigureAwait(continueOnCapturedContext: false);

			if (!callers.Locations.Any(predicate: x => IsNotAssignment(location: x.Location)))
			{
				return new EvaluationResult
					   {
						   Snippet = node.ToFullString()
					   };
			}

			return null;
		}

		private bool IsNotAssignment(Location location)
		{
			if (!location.IsInSource)
			{
				return false;
			}

			var token = location.SourceTree.GetRoot().FindToken(position: location.SourceSpan.Start);
			var assignmentSyntax = GetAssignmentSyntax(node: token.Parent);
			if (assignmentSyntax == null)
			{
				return true;
			}

			return false;
		}

		private SyntaxNode GetAssignmentSyntax(SyntaxNode node)
		{
			if (node == null)
			{
				return null;
			}

			if (node.IsKind(kind: SyntaxKind.EqualsExpression))
			{
				return node;
			}

			return GetAssignmentSyntax(node: node.Parent);
		}
	}
}
