// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnreadVariableRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnreadVariableRule type.
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
	internal class UnreadVariableRule : UnreadValueRule
	{
		public override string ID => "AM0061";

		public override SyntaxKind EvaluatedKind => SyntaxKind.VariableDeclaration;

		public override string Title => "Variable is never read";

		public override string Suggestion => "Remove unread variable.";

		protected override IEnumerable<ISymbol> GetSymbols(SyntaxNode node, SemanticModel semanticModel)
		{
			var declaration = (VariableDeclarationSyntax)node;

			var symbols = declaration.Variables.Select(selector: x => semanticModel.GetDeclaredSymbol(declarationSyntax: x)).AsArray();

			return symbols;
		}
	}
}