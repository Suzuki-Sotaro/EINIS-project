// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISymbolEvaluation.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ISymbolEvaluation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

namespace SimiSharp.CodeAnalysis.Common.CodeReview
{
	public interface ISymbolEvaluation : IEvaluation
	{
		SymbolKind EvaluatedKind { get; }

		EvaluationResult Evaluate(ISymbol symbol, SemanticModel semanticModel);
	}
}