// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEvaluation.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IEvaluation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimiSharp.CodeAnalysis.Common.CodeReview
{
	public interface IEvaluation
	{
		string ID { get; }

		string Title { get; }

		string Suggestion { get; }

		CodeQuality Quality { get; }

		QualityAttribute QualityAttribute { get; }

		ImpactLevel ImpactLevel { get; }
	}
}