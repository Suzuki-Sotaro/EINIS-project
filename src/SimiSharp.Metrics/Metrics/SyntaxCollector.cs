// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyntaxCollector.cs" company="Reimers.dk">
//   Copyright � 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SyntaxCollector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common;

namespace SimiSharp.CodeAnalysis.Metrics
{
	internal sealed class SyntaxCollector : CSharpSyntaxWalker
	{
		private readonly IList<MemberDeclarationSyntax> _members = new List<MemberDeclarationSyntax>();
		private readonly IList<NamespaceDeclarationSyntax> _namespaces = new List<NamespaceDeclarationSyntax>();
		private readonly IList<SyntaxNode> _statements = new List<SyntaxNode>();
		private readonly IList<TypeDeclarationSyntax> _types = new List<TypeDeclarationSyntax>();

		public SyntaxDeclarations GetDeclarations(IEnumerable<SyntaxTree> trees)
		{
			var syntaxTrees = trees.AsArray();

			foreach (var root in syntaxTrees.Select(selector: syntaxTree => syntaxTree.GetRoot()))
			{
				Visit(node: root);
				CheckStatementSyntax(node: root);
			}

			return new SyntaxDeclarations
			{
				MemberDeclarations = _members.AsArray(), 
				NamespaceDeclarations = _namespaces.AsArray(), 
				Statements = _statements.AsArray(), 
				TypeDeclarations = _types.AsArray()
			};
		}

		public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
		{
			_namespaces.Add(item: node);
		}

		public override void VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			_types.Add(item: node);
		}

		public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
		{
			_types.Add(item: node);
		}

		public override void VisitStructDeclaration(StructDeclarationSyntax node)
		{
			_types.Add(item: node);
		}

		public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
		{
			_members.Add(item: node);
		}

		public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
		{
			_members.Add(item: node);
		}

		public override void VisitEventDeclaration(EventDeclarationSyntax node)
		{
			_members.Add(item: node);
		}

		public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			_members.Add(item: node);
		}

		public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
		{
			_members.Add(item: node);
		}

		private void CheckStatementSyntax(SyntaxNode node)
		{
			var syntaxNodes = node.ChildNodes().AsArray();
			
			var statements =
				syntaxNodes
				.Where(predicate: x => !(x is TypeDeclarationSyntax))
					.Where(predicate: x => x is BaseFieldDeclarationSyntax || x is StatementSyntax)
					.AsArray();

			foreach (var statement in statements)
			{
				_statements.Add(item: statement);
			}
		}
	}
}
