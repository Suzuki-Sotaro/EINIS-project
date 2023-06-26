// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SemanticModelMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SemanticModelMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

namespace SimiSharp.CodeAnalysis.Metrics
{
	public abstract class SemanticModelMetricsCalculator
	{
	    protected SemanticModelMetricsCalculator(SemanticModel semanticModel)
		{
			Model = semanticModel;
		}

		protected SemanticModel Model { get; }

	    protected SyntaxNode Root => Model.SyntaxTree.GetRoot();
	}
}
