// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EvaluationBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.CodeReview;
using SimiSharp.CodeAnalysis.Metrics;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal abstract class EvaluationBase : IEvaluation
	{
		private readonly LinesOfCodeCalculator _locCalculator = new LinesOfCodeCalculator();

		public abstract string ID { get; }

		public abstract CodeQuality Quality { get; }

		public abstract QualityAttribute QualityAttribute { get; }

		public abstract ImpactLevel ImpactLevel { get; }

		public abstract SyntaxKind EvaluatedKind { get; }

		public abstract string Title { get; }

		public abstract string Suggestion { get; }

		protected static string GetNamespace(SyntaxNode node)
		{
			var namespaceDeclaration = node as NamespaceDeclarationSyntax;
			if (namespaceDeclaration != null)
			{
				return namespaceDeclaration.Name.GetText().ToString().Trim();
			}

			if (node.Parent == null)
			{
				return SyntaxFactory.Token(kind: SyntaxKind.GlobalKeyword).ValueText;
			}

			return GetNamespace(node: node.Parent);
		}

		protected static Tuple<string, string> GetNodeType(SyntaxNode node)
		{
			var declaration = node as TypeDeclarationSyntax;
			if (declaration != null)
			{
				var keyword = declaration.Keyword.ValueText.ToTitleCase();
				var name = declaration.Identifier.ValueText;
				return new Tuple<string, string>(item1: keyword, item2: name);
			}

			if (node.Parent == null)
			{
				return new Tuple<string, string>(item1: SyntaxFactory.Token(kind: SyntaxKind.GlobalKeyword).ValueText, item2: string.Empty);
			}

			return GetNodeType(node: node.Parent);
		}

		protected static SyntaxNode FindMethodParent(SyntaxNode node)
		{
			if (node.Parent == null)
			{
				return null;
			}

			if (node.Parent.IsKind(kind: SyntaxKind.MethodDeclaration) || node.Parent.IsKind(kind: SyntaxKind.ConstructorDeclaration))
			{
				return node.Parent;
			}

			return FindMethodParent(node: node.Parent);
		}

		protected int GetLinesOfCode(SyntaxNode node)
		{
			return _locCalculator.Calculate(node: node);
		}

		protected TypeDeclarationSyntax FindClassParent(SyntaxNode node)
		{
			if (node.Parent == null)
			{
				return null;
			}

			if (node.Parent.IsKind(kind: SyntaxKind.ClassDeclaration) || node.Parent.IsKind(kind: SyntaxKind.StructDeclaration))
			{
				return node.Parent as TypeDeclarationSyntax;
			}

			return FindClassParent(node: node.Parent);
		}

		protected NamespaceDeclarationSyntax FindNamespaceParent(SyntaxNode node)
		{
			if (node.Parent == null)
			{
				return null;
			}

			if (node.Parent.IsKind(kind: SyntaxKind.NamespaceDeclaration))
			{
				return node.Parent as NamespaceDeclarationSyntax;
			}

			return FindNamespaceParent(node: node.Parent);
		}
	}
}