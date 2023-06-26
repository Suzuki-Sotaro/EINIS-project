// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooManyMethodParametersRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TooManyMethodParametersRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class TooManyMethodParametersRule : CodeEvaluationBase
	{
		private const int Limit = 5;

		public override string ID => "AM0047";

		public override SyntaxKind EvaluatedKind => SyntaxKind.MethodDeclaration;

		public override string Title => "More than " + Limit + " parameters on method";

		public override string Suggestion => "Refactor method to reduce number of dependencies passed.";

		public override CodeQuality Quality => CodeQuality.NeedsRefactoring;

		public override QualityAttribute QualityAttribute => QualityAttribute.Testability | QualityAttribute.Maintainability | QualityAttribute.Modifiability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var methodDeclaration = (MethodDeclarationSyntax)node;
			var parameterCount = methodDeclaration.ParameterList.Parameters.Count;

			if (parameterCount >= Limit)
			{
				return new EvaluationResult
						   {
							   ErrorCount = parameterCount, 
							   Snippet = methodDeclaration.ToFullString()
						   };
			}

			return null;
		}
	}
}
