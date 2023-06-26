// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassCouplingAnalyzerBase.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ClassCouplingAnalyzerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.Metrics;

namespace SimiSharp.CodeAnalysis.Metrics
{
	internal abstract class ClassCouplingAnalyzerBase : CSharpSyntaxWalker
	{
	    private readonly IDictionary<string, ITypeSymbol> _types;

		protected ClassCouplingAnalyzerBase(SemanticModel semanticModel)
			: base(depth: SyntaxWalkerDepth.Node)
		{
			_types = new Dictionary<string, ITypeSymbol>();

			SemanticModel = semanticModel;
		}

		protected SemanticModel SemanticModel { get; }

	    protected void FilterType(TypeSyntax syntax)
		{
			if (syntax.IsKind(kind: SyntaxKind.PredefinedType))
			{
				var symbolInfo = SemanticModel.GetSymbolInfo(expression: syntax);
				if ((symbolInfo.Symbol != null) && (symbolInfo.Symbol.Kind == SymbolKind.NamedType))
				{
					var symbol = (ITypeSymbol)symbolInfo.Symbol;
					FilterTypeSymbol(symbol: symbol);
				}
			}
		}

		protected void FilterTypeSymbol(ITypeSymbol symbol)
		{
			switch (symbol.TypeKind)
			{
				case TypeKind.Class:
				case TypeKind.Delegate:
				case TypeKind.Enum:
				case TypeKind.Interface:
					{
						var qualifiedName = symbol.GetQualifiedName().ToString();
						if (!_types.ContainsKey(key: qualifiedName))
						{
							_types.Add(key: qualifiedName, value: symbol);
						}

						break;
					}

				case TypeKind.Dynamic:
				case TypeKind.Error:
				case TypeKind.TypeParameter:
					break;

				default:
					return;
			}
		}

		protected IEnumerable<ITypeCoupling> GetCollectedTypesNames()
		{
			return GetCollectedTypesNames(calledProperties: new IPropertySymbol[0], calledMethods: new IMethodSymbol[0], usedEvents: new IEventSymbol[0]);
		}

		protected IEnumerable<ITypeCoupling> GetCollectedTypesNames(IEnumerable<IPropertySymbol> calledProperties, IEnumerable<IMethodSymbol> calledMethods, IEnumerable<IEventSymbol> usedEvents)
		{
			var memberCouplings = _types.Select(selector: x => CresateTypeCoupling(calledProperties: calledProperties, calledMethods: calledMethods, usedEvents: usedEvents, x: x)).AsArray();
			var inheritedCouplings = _types
				.Select(selector: x => x.Value)
				.SelectMany(selector: GetInheritedTypeNames);
			var interfaces = _types.SelectMany(selector: x => x.Value.AllInterfaces);
			var inheritedTypeCouplings = inheritedCouplings.Concat(second: interfaces)
				.Select(selector: CreateTypeCoupling)
				.Except(second: memberCouplings);

			return memberCouplings.Concat(second: inheritedTypeCouplings);
		}

		private static TypeCoupling CresateTypeCoupling(
			IEnumerable<IPropertySymbol> calledProperties,
			IEnumerable<IMethodSymbol> calledMethods,
			IEnumerable<IEventSymbol> usedEvents,
			KeyValuePair<string, ITypeSymbol> x)
		{
			var typeSymbol = x.Value;
			var usedMethods =
				calledMethods.Where(predicate: m => m.ContainingType.ToDisplayString() == typeSymbol.ToDisplayString())
					.Select(selector: m => m.ToDisplayString());
			var usedProperties =
				calledProperties.Where(predicate: m => m.ContainingType.ToDisplayString() == typeSymbol.ToDisplayString())
					.Select(selector: m => m.ToDisplayString());
			var events =
				usedEvents.Where(predicate: m => m.ContainingType.ToDisplayString() == typeSymbol.ToDisplayString())
					.Select(selector: m => m.ToDisplayString());

			return CreateTypeCoupling(typeSymbol: typeSymbol, usedMethods: usedMethods, usedProperties: usedProperties, events: events);
		}

		private static TypeCoupling CreateTypeCoupling(ITypeSymbol typeSymbol)
		{
			return CreateTypeCoupling(typeSymbol: typeSymbol, usedMethods: Enumerable.Empty<string>(), usedProperties: Enumerable.Empty<string>(), events: Enumerable.Empty<string>());
		}

		private static TypeCoupling CreateTypeCoupling(ITypeSymbol typeSymbol, IEnumerable<string> usedMethods, IEnumerable<string> usedProperties, IEnumerable<string> events)
		{
			var name = typeSymbol.IsAnonymousType ? typeSymbol.ToDisplayString() : typeSymbol.Name;

			var namespaceName = string.Join(separator: ".", values: GetFullNamespace(namespaceSymbol: typeSymbol.ContainingNamespace));
			if (string.IsNullOrWhiteSpace(value: namespaceName))
			{
				namespaceName = "global";
			}

			var assemblyName = "Unknown";
			if (typeSymbol.ContainingAssembly != null)
			{
				assemblyName = typeSymbol.ContainingAssembly.Name;
			}

			return new TypeCoupling(typeName: name, namespaceName: namespaceName, assemblyName: assemblyName, usedMethods: usedMethods, usedProperties: usedProperties, useEvents: events);
		}

		private static IEnumerable<ITypeSymbol> GetInheritedTypeNames(ITypeSymbol symbol)
		{
			if (symbol.BaseType == null)
			{
				yield break;
			}

			yield return symbol.BaseType;
			foreach (var name in GetInheritedTypeNames(symbol: symbol.BaseType))
			{
				yield return name;
			}
		}

		private static IEnumerable<string> GetFullNamespace(INamespaceSymbol namespaceSymbol)
		{
			if (namespaceSymbol.ContainingNamespace != null
				&& !namespaceSymbol.ContainingNamespace.IsGlobalNamespace)
			{
				foreach (var ns in GetFullNamespace(namespaceSymbol: namespaceSymbol.ContainingNamespace))
				{
					yield return ns;
				}
			}

			yield return namespaceSymbol.Name;
		}
	}
}
