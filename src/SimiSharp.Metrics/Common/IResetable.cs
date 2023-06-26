// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResetable.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IResetable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimiSharp.CodeAnalysis.Common
{
	/// <summary>
	/// Defines the interface for items which can be reset to their original state.
	/// </summary>
	public interface IResetable
	{
		/// <summary>
		/// Resets the instance to the original state.
		/// </summary>
		void Reset();
	}
}
