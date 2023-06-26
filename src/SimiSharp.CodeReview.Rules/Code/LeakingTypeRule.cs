// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LeakingTypeRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LeakingTypeRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal abstract class LeakingTypeRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind => SyntaxKind.PropertyDeclaration;

		public override string Title => "Current Type Exposes " + TypeIdentifier;

		public override string Suggestion => "Remove public access to " + TypeIdentifier;

		public override CodeQuality Quality => CodeQuality.Broken;

		public override QualityAttribute QualityAttribute => QualityAttribute.Modifiability | QualityAttribute.Reusability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected abstract string TypeIdentifier { get; }

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var parentClass = FindClassParent(node: node);
			if (parentClass != null && parentClass.Modifiers.Any(kind: SyntaxKind.PublicKeyword))
			{
				var propertyDeclaration = (PropertyDeclarationSyntax)node;
				if (propertyDeclaration.Modifiers.Any(kind: SyntaxKind.PublicKeyword))
				{
					var type = propertyDeclaration.Type as SimpleNameSyntax;
					if (type != null && type.Identifier.ValueText != null
						&& type.Identifier.ValueText.EndsWith(value: TypeIdentifier ?? string.Empty, comparisonType: StringComparison.InvariantCultureIgnoreCase))
					{
						return new EvaluationResult
								   {
									   Snippet = parentClass.ToFullString()
								   };
					}
				}
			}

			return null;
		}
	}
}
