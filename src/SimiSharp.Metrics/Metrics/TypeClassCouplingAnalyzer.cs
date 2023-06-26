// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeClassCouplingAnalyzer.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeClassCouplingAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.Metrics;

namespace SimiSharp.CodeAnalysis.Metrics
{
	internal sealed class TypeClassCouplingAnalyzer : ClassCouplingAnalyzerBase
	{
		public TypeClassCouplingAnalyzer(SemanticModel semanticModel)
			: base(semanticModel: semanticModel)
		{
		}

		public IEnumerable<ITypeCoupling> Calculate(TypeDeclarationSyntax typeNode)
		{
			SyntaxNode node = typeNode;
			Visit(node: node);
			return GetCollectedTypesNames();
		}

		public override void VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			base.VisitClassDeclaration(node: node);
			if (node.BaseList != null)
			{
				var symbol = node.BaseList.Types
								 .Select(selector: x => SemanticModel.GetSymbolInfo(node: x))
								 .Where(predicate: x => (x.Symbol != null) && (x.Symbol.Kind == SymbolKind.NamedType))
								 .Select(selector: x => x.Symbol)
								 .OfType<INamedTypeSymbol>()
								 .FirstOrDefault();
				if (symbol != null)
				{
					FilterTypeSymbol(symbol: symbol);
				}
			}
		}

		public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
		{
			base.VisitFieldDeclaration(node: node);
			FilterType(syntax: node.Declaration.Type);
		}
	}
}
