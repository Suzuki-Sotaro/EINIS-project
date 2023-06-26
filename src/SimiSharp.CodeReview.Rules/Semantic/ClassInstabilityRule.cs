// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassInstabilityRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ClassInstabilityRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Semantic
{
	internal class ClassInstabilityRule : SemanticEvaluationBase
	{
		public override string ID => "AM0053";

		public override ImpactLevel ImpactLevel => ImpactLevel.Type;

		public override SyntaxKind EvaluatedKind => SyntaxKind.ClassDeclaration;

		public override string Title => "Unstable Class";

		public override string Suggestion => "Refactor class dependencies.";

		public override CodeQuality Quality => CodeQuality.NeedsRefactoring;

		public override QualityAttribute QualityAttribute => QualityAttribute.Maintainability | QualityAttribute.Modifiability;

		protected override async Task<EvaluationResult> EvaluateImpl(SyntaxNode node, SemanticModel semanticModel, Solution solution)
		{
			var symbol = (ITypeSymbol)semanticModel.GetDeclaredSymbol(declaration: node);
			var efferent = GetReferencedTypes(classDeclaration: node, sourceSymbol: symbol, semanticModel: semanticModel).AsArray();
			var awaitable = SymbolFinder.FindCallersAsync(symbol: symbol, solution: solution, cancellationToken: CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false);
			var callers = (await awaitable).AsArray();
			var testCallers = callers
				.Where(predicate: c => c.CallingSymbol.GetAttributes()
				.Any(predicate: x => x.AttributeClass.Name.IsKnownTestAttribute()))
				.AsArray();
			var afferent = callers.Except(second: testCallers)
				.Select(selector: x => x.CallingSymbol.ContainingType)
				.DistinctBy(func: s => s.ToDisplayString())
				.AsArray();

			var efferentLength = (double)efferent.Length;
			var stability = efferentLength / (efferentLength + afferent.Length);
			if (stability >= 0.8)
			{
				return new EvaluationResult
				{
					ImpactLevel = ImpactLevel.Project,
					Quality = CodeQuality.NeedsReview,
					QualityAttribute = QualityAttribute.CodeQuality | QualityAttribute.Conformance,
					Snippet = node.ToFullString()
				};
			}

			return null;
		}

		private static IEnumerable<ITypeSymbol> GetReferencedTypes(SyntaxNode classDeclaration, ISymbol sourceSymbol, SemanticModel semanticModel)
		{
			var typeSyntaxes = classDeclaration.DescendantNodesAndSelf().OfType<TypeSyntax>();
			var commonSymbolInfos = typeSyntaxes.Select(selector: x => semanticModel.GetSymbolInfo(expression: x)).AsArray();
			var members = commonSymbolInfos
				.Select(selector: x => x.Symbol)
				.Where(predicate: x => x != null)
				.Select(selector: x =>
				{
					var typeSymbol = x as ITypeSymbol;
					return typeSymbol == null ? x.ContainingType : x;
				})
				.Cast<ITypeSymbol>()
				.WhereNotNull()
				.DistinctBy(func: x => x.ToDisplayString())
				.Where(predicate: x => !SymbolEqualityComparer.Default.Equals(x: x, y: sourceSymbol))
				.AsArray();

			return members;
		}
	}
}