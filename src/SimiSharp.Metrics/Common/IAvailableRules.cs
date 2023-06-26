// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAvailableRules.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IAvailableRules type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeAnalysis.Common
{
	/// <summary>
	/// Defines the interface for accessing temporally available items.
	/// </summary>
	public interface IAvailableRules : IEnumerable<IEvaluation>, INotifyCollectionChanged, IDisposable
	{
		/// <summary>
		/// Gets an <see cref="IEnumerable{T}"/> of available items.
		/// </summary>
		IEnumerable<IAvailability> Availabilities { get; }
	}
}