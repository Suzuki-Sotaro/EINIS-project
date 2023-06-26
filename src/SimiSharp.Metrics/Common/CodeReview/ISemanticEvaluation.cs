// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISemanticEvaluation.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ISemanticEvaluation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace SimiSharp.CodeAnalysis.Common.CodeReview
{
	public interface ISemanticEvaluation : ISyntaxEvaluation
	{
		Task<EvaluationResult> Evaluate(SyntaxNode node, SemanticModel semanticModel, Solution solution);
	}
}