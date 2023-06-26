// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HalsteadAnalyzer.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the HalsteadAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.Metrics;

namespace SimiSharp.CodeAnalysis.Metrics
{
	internal sealed class HalsteadAnalyzer : SyntaxWalker
	{
		private IHalsteadMetrics _metrics = new HalsteadMetrics(numOperands: 0, numOperators: 0, numUniqueOperands: 0, numUniqueOperators: 0);

		public HalsteadAnalyzer()
			: base(depth: SyntaxWalkerDepth.Node)
		{
		}

		public IHalsteadMetrics Calculate(SyntaxNode syntax)
		{
			if (syntax != null)
			{
				Visit(node: syntax);
				return _metrics;
			}

			return _metrics;
		}

		/// <summary>
		/// Called when the walker visits a node.  This method may be overridden if subclasses want to handle the node.  Overrides should call back into this base method if they want the children of this node to be visited.
		/// </summary>
		/// <param name="node">The current node that the walker is visiting.</param>
		public override void Visit(SyntaxNode node)
		{
			var blockSyntax = node as BlockSyntax;
			if (blockSyntax != null)
			{
				VisitBlock(node: blockSyntax);
			}

			base.Visit(node: node);
		}

		public void VisitBlock(BlockSyntax node)
		{
			var tokens = node.DescendantTokens().ToList();
			var dictionary = ParseTokens(tokens: tokens, filter: Operands.All);
			var dictionary2 = ParseTokens(tokens: tokens, filter: Operators.All);
			var metrics = new HalsteadMetrics(
				numOperands: dictionary.Values.Sum(selector: x => x.Count), 
				numUniqueOperands: dictionary.Values.SelectMany(selector: x => x).Distinct().Count(), 
				numOperators: dictionary2.Values.Sum(selector: x => x.Count), 
				numUniqueOperators: dictionary2.Values.SelectMany(selector: x => x).Distinct().Count());
			_metrics = metrics;
		}

		private static IDictionary<SyntaxKind, IList<string>> ParseTokens(IEnumerable<SyntaxToken> tokens, IEnumerable<SyntaxKind> filter)
		{
			IDictionary<SyntaxKind, IList<string>> dictionary = new Dictionary<SyntaxKind, IList<string>>();
			foreach (var token in tokens)
			{
				var kind = token.Kind();
				if (filter.Any(predicate: x => x == kind))
				{
					IList<string> list;
					var valueText = token.ValueText;
					if (!dictionary.TryGetValue(key: kind, value: out list))
					{
						dictionary[key: kind] = new List<string>();
						list = dictionary[key: kind];
					}

					list.Add(item: valueText);
				}
			}

			return dictionary;
		}
	}
}
