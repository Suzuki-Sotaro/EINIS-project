// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HiddenTypeDependencyRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the HiddenTypeDependencyRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Semantic
{
	internal class HiddenTypeDependencyRule : SemanticEvaluationBase
	{
		private static readonly string[] SystemAssemblyPrefixes = new[] { "mscorlib", "System", "Microsoft", "PresentationFramework", "Windows" };

		public override string ID => "AM0054";

		public override SyntaxKind EvaluatedKind => SyntaxKind.MethodDeclaration;

		public override string Title => "Hidden Type Dependency in " + EvaluatedKind.ToString().ToTitleCase();

		public override string Suggestion => "Refactor to pass dependencies explicitly.";

		public override CodeQuality Quality => CodeQuality.NeedsRefactoring;

		public override QualityAttribute QualityAttribute => QualityAttribute.Maintainability | QualityAttribute.Modifiability | QualityAttribute.Testability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Project;

		protected override Task<EvaluationResult> EvaluateImpl(SyntaxNode node, SemanticModel semanticModel, Solution solution)
		{
			var methodDeclaration = (MethodDeclarationSyntax)node;
			if (methodDeclaration.Body == null)
			{
				return Task.FromResult<EvaluationResult>(result: null);
			}

			var descendantNodes = methodDeclaration.Body.DescendantNodes().AsArray();
			var genericParameterTypes =
				descendantNodes.OfType<TypeArgumentListSyntax>()
					.SelectMany(selector: x => x.Arguments.Select(selector: y => semanticModel.GetSymbolInfo(expression: y).Symbol));
			var symbolInfo = semanticModel.GetDeclaredSymbol(declaration: node);
			var containingType = symbolInfo.ContainingType;
			var fieldTypes = containingType.GetMembers()
				.OfType<IFieldSymbol>()
				.Select(selector: x => x.Type)
				.AsArray();
			var usedTypes = genericParameterTypes.Concat(second: fieldTypes)
				.WhereNotNull()
				.DistinctBy(func: x => x.ToDisplayString());
			var parameterTypes =
				methodDeclaration.ParameterList.Parameters.Select(selector: x => semanticModel.GetSymbolInfo(expression: x.Type).Symbol)
					.Concat(second: new[] { semanticModel.GetSymbolInfo(expression: methodDeclaration.ReturnType).Symbol })
					.WhereNotNull()
					.DistinctBy(func: x => x.ToDisplayString())
					.AsArray();

			var parameterAssemblies = parameterTypes.Select(selector: x => x.ContainingAssembly).AsArray();

			var locals = usedTypes.Except(second: parameterTypes);
			if (locals.Any(predicate: x =>
				x.ContainingAssembly == null ||
				(!SymbolEqualityComparer.Default.Equals(x: x.ContainingAssembly, y: semanticModel.Compilation.Assembly)
				&& !parameterAssemblies.Contains(value: x.ContainingAssembly)
				&& !SystemAssemblyPrefixes.Any(predicate: y => x.ContainingAssembly.Name.StartsWith(value: y)))))
			{
				return Task.FromResult(
					result: new EvaluationResult
					{
						Snippet = node.ToFullString()
					});
			}

			return Task.FromResult(result: (EvaluationResult)null);
		}
	}
}