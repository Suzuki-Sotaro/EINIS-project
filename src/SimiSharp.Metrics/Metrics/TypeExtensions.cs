// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.Metrics;

namespace SimiSharp.CodeAnalysis.Metrics
{
	internal static class TypeExtensions
	{
		public static string GetName(this TypeDeclarationSyntax syntax)
		{
			var containingTypeName = string.Join(separator: ".", values: GetContainingTypeName(syntax: syntax).Reverse());
			if (syntax.TypeParameterList != null)
			{
				var parameters = syntax.TypeParameterList.Parameters;
				if (parameters.Any())
				{
					var str3 = string.Join(separator: ", ", values: from x in parameters select x.Identifier.ValueText);
					containingTypeName = containingTypeName + $"<{str3}>";
				}
			}

			return containingTypeName;
		}

		public static ITypeDefinition GetQualifiedName(this ITypeSymbol symbol)
		{
			var name = string.Join(separator: ".", values: GetContainingTypeName(symbol: symbol).Reverse());

			var namedTypeSymbol = symbol as INamedTypeSymbol;
			if (namedTypeSymbol != null && namedTypeSymbol.TypeParameters != null && namedTypeSymbol.TypeParameters.Any())
			{
				var joined = string.Join(separator: ", ", values: namedTypeSymbol.TypeParameters.Select(selector: x => x.Name));
				name = name + $"<{joined}>";
			}

			var namespaceNames = new List<string>();
			for (var containingSymbol = symbol.ContainingSymbol; (containingSymbol != null) && (containingSymbol.Kind == SymbolKind.Namespace); containingSymbol = containingSymbol.ContainingSymbol)
			{
				var namespaceSymbol = (INamespaceSymbol)containingSymbol;
				if (namespaceSymbol.IsGlobalNamespace)
				{
					return new TypeDefinition(typeName: name, namespaceName: string.Join(separator: ".", values: namespaceNames), assemblyName: namespaceSymbol.ContainingAssembly.Name);
				}

				namespaceNames.Add(item: namespaceSymbol.Name);
			}

			return new TypeDefinition(typeName: name, namespaceName: string.Join(separator: ".", values: namespaceNames), assemblyName: string.Empty);
		}

		private static IEnumerable<string> GetContainingTypeName(TypeDeclarationSyntax syntax)
		{
			for (var typeDeclaration = syntax; typeDeclaration != null; typeDeclaration = typeDeclaration.Parent as TypeDeclarationSyntax)
			{
				yield return typeDeclaration.Identifier.ValueText;
			}
		}

		private static IEnumerable<string> GetContainingTypeName(ITypeSymbol symbol)
		{
			for (var typeSymbol = symbol; typeSymbol != null; typeSymbol = typeSymbol.ContainingSymbol as ITypeSymbol)
			{
				yield return typeSymbol.Name;
			}
		}
	}
}
