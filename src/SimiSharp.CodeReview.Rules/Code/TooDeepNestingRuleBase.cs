// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooDeepNestingRuleBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TooDeepNestingRuleBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal abstract class TooDeepNestingRuleBase : CodeEvaluationBase
	{
		private readonly int _depth;

		protected TooDeepNestingRuleBase()
			: this(maxDepth: 3)
		{
		}

		protected TooDeepNestingRuleBase(int maxDepth)
		{
			_depth = maxDepth;
		}

		public override string ID => "AM0045";

		public override string Title => "Too Deep " + NestingMember + " Nesting";

		public override string Suggestion => "Reduce nesting to make code more readable.";

		public override CodeQuality Quality => CodeQuality.NeedsReview;

		public override QualityAttribute QualityAttribute => QualityAttribute.Maintainability | QualityAttribute.Testability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected abstract string NestingMember { get; }

		protected abstract BlockSyntax GetBody(SyntaxNode node);

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var body = GetBody(node: node);
			if (body != null && HasDeepNesting(block: body, level: 0))
			{
				return new EvaluationResult
					   {
						   Snippet = node.ToFullString()
					   };
			}

			return null;
		}

		private bool HasDeepNesting(BlockSyntax block, int level)
		{
			if (level >= _depth)
			{
				return true;
			}

			var result = GetBlocks(node: block).Aggregate(seed: false, func: (a, b) => a || HasDeepNesting(block: b, level: level + 1));

			return result;
		}

		private IEnumerable<BlockSyntax> GetBlocks(SyntaxNode node)
		{
			var childBlocks = node.ChildNodes().Where(predicate: x => x.IsKind(kind: SyntaxKind.Block)).Cast<BlockSyntax>();
			var others = node.ChildNodes()
				.Where(predicate: x => !x.IsKind(kind: SyntaxKind.Block))
				.SelectMany(selector: GetBlocks);

			return childBlocks.Concat(second: others);
		}
	}
}