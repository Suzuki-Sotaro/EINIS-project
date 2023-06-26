// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodNamePairRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MethodNamePairRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal abstract class MethodNamePairRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind => SyntaxKind.MethodDeclaration;

		public override CodeQuality Quality => CodeQuality.NeedsRefactoring;

		public override QualityAttribute QualityAttribute => QualityAttribute.Conformance;

		public override ImpactLevel ImpactLevel => ImpactLevel.Type;

		protected abstract string BeginToken { get; }

		protected abstract string PairToken { get; }

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var method = (MethodDeclarationSyntax)node;
			if (!HasMatchingMethod(start: BeginToken, match: PairToken, method: method))
			{
				return new EvaluationResult
						   {
							   Snippet = method.ToFullString()
						   };
			}

			return null;
		}

		private bool HasMatchingMethod(string start, string match, MethodDeclarationSyntax method)
		{
			var methodName = method.Identifier.ValueText;
			if (methodName.StartsWith(value: start, comparisonType: StringComparison.InvariantCultureIgnoreCase))
			{
				var pairMethodName = Regex.Replace(input: methodName, pattern: "^" + start, replacement: match);
				var parentClass = FindClassParent(node: method);
				if (parentClass == null)
				{
					return true;
				}

				return parentClass
					.ChildNodes()
					.OfType<MethodDeclarationSyntax>()
					.Any(predicate: m => m.Identifier.ValueText == pairMethodName);
			}

			return true;
		}
	}
}
