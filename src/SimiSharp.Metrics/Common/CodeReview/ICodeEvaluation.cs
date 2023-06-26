// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICodeEvaluation.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ICodeEvaluation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

namespace SimiSharp.CodeAnalysis.Common.CodeReview
{
	public interface ICodeEvaluation : ISyntaxEvaluation
	{
		EvaluationResult Evaluate(SyntaxNode node);
	}
}
