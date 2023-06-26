// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyNameSpellingRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the PropertyNameSpellingRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class PropertyNameSpellingRule : NameSpellingRuleBase
	{
		public PropertyNameSpellingRule(ISpellChecker speller)
			: base(speller: speller)
		{
		}

		public override string ID => "AM0025";

		public override SyntaxKind EvaluatedKind => SyntaxKind.PropertyDeclaration;

		public override string Title => "Property Name Spelling";

		public override string Suggestion => "Check that the property name is spelled correctly. Consider adding exceptions to the dictionary.";

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var propertyDeclaration = (PropertyDeclarationSyntax)node;
			var propertyName = propertyDeclaration.Identifier.ValueText;

			var correct = IsSpelledCorrectly(name: propertyName);
			if (!correct)
			{
				return new EvaluationResult
					   {
						   Quality = CodeQuality.NeedsReview, 
						   ImpactLevel = ImpactLevel.Node, 
						   QualityAttribute = QualityAttribute.Conformance, 
						   Snippet = propertyName, 
						   ErrorCount = 1
					   };
			}

			return null;
		}
	}
}
