// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SemanticEvaluationBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SemanticEvaluationBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using SimiSharp.CodeAnalysis.Common.CodeReview;
using SimiSharp.CodeReview.Rules.Code;

namespace SimiSharp.CodeReview.Rules.Semantic
{
	internal abstract class SemanticEvaluationBase : EvaluationBase, ISemanticEvaluation
	{
		public async Task<EvaluationResult> Evaluate(SyntaxNode node, SemanticModel semanticModel, Solution solution)
		{
			if (semanticModel == null || solution == null)
			{
				return null;
			}

			var result = await EvaluateImpl(node: node, semanticModel: semanticModel, solution: solution).ConfigureAwait(continueOnCapturedContext: false);
			if (result == null)
			{
				return null;
			}

			var sourceTree = node.GetLocation().SourceTree;
			var filePath = sourceTree.FilePath;
			var typeDefinition = GetNodeType(node: node);
			var unitNamespace = GetNamespace(node: node);
			if (result.ErrorCount == 0)
			{
				result.ErrorCount = 1;
			}

			if (result.LinesOfCodeAffected <= 0)
			{
				result.LinesOfCodeAffected = GetLinesOfCode(node: node);
			}

			result.Namespace = unitNamespace;
			result.TypeKind = typeDefinition.Item1;
			result.TypeName = typeDefinition.Item2;
			result.Title = Title;
			result.Suggestion = Suggestion;
			result.Quality = Quality;
			result.QualityAttribute = QualityAttribute;
			result.ImpactLevel = ImpactLevel;
			result.FilePath = filePath;

			return result;
		}

		protected abstract Task<EvaluationResult> EvaluateImpl(SyntaxNode node, SemanticModel semanticModel, Solution solution);
	}
}