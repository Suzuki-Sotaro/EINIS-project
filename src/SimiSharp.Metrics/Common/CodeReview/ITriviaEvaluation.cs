// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITriviaEvaluation.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ITriviaEvaluation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

namespace SimiSharp.CodeAnalysis.Common.CodeReview
{
	public interface ITriviaEvaluation : ISyntaxEvaluation
	{
		EvaluationResult Evaluate(SyntaxTrivia trivia);
	}
}