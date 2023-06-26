// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PossibleStaticMethod.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the PossibleStaticMethod type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Semantic
{
	internal class PossibleStaticMethod : SemanticEvaluationBase
	{
		public override string ID => "AM0057";

		public override SyntaxKind EvaluatedKind => SyntaxKind.MethodDeclaration;

		public override string Title => "Method Can Be Made Static";

		public override string Suggestion => "Mark method as static.";

		public override CodeQuality Quality => CodeQuality.NeedsReview;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override Task<EvaluationResult> EvaluateImpl(
			SyntaxNode node, 
			SemanticModel semanticModel, 
			Solution solution)
		{
			var method = (MethodDeclarationSyntax)node;
			var analyzer = new SemanticAnalyzer(model: semanticModel);

			if (analyzer.CanBeMadeStatic(method: method))
			{
				var snippet = method.ToFullString();
				return Task.FromResult(
					result: new EvaluationResult
					{
						Snippet = snippet
					});
			}

			return Task.FromResult(result: (EvaluationResult)null);
		}
	}
}