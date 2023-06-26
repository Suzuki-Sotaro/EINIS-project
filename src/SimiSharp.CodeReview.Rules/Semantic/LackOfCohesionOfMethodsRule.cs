// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LackOfCohesionOfMethodsRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LackOfCohesionOfMethodsRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Semantic
{
	internal class LackOfCohesionOfMethodsRule : SemanticEvaluationBase
	{
		private static readonly SymbolKind[] MemberKinds =
		{
			SymbolKind.Event, 
			SymbolKind.Method, 
			SymbolKind.Property
		};

		private int _threshold = 6;

		public override string ID => "AM0055";

		public override SyntaxKind EvaluatedKind => SyntaxKind.ClassDeclaration;

		public override string Title => "Lack of Cohesion of Methods";

		public override string Suggestion => "Refactor class into separate classes with single responsibility.";

		public override CodeQuality Quality => CodeQuality.NeedsRefactoring;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality | QualityAttribute.Maintainability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Type;

		public void SetThreshold(int threshold)
		{
			_threshold = threshold;
		}

		protected override async Task<EvaluationResult> EvaluateImpl(SyntaxNode node, SemanticModel semanticModel, Solution solution)
		{
			var classDeclaration = (ClassDeclarationSyntax)node;
			var symbol = (ITypeSymbol)ModelExtensions.GetDeclaredSymbol(semanticModel: semanticModel, declaration: classDeclaration);
			var members = symbol.GetMembers();

			var memberCount = members.Count(predicate: x => MemberKinds.Contains(value: x.Kind));
			if (memberCount < _threshold)
			{
				return null;
			}

			var fields = members.Where(predicate: x => x.Kind == SymbolKind.Field).AsArray();
			var fieldCount = fields.Length;

			if (fieldCount < _threshold)
			{
				return null;
			}

			var references = await Task.WhenAll(tasks: fields.Select(selector: solution.FindReferences)).ConfigureAwait(continueOnCapturedContext: false);
			var sumFieldUsage = (double)references.Sum(selector: r => r.Locations.Count());

			var lcomhs = (memberCount - (sumFieldUsage / fieldCount)) / (memberCount - 1);
			if (lcomhs < 1)
			{
				return null;
			}

			var snippet = node.ToFullString();
			return new EvaluationResult
				   {
					   Snippet = snippet
				   };
		}
	}
}