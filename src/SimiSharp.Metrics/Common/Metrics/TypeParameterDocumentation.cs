// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeParameterDocumentation.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeParameterDocumentation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimiSharp.CodeAnalysis.Common.Metrics
{
	public class TypeParameterDocumentation
	{
		public TypeParameterDocumentation(string typeParameterName, string constraint, string description)
		{
			TypeParameterName = typeParameterName;
			Constraint = constraint;
			Description = description;
		}

		public string TypeParameterName { get; }

		public string Constraint { get; }

		public string Description { get; }
	}
}