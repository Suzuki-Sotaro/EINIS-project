// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnreadFieldRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnreadFieldRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common;

namespace SimiSharp.CodeReview.Rules.Semantic
{
	internal class UnreadFieldRule : UnreadValueRule
	{
		public override string ID => "AM0059";

		public override SyntaxKind EvaluatedKind => SyntaxKind.FieldDeclaration;

		public override string Title => "Field is never read";

		public override string Suggestion => "Remove unread field.";

		protected override IEnumerable<ISymbol> GetSymbols(SyntaxNode node, SemanticModel semanticModel)
		{
			var declaration = (FieldDeclarationSyntax)node;

			var symbols = declaration.Declaration.Variables.Select(selector: x => semanticModel.GetDeclaredSymbol(declarationSyntax: x)).AsArray();

			return symbols;
		}
	}
}