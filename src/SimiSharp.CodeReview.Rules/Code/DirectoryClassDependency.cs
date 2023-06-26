// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryClassDependency.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DirectoryClassDependency type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class DirectoryClassDependency : CodeEvaluationBase
	{
		public override string ID => "AM0003";

		public override SyntaxKind EvaluatedKind => SyntaxKind.SimpleMemberAccessExpression;

		public override string Title => "Directory Class Dependency";

		public override string Suggestion => "Consider breaking the direct dependency on the file system with an abstraction.";

		public override CodeQuality Quality => CodeQuality.NeedsRefactoring;

		public override QualityAttribute QualityAttribute => QualityAttribute.Modifiability | QualityAttribute.Testability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Type;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var memberAccess = (MemberAccessExpressionSyntax)node;
			if (memberAccess.Expression.IsKind(kind: SyntaxKind.IdentifierName)
			    && ((IdentifierNameSyntax)memberAccess.Expression).Identifier.ValueText == "Directory")
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
