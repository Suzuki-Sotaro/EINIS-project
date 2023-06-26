// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DepthOfInheritanceAnalyzer.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DepthOfInheritanceAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SimiSharp.CodeAnalysis.Metrics
{
	internal sealed class DepthOfInheritanceAnalyzer
	{
		private readonly IEnumerable<TypeKind> _inheritableTypes = new[] { TypeKind.Class, TypeKind.Struct };
		private readonly SemanticModel _semanticModel;

		public DepthOfInheritanceAnalyzer(SemanticModel semanticModel)
		{
			_semanticModel = semanticModel;
		}

		public int Calculate(TypeDeclarationSyntax type)
		{
			var num = type.IsKind(kind: SyntaxKind.ClassDeclaration) || type.IsKind(kind: SyntaxKind.StructDeclaration) ? 1 : 0;
			if (type.BaseList != null)
			{
				foreach (var symbolInfo in type.BaseList.Types.Select(selector: syntax => _semanticModel.GetSymbolInfo(node: syntax)))
				{
					for (var symbol = symbolInfo.Symbol as INamedTypeSymbol; symbol != null; symbol = symbol.BaseType)
					{
						if (_inheritableTypes.Any(predicate: x => x == symbol.TypeKind))
						{
							num++;
						}
					}
				}
			}

			return num == 0 && (type.IsKind(kind: SyntaxKind.ClassDeclaration) || type.IsKind(kind: SyntaxKind.StructDeclaration))
				? 1
				: num;
		}
	}
}
