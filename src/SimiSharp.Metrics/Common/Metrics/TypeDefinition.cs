// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeDefinition.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeDefinition type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace SimiSharp.CodeAnalysis.Common.Metrics
{
	internal class TypeDefinition : ITypeDefinition, IComparable
	{
		private readonly string _fullName;

		public TypeDefinition(string typeName, string namespaceName, string assemblyName)
		{
			TypeName = typeName;
			Namespace = namespaceName;
			Assembly = assemblyName;

			_fullName = $"{namespaceName}.{typeName}, {assemblyName}";
		}

		public string TypeName { get; }

		public string Namespace { get; }

		public string Assembly { get; }

		public static bool operator ==(TypeDefinition c1, TypeDefinition c2)
		{
			return ReferenceEquals(objA: c1, objB: null)
					   ? ReferenceEquals(objA: c2, objB: null)
					   : c1.CompareTo(other: c2) == 0;
		}

		public static bool operator !=(TypeDefinition c1, TypeDefinition c2)
		{
			return ReferenceEquals(objA: c1, objB: null)
					   ? !ReferenceEquals(objA: c2, objB: null)
					   : c1.CompareTo(other: c2) != 0;
		}

		public static bool operator <(TypeDefinition c1, TypeDefinition c2)
		{
			return !ReferenceEquals(objA: c1, objB: null) && c1.CompareTo(other: c2) < 0;
		}

		public static bool operator >(TypeDefinition c1, TypeDefinition c2)
		{
			return !ReferenceEquals(objA: c1, objB: null) && c1.CompareTo(other: c2) > 0;
		}

		public virtual int CompareTo(object obj)
		{
			var other = obj as TypeDefinition;
			return CompareTo(other: other);
		}

		public int CompareTo(ITypeDefinition other)
		{
			return other == null
					   ? -1
					   : string.Compare(strA: ToString(), strB: other.ToString(), comparisonType: StringComparison.OrdinalIgnoreCase);
		}

		public override string ToString()
		{
			return _fullName;
		}

		public override bool Equals(object obj)
		{
			return CompareTo(obj: obj) == 0;
		}

		public override int GetHashCode()
		{
			return _fullName.GetHashCode();
		}
	}
}