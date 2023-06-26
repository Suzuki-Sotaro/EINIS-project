// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoslynExtensions.cs" company="Reimers.dk">
//   Copyright � 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RoslynExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SimiSharp.CodeAnalysis
{
	public static class RoslynExtensions
	{
		public static MethodDeclarationSyntax GetMethod(this SyntaxToken token)
		{
			var parent = token.Parent;
			return GetMethod(node: parent);
		}

		public static MethodDeclarationSyntax GetMethod(this SyntaxNode node)
		{
			if (node == null)
			{
				return null;
			}

			if (node.IsKind(kind: SyntaxKind.MethodDeclaration))
			{
				return (MethodDeclarationSyntax)node;
			}

			return GetMethod(node: node.Parent);
		}

		public static bool EquivalentTo(this SyntaxNode node1, SyntaxNode node2)
		{
			if (node1 == null || node2 == null)
			{
				return node1 == null && node2 == null;
			}

			return node1.RawKind == node2.RawKind && node1.ToFullString().Trim().Equals(value: node2.ToFullString().Trim());
		}
	}
}