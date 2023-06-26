// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoNotImplementedExceptionRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NoNotImplementedExceptionRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class NoNotImplementedExceptionRule : CodeEvaluationBase
	{
		public override string ID => "AM0030";

		public override SyntaxKind EvaluatedKind => SyntaxKind.ThrowStatement;

		public override string Title => "NotImplementedException Thrown";

		public override string Suggestion => "Add method implementation.";

		public override CodeQuality Quality => CodeQuality.Broken;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var statement = (ThrowStatementSyntax)node;
			var exceptionCreation = statement.Expression as ObjectCreationExpressionSyntax;
			if (exceptionCreation != null)
			{
				var exceptionType = exceptionCreation.Type as IdentifierNameSyntax;
				if (exceptionType != null && exceptionType.Identifier.ValueText.EndsWith(value: "NotImplementedException"))
				{
					return new EvaluationResult
					{
						Snippet = node.ToFullString()
					};
				}
			}

			return null;
		}
	}
}