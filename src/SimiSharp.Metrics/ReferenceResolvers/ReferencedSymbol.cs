// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferencedSymbol.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ReferencedSymbol type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using SimiSharp.CodeAnalysis.Common;

namespace SimiSharp.CodeAnalysis.ReferenceResolvers
{
	public class ReferencedSymbol
	{
		public ReferencedSymbol(ISymbol symbol, IEnumerable<ReferenceLocation> locations)
		{
			Symbol = symbol;
			Locations = locations.AsArray();
		}

		public ISymbol Symbol { get; private set; }

		public IEnumerable<ReferenceLocation> Locations { get; private set; }
	}
}