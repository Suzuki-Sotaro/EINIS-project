// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceDeclaration.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceDeclaration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace SimiSharp.CodeAnalysis.Metrics
{
	public sealed class NamespaceDeclaration
	{
		public string Name { get; set; }

		public IEnumerable<NamespaceDeclarationSyntaxInfo> SyntaxNodes { get; set; }
	}
}
