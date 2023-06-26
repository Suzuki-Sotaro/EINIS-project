// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileClassDependency.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the FileClassDependency type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class FileClassDependency : CodeEvaluationBase
	{
		public override string ID => "AM0011";

		public override SyntaxKind EvaluatedKind => SyntaxKind.SimpleMemberAccessExpression;

		public override string Title => "File Class Dependency";

		public override string Suggestion => "Replace explicit file dependency to reduce coupling with file system.";

		public override CodeQuality Quality => CodeQuality.Broken;

		public override QualityAttribute QualityAttribute => QualityAttribute.Modifiability | QualityAttribute.Testability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Type;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var memberAccess = (MemberAccessExpressionSyntax)node;
			if (memberAccess.Expression.IsKind(kind: SyntaxKind.IdentifierName)
				&& ((IdentifierNameSyntax)memberAccess.Expression).Identifier.ValueText == "File")
			{
				var methodParent = FindMethodParent(node: node);
				var snippet = methodParent == null
								  ? FindClassParent(node: node).ToFullString()
								  : methodParent.ToFullString();

				return new EvaluationResult
						   {
							   Snippet = snippet
						   };
			}

			return null;
		}
	}
}
