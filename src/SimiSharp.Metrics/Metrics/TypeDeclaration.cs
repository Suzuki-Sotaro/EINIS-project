// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeDeclaration.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeDeclaration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using SimiSharp.CodeAnalysis.Common.Metrics;

namespace SimiSharp.CodeAnalysis.Metrics
{
	internal sealed class TypeDeclaration
	{
		public string Name { get; set; }

		public IEnumerable<TypeDeclarationSyntaxInfo> SyntaxNodes { get; set; }
	}
}
