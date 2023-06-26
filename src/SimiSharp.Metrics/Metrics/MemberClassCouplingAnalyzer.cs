// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberClassCouplingAnalyzer.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberClassCouplingAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.Metrics;

namespace SimiSharp.CodeAnalysis.Metrics
{
	internal sealed class MemberClassCouplingAnalyzer : ClassCouplingAnalyzerBase
	{
		private readonly List<IMethodSymbol> _calledMethods;
		private readonly List<IPropertySymbol> _calledProperties;
		private readonly Dictionary<SyntaxKind, Action<SyntaxNode>> _classCouplingActions;
		private readonly Dictionary<SymbolKind, Action<ISymbol>> _symbolActions;
		private readonly List<IEventSymbol> _usedEvents;

		public MemberClassCouplingAnalyzer(SemanticModel semanticModel)
			: base(semanticModel: semanticModel)
		{
			_calledMethods = new List<IMethodSymbol>();
			_calledProperties = new List<IPropertySymbol>();
			_usedEvents = new List<IEventSymbol>();
			_symbolActions = new Dictionary<SymbolKind, Action<ISymbol>>
								 {
									 { SymbolKind.NamedType, x => FilterTypeSymbol(symbol: (ITypeSymbol)x) }, 
									 { SymbolKind.Parameter, x => FilterTypeSymbol(symbol: ((IParameterSymbol)x).Type) }, 
									 { SymbolKind.Method, x => FilterTypeSymbol(symbol: x.ContainingType) }, 
									 { SymbolKind.Field, x => FilterTypeSymbol(symbol: ((IFieldSymbol)x).Type) }, 
									 { SymbolKind.Property, x => FilterTypeSymbol(symbol: x.ContainingType) }, 
									 { SymbolKind.Event, x => FilterTypeSymbol(symbol: x.ContainingType) }
								 };

			_classCouplingActions = new Dictionary<SyntaxKind, Action<SyntaxNode>>
										{
											{ SyntaxKind.MethodDeclaration, x => CalculateMethodClassCoupling(syntax: (MethodDeclarationSyntax)x) }, 
				                        { SyntaxKind.ConstructorDeclaration, x => CalculateConstructorCoupling(syntax: (ConstructorDeclarationSyntax)x) }, 
				                        { SyntaxKind.DestructorDeclaration, x => CalculateConstructorCoupling(syntax: (DestructorDeclarationSyntax)x) }, 
				                        { SyntaxKind.GetAccessorDeclaration, x => CalculateAccessorClassCoupling(accessor: (AccessorDeclarationSyntax)x) }, 
				                        { SyntaxKind.SetAccessorDeclaration, x => CalculateAccessorClassCoupling(accessor: (AccessorDeclarationSyntax)x) }, 
										{ SyntaxKind.EventFieldDeclaration, x => CalculateEventClassCoupling(syntax: (EventFieldDeclarationSyntax)x) }, 
				                        { SyntaxKind.AddAccessorDeclaration, x => CalculateAccessorClassCoupling(accessor: (AccessorDeclarationSyntax)x) }, 
				                        { SyntaxKind.RemoveAccessorDeclaration, x => CalculateAccessorClassCoupling(accessor: (AccessorDeclarationSyntax)x) }
										};
		}

		public IEnumerable<ITypeCoupling> Calculate(SyntaxNode syntaxNode)
		{
			Action<SyntaxNode> action;

			if (_classCouplingActions.TryGetValue(key: syntaxNode.Kind(), value: out action))
			{
				action(obj: syntaxNode);
			}

			return GetCollectedTypesNames(calledProperties: _calledProperties, calledMethods: _calledMethods, usedEvents: _usedEvents);
		}

		public override void VisitIdentifierName(IdentifierNameSyntax node)
		{
			base.VisitIdentifierName(node: node);
			var symbolInfo = SemanticModel.GetSymbolInfo(expression: node);
			if (symbolInfo.Symbol != null)
			{
				Action<ISymbol> action;
				var symbol = symbolInfo.Symbol;
				if (_symbolActions.TryGetValue(key: symbol.Kind, value: out action))
				{
					action(obj: symbol);
				}
			}
		}

		public override void VisitParameter(ParameterSyntax node)
		{
			base.VisitParameter(node: node);
			var type = node.Type;
			if (type != null)
			{
				FilterType(syntax: type);
			}
		}

		private void CalculateEventClassCoupling(EventFieldDeclarationSyntax syntax)
		{
			IdentifierNameSyntax node = (IdentifierNameSyntax)syntax.Declaration.Type;
			var symbolInfo = SemanticModel.GetSymbolInfo(expression: node);
			if (symbolInfo.Symbol != null)
			{
				Action<ISymbol> action;
				var symbol = symbolInfo.Symbol;
				if (_symbolActions.TryGetValue(key: symbol.Kind, value: out action))
				{
					action(obj: symbol);
				}
			}
		}

		private void CalculateConstructorCoupling(BaseMethodDeclarationSyntax syntax)
		{
			Visit(node: syntax);
			if (syntax.Body != null)
			{
				CollectMemberCouplings(syntax: syntax.Body);
			}
		}

		private void CalculateMethodClassCoupling(MethodDeclarationSyntax syntax)
		{
			Visit(node: syntax);
			FilterType(syntax: syntax.ReturnType);
			if (syntax.Body != null)
			{
				CollectMemberCouplings(syntax: syntax.Body);
			}
		}

		private void CalculateAccessorClassCoupling(AccessorDeclarationSyntax accessor)
		{
			var syntax = (BasePropertyDeclarationSyntax)accessor.Parent.Parent;
			FilterType(syntax: syntax.Type);

			var body = accessor.Body;
			if (body != null)
			{
				Visit(node: body);
				CollectMemberCouplings(syntax: body);
			}
		}

		private void CollectMemberCouplings(SyntaxNode syntax)
		{
			if (syntax == null)
			{
				return;
			}

			var methodCouplings = GetMemberCouplings<MemberAccessExpressionSyntax>(block: syntax)
				.Union(second: GetMemberCouplings<IdentifierNameSyntax>(block: syntax))
				.Where(predicate: x => x.Kind == SymbolKind.Method || x.Kind == SymbolKind.Property || x.Kind == SymbolKind.Event)
				.AsArray();
			_calledMethods.AddRange(collection: methodCouplings.Where(predicate: x => x.Kind == SymbolKind.Method).Cast<IMethodSymbol>());
			_calledProperties.AddRange(collection: methodCouplings.Where(predicate: x => x.Kind == SymbolKind.Property).Cast<IPropertySymbol>());
			_usedEvents.AddRange(collection: methodCouplings.Where(predicate: x => x.Kind == SymbolKind.Event).Cast<IEventSymbol>());
		}

		private IEnumerable<ISymbol> GetMemberCouplings<T>(SyntaxNode block) where T : ExpressionSyntax
		{
			return block.DescendantNodes()
				.OfType<T>()
				.Select(selector: r => new { node = r, model = SemanticModel })
				.Select(selector: info => info.model.GetSymbolInfo(expression: info.node).Symbol)
				.Where(predicate: x => x != null);
		}
	}
}
