// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeCoupling.cs" company="Reimers.dk">
//   Copyright � 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeCoupling type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace SimiSharp.CodeAnalysis.Common.Metrics
{
	internal class TypeCoupling : TypeDefinition, ITypeCoupling
	{
		public TypeCoupling(string typeName, string namespaceName, string assemblyName, IEnumerable<string> usedMethods, IEnumerable<string> usedProperties, IEnumerable<string> useEvents)
			: base(typeName: typeName, namespaceName: namespaceName, assemblyName: assemblyName)
		{
			UsedMethods = usedMethods.Distinct().AsArray();
			UsedProperties = usedProperties.Distinct().AsArray();
			UsedEvents = useEvents.Distinct().AsArray();
		}

		public IEnumerable<string> UsedMethods { get; }

		public IEnumerable<string> UsedProperties { get; }

		public IEnumerable<string> UsedEvents { get; }

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <returns>
		/// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public int CompareTo(ITypeCoupling other)
		{
			return other == null ? -1 : string.Compare(strA: ToString(), strB: other.ToString(), comparisonType: StringComparison.OrdinalIgnoreCase);
		}
	}
}
