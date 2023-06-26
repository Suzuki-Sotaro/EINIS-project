// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComparableComparer.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ComparableComparer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace SimiSharp.CodeAnalysis.Metrics
{
	internal class ComparableComparer<T> : IEqualityComparer<T> where T : IComparable<T>
	{
		/// <summary>
		/// Determines whether the specified objects are equal.
		/// </summary>
		/// <returns>
		/// True if the specified objects are equal; otherwise, false.
		/// </returns>
		/// <param name="x">The first object of type <paramref name="x"/> to compare.</param><param name="y">The second object of type <paramref name="y"/> to compare.</param>
		public bool Equals(T x, T y)
		{
			return ReferenceEquals(objA: x, objB: null)
				? ReferenceEquals(objA: y, objB: null)
				: x.CompareTo(other: y) == 0;
		}

		/// <summary>
		/// Returns a hash code for the specified object.
		/// </summary>
		/// <returns>
		/// A hash code for the specified object.
		/// </returns>
		/// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
		public int GetHashCode(T obj)
		{
			return ReferenceEquals(objA: obj, objB: null)
				? 0
				: obj.GetHashCode();
		}
	}
}
