// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorInParameterErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceLocatorInParameterErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class ServiceLocatorInParameterErrorRule : CodeEvaluationBase
	{
		public override string ID => "AM0039";

		public override SyntaxKind EvaluatedKind => SyntaxKind.Parameter;

		public override string Title => "ServiceLocator Passed as Parameter";

		public override string Suggestion => "Remove ServiceLocator parameter and inject only needed dependencies.";

		public override CodeQuality Quality => CodeQuality.Broken;

		public override QualityAttribute QualityAttribute => QualityAttribute.Maintainability | QualityAttribute.Modifiability | QualityAttribute.Reusability | QualityAttribute.Testability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var parameterSyntax = (ParameterSyntax)node;
			if (parameterSyntax.Type != null
				&& parameterSyntax.Type.IsKind(kind: SyntaxKind.IdentifierName)
				&& ((IdentifierNameSyntax)parameterSyntax.Type).Identifier.ValueText.Contains(value: "ServiceLocator"))
			{
				var parentMethod = FindMethodParent(node: parameterSyntax);
				var snippet = parentMethod == null
								  ? parameterSyntax.Parent.Parent.ToFullString()
								  : parentMethod is ConstructorDeclarationSyntax
										? FindClassParent(node: parameterSyntax).ToFullString()
										: parentMethod.ToFullString();

				return new EvaluationResult
				{
					Snippet = snippet
				};
			}

			return null;
		}
	}
}
