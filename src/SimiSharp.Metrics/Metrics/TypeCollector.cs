// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeCollector.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeCollector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common;

namespace SimiSharp.CodeAnalysis.Metrics
{
	internal sealed class TypeCollector
	{
		public IEnumerable<TypeDeclarationSyntax> GetTypes(SyntaxNode namespaceNode)
		{
			var innerCollector = new InnerTypeCollector();
			return innerCollector.GetTypes(namespaceNode: namespaceNode);
		}

		private class InnerTypeCollector : CSharpSyntaxWalker
		{
			private readonly IList<TypeDeclarationSyntax> _types;

			public InnerTypeCollector()
				: base(depth: SyntaxWalkerDepth.Node)
			{
				_types = new List<TypeDeclarationSyntax>();
			}

			public IEnumerable<TypeDeclarationSyntax> GetTypes(SyntaxNode namespaceNode)
			{
				var node = namespaceNode as NamespaceDeclarationSyntax;
				if (node != null)
				{
					Visit(node: node);
				}

				return _types.AsArray();
			}

			public override void VisitClassDeclaration(ClassDeclarationSyntax node)
			{
				base.VisitClassDeclaration(node: node);
				_types.Add(item: node);
			}

			public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
			{
				base.VisitInterfaceDeclaration(node: node);
				_types.Add(item: node);
			}

			public override void VisitStructDeclaration(StructDeclarationSyntax node)
			{
				base.VisitStructDeclaration(node: node);
				_types.Add(item: node);
			}
		}
	}
}
