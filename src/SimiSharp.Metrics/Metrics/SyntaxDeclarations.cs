// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyntaxDeclarations.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SyntaxDeclarations type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SimiSharp.CodeAnalysis.Metrics
{
	internal class SyntaxDeclarations
	{
		public IEnumerable<NamespaceDeclarationSyntax> NamespaceDeclarations { get; set; }

		public IEnumerable<TypeDeclarationSyntax> TypeDeclarations { get; set; }

		public IEnumerable<MemberDeclarationSyntax> MemberDeclarations { get; set; }

		public IEnumerable<SyntaxNode> Statements { get; set; }
	}
}
