// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyntaxExtensions.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SyntaxExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SimiSharp.CodeAnalysis.Metrics
{
	internal static class SyntaxExtensions
	{
		public static string GetName(this NamespaceDeclarationSyntax node, SyntaxNode rootNode)
		{
			var name = node.Name;
			return rootNode.GetText().GetSubText(span: name.Span).ToString();
		}

		public static string GetName(this TypeDeclarationSyntax node, SyntaxNode rootNode)
		{
			var identifier = node.Identifier;
			return rootNode.GetText().GetSubText(span: identifier.Span).ToString();
		}
	}
}
