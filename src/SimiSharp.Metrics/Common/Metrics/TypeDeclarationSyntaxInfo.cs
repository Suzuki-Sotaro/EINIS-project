// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeDeclarationSyntaxInfo.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeDeclarationSyntaxInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SimiSharp.CodeAnalysis.Common.Metrics
{
	public class TypeDeclarationSyntaxInfo
	{
		public TypeDeclarationSyntaxInfo(string codeFile, string name, TypeDeclarationSyntax syntax)
		{
			CodeFile = codeFile;
			Name = name;
			Syntax = syntax;
		}

		public string CodeFile { get; }

		public string Name { get; }

		public TypeDeclarationSyntax Syntax { get; set; }
	}
}
