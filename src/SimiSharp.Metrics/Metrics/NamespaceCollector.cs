// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceCollector.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceCollector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common;

namespace SimiSharp.CodeAnalysis.Metrics
{
	internal sealed class NamespaceCollector : CSharpSyntaxWalker
	{
		private readonly IList<NamespaceDeclarationSyntax> _namespaces;

		public NamespaceCollector()
			: base(depth: SyntaxWalkerDepth.Node)
		{
			_namespaces = new List<NamespaceDeclarationSyntax>();
		}

		public IEnumerable<NamespaceDeclarationSyntax> GetNamespaces(SyntaxNode commonNode)
		{
			var node = commonNode as SyntaxNode;
			if (node != null)
			{
				Visit(node: node);
			}

			return _namespaces.AsArray();
		}

		public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
		{
			base.VisitNamespaceDeclaration(node: node);
			_namespaces.Add(item: node);
		}
	}
}
