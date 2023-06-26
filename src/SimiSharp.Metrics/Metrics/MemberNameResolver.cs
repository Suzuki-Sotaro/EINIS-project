// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberNameResolver.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberNameResolver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SimiSharp.CodeAnalysis.Metrics
{
	internal sealed class MemberNameResolver
	{
		private readonly SemanticModel _semanticModel;

		public MemberNameResolver(SemanticModel semanticModel)
		{
			_semanticModel = semanticModel;
		}

		public string TryResolveMemberSignatureString(SyntaxNode syntaxNode)
		{
			Func<SyntaxNode, string> func;
			var dictionary = new Dictionary<SyntaxKind, Func<SyntaxNode, string>>
				                  {
					                  { SyntaxKind.MethodDeclaration, x => GetMethodSignatureString(syntax: (MethodDeclarationSyntax)x) }, 
					                  { SyntaxKind.ConstructorDeclaration, x => GetConstructorSignatureString(syntax: (ConstructorDeclarationSyntax)x) }, 
					                  { SyntaxKind.DestructorDeclaration, x => GetDestructorSignatureString(syntax: (DestructorDeclarationSyntax)x) }, 
					                  { SyntaxKind.GetAccessorDeclaration, x => GetPropertyGetterSignatureString(syntax: (AccessorDeclarationSyntax)x) }, 
									  { SyntaxKind.SetAccessorDeclaration, x => GetPropertySetterSignatureString(syntax: (AccessorDeclarationSyntax)x) }, 
					                  { SyntaxKind.AddAccessorDeclaration, x => GetAddEventHandlerSignatureString(accessor: (AccessorDeclarationSyntax)x) }, 
					                  { SyntaxKind.RemoveAccessorDeclaration, x => GetRemoveEventHandlerSignatureString(accessor: (AccessorDeclarationSyntax)x) }
				                  };
			var kind = syntaxNode.Kind();
			return dictionary.TryGetValue(key: kind, value: out func)
				? func(arg: syntaxNode)
				: string.Empty;
		}

		private static string ResolveTypeName(ITypeSymbol symbol)
		{
			INamedTypeSymbol symbol3;
			var builder = new StringBuilder();
			var flag = false;
			var symbol2 = symbol as IArrayTypeSymbol;
			if (symbol2 != null)
			{
				flag = true;
				symbol = symbol2.ElementType;
			}

			builder.Append(value: symbol.Name);
			if (((symbol3 = symbol as INamedTypeSymbol) != null) && symbol3.TypeArguments.Any())
			{
				IEnumerable<string> values = (from x in symbol3.TypeArguments.AsEnumerable() select ResolveTypeName(symbol: x)).ToArray();
				builder.AppendFormat(format: "<{0}>", arg0: string.Join(separator: ", ", values: values));
			}

			if (flag)
			{
				builder.Append(value: "[]");
			}

			return builder.ToString();
		}

		private static void AppendMethodIdentifier(ConstructorDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append(value: syntax.Identifier.ValueText);
		}

		private static void AppendMethodIdentifier(DestructorDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append(value: syntax.Identifier.ValueText);
		}

		private static void AppendMethodIdentifier(EventDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append(value: syntax.Identifier.ValueText);
		}

		private static void AppendMethodIdentifier(MethodDeclarationSyntax syntax, StringBuilder builder)
		{
			ExplicitInterfaceSpecifierSyntax syntax2;
			IdentifierNameSyntax syntax3;
			if (((syntax2 = syntax.ExplicitInterfaceSpecifier) != null) && ((syntax3 = syntax2.Name as IdentifierNameSyntax) != null))
			{
				var valueText = syntax3.Identifier.ValueText;
				builder.AppendFormat(format: "{0}.", arg0: valueText);
			}

			builder.Append(value: syntax.Identifier.ValueText);
		}

		private static string GetMethodIdentifier(PropertyDeclarationSyntax syntax)
		{
			return syntax.Identifier.ValueText;
		}

		private static void AppendTypeParameters(MethodDeclarationSyntax syntax, StringBuilder builder)
		{
			if (syntax.TypeParameterList != null)
			{
				var parameters = syntax.TypeParameterList.Parameters;
				if (parameters.Any())
				{
					var parameterNames = string.Join(separator: ", ", values: from x in parameters select x.Identifier.ValueText);
					builder.AppendFormat(format: "<{0}>", arg0: parameterNames);
				}
			}
		}

		private static string GetDestructorSignatureString(DestructorDeclarationSyntax syntax)
		{
			var builder = new StringBuilder();
			AppendMethodIdentifier(syntax: syntax, builder: builder);
			return builder.ToString();
		}

		private string GetConstructorSignatureString(ConstructorDeclarationSyntax syntax)
		{
			var builder = new StringBuilder();
			AppendMethodIdentifier(syntax: syntax, builder: builder);
			AppendParameters(syntax: syntax, builder: builder);
			return builder.ToString();
		}

		private string GetMethodSignatureString(MethodDeclarationSyntax syntax)
		{
			var builder = new StringBuilder();
			AppendMethodIdentifier(syntax: syntax, builder: builder);
			AppendTypeParameters(syntax: syntax, builder: builder);
			AppendParameters(syntax: syntax, builder: builder);
			AppendReturnType(syntax: syntax, builder: builder);
			return builder.ToString();
		}

		private string GetPropertyGetterSignatureString(AccessorDeclarationSyntax syntax)
		{
			var propertyDeclarationSyntax = syntax.Parent.Parent as PropertyDeclarationSyntax;
			var identifier = GetMethodIdentifier(syntax: propertyDeclarationSyntax) + ".get()";
			var returnType = GetReturnType(syntax: propertyDeclarationSyntax);
			return identifier + returnType;
		}

		private string GetPropertySetterSignatureString(AccessorDeclarationSyntax syntax)
		{
			var propertyDeclarationSyntax = syntax.Parent.Parent as PropertyDeclarationSyntax;
			var identifier = GetMethodIdentifier(syntax: propertyDeclarationSyntax);
			var parameters = GetParameters(syntax: propertyDeclarationSyntax);

			return $"{identifier}.set{parameters} : void";
		}

		private string GetAddEventHandlerSignatureString(AccessorDeclarationSyntax accessor)
		{
			var syntax = (EventDeclarationSyntax)accessor.Parent.Parent;
			var builder = new StringBuilder();
			AppendMethodIdentifier(syntax: syntax, builder: builder);
			builder.Append(value: ".add");
			AppendParameters(syntax: syntax, builder: builder);
			builder.Append(value: " : void");
			return builder.ToString();
		}

		private void AppendParameters(BaseMethodDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append(value: "(");
			var parameterList = syntax.ParameterList;
			if (parameterList != null)
			{
				var parameters = parameterList.Parameters;
				Func<ParameterSyntax, string> selector = parameters.Any() 
					? new Func<ParameterSyntax, string>(TypeNameSelector) 
					: x => string.Empty;
				
				var parameterNames = string.Join(separator: ", ", values: parameters.Select(selector: selector).Where(predicate: x => !string.IsNullOrWhiteSpace(value: x)));
				builder.Append(value: parameterNames);
			}

			builder.Append(value: ")");
		}

		private void AppendParameters(EventDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append(value: "(");
			var symbol = ModelExtensions.GetSymbolInfo(semanticModel: _semanticModel, node: syntax.Type).Symbol as ITypeSymbol;
			if (symbol != null)
			{
				var typeName = ResolveTypeName(symbol: symbol);
				builder.Append(value: typeName);
			}

			builder.Append(value: ")");
		}

		private string TypeNameSelector(ParameterSyntax x)
		{
			var b = new StringBuilder();
			var value = string.Join(separator: " ", values: from m in x.Modifiers select m.ValueText);
			if (!string.IsNullOrEmpty(value: value))
			{
				b.Append(value: value);
				b.Append(value: " ");
			}

			var symbol = ModelExtensions.GetSymbolInfo(semanticModel: _semanticModel, node: x.Type);
			var typeSymbol = symbol.Symbol as ITypeSymbol;
			if (typeSymbol == null)
			{
				return "?";
			}

			var typeName = ResolveTypeName(symbol: typeSymbol);
			if (!string.IsNullOrWhiteSpace(value: typeName))
			{
				b.Append(value: typeName);
			}

			return b.ToString();
		}

		private string GetParameters(BasePropertyDeclarationSyntax syntax)
		{
			var symbol = ModelExtensions.GetSymbolInfo(semanticModel: _semanticModel, node: syntax.Type).Symbol as ITypeSymbol;
			return $"({(symbol == null ? string.Empty : ResolveTypeName(symbol: symbol))})";
		}

		private void AppendReturnType(MethodDeclarationSyntax syntax, StringBuilder builder)
		{
			var symbolInfo = ModelExtensions.GetSymbolInfo(semanticModel: _semanticModel, node: syntax.ReturnType);
			var symbol = symbolInfo.Symbol as ITypeSymbol;
			if (symbol != null)
			{
				var typeName = ResolveTypeName(symbol: symbol);
				builder.AppendFormat(format: " : {0}", arg0: typeName);
			}
		}

		private string GetReturnType(BasePropertyDeclarationSyntax syntax)
		{
			var symbol = ModelExtensions.GetSymbolInfo(semanticModel: _semanticModel, node: syntax.Type).Symbol as ITypeSymbol;
			return symbol != null ? $": {ResolveTypeName(symbol: symbol)}" : string.Empty;
		}

		private string GetRemoveEventHandlerSignatureString(AccessorDeclarationSyntax accessor)
		{
			var syntax = (EventDeclarationSyntax)accessor.Parent.Parent;
			var builder = new StringBuilder();
			AppendMethodIdentifier(syntax: syntax, builder: builder);
			builder.Append(value: ".remove");
			AppendParameters(syntax: syntax, builder: builder);
			builder.Append(value: " : void");
			return builder.ToString();
		}
	}
}
