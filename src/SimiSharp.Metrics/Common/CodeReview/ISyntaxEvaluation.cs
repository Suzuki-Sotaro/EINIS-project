// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyntaxEvaluation.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ISyntaxEvaluation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp;

namespace SimiSharp.CodeAnalysis.Common.CodeReview
{
	public interface ISyntaxEvaluation : IEvaluation
	{
		SyntaxKind EvaluatedKind { get; }
	}
}