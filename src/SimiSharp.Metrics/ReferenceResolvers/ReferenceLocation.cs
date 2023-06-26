// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceLocation.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ReferenceLocation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

namespace SimiSharp.CodeAnalysis.ReferenceResolvers
{
	public class ReferenceLocation
	{
		public ReferenceLocation(Location location, ITypeSymbol referencingType, SemanticModel model)
		{
			Location = location;
			ReferencingType = referencingType;
			Model = model;
		}

		public Location Location { get; private set; }

		public ITypeSymbol ReferencingType { get; private set; }

		public SemanticModel Model { get; private set; }
	}
}