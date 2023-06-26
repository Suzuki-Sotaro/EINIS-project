// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeObfuscationRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeObfuscationRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class TypeObfuscationRule : CodeEvaluationBase
	{
		public override string ID => "AM0049";

		public override SyntaxKind EvaluatedKind => SyntaxKind.LocalDeclarationStatement;

		public override string Title => "Type Obfuscation";

		public override string Suggestion => "Assigning a value to a variable of type object bypasses type checking.";

		public override CodeQuality Quality => CodeQuality.NeedsRefactoring;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var declaration = ((LocalDeclarationStatementSyntax)node).Declaration;

			var declarationString = declaration.Type.ToFullString().Trim();
			var objectString = SyntaxFactory.PredefinedType(keyword: SyntaxFactory.Token(kind: SyntaxKind.ObjectKeyword)).ToFullString().Trim();
			if (declarationString.Equals(value: objectString)
				&& declaration.Variables.Any(predicate: v => v.Initializer == null || v.Initializer.Value.IsKind(kind: SyntaxKind.NullLiteralExpression)))
			{
				return new EvaluationResult
						   {
							   Snippet = (FindMethodParent(node: node) ?? FindClassParent(node: node)).ToFullString()
						   };
			}

			return null;
		}
	}
}
