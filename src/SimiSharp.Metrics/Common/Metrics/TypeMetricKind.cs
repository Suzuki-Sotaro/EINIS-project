// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMetricKind.cs" company="Reimers.dk">
//   Copyright � 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeMetricKind type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace SimiSharp.CodeAnalysis.Common.Metrics
{
	/// <summary>
	/// Defines the kind of <see cref="Type"/>.
	/// </summary>
	public enum TypeMetricKind
	{
		/// <summary>
		/// The type kind cannot be determined.
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// The type is a class.
		/// </summary>
		Class = 1,

		/// <summary>
		/// The type is a delegate.
		/// </summary>
		Delegate = 2,

		/// <summary>
		/// The type is an interface.
		/// </summary>
		Interface = 3,

		/// <summary>
		/// The type is a struct.
		/// </summary>
		Struct = 4,
	}
}
