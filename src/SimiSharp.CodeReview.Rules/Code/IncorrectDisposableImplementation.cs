// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IncorrectDisposableImplementation.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IncorrectDisposableImplementation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class IncorrectDisposableImplementation : CodeEvaluationBase
	{
		public override string ID => "AM0018";

		public override SyntaxKind EvaluatedKind => SyntaxKind.ClassDeclaration;

		public override string Title => "Incorrect Dispose pattern implementation";

		public override string Suggestion => "Implement dispose pattern with finalizer and separate disposal of managed and unmanaged resources.";

		public override CodeQuality Quality => CodeQuality.NeedsReview;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality | QualityAttribute.Conformance;

		public override ImpactLevel ImpactLevel => ImpactLevel.Type;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var classDeclaration = (ClassDeclarationSyntax)node;
			if (classDeclaration.BaseList != null && classDeclaration.BaseList.Types.Any(predicate: t => (t.Type is IdentifierNameSyntax) && ((IdentifierNameSyntax)t.Type).Identifier.ValueText.Contains(value: "IDisposable")))
			{
				var methods = classDeclaration.ChildNodes().OfType<MethodDeclarationSyntax>()
					.Where(predicate: m => m.Identifier.ValueText == "Dispose")
					.Where(predicate: m =>
						{
							var predefinedType = SyntaxFactory.PredefinedType(keyword: SyntaxFactory.Token(kind: SyntaxKind.BoolKeyword));
							return m.ParameterList.Parameters.Count == 0
										   || (m.ParameterList.Parameters.Count == 1 && m.ParameterList.Parameters[index: 0].Type.EquivalentTo(node2: predefinedType));
						}).AsArray();
				var destructor = classDeclaration
					.ChildNodes()
					.OfType<DestructorDeclarationSyntax>()
					.FirstOrDefault(predicate: d => d.Body.ChildNodes().Any(predicate: InvokesDispose));
				if (methods.Length < 2 || destructor == null)
				{
					return new EvaluationResult
					{
						Snippet = node.ToFullString()
					};
				}
			}

			return null;
		}

		private bool InvokesDispose(SyntaxNode node)
		{
			var expression = node as ExpressionStatementSyntax;
			if (expression != null)
			{
				var invocation = expression.Expression as InvocationExpressionSyntax;
				if (invocation != null)
				{
					var identifier = invocation.Expression as IdentifierNameSyntax;
					if (identifier != null
						&& identifier.Identifier.ValueText == "Dispose"
						&& invocation.ArgumentList != null
						&& invocation.ArgumentList.Arguments.Count == 1
						&& invocation.ArgumentList.Arguments[index: 0].EquivalentTo(node2: SyntaxFactory.Argument(expression: SyntaxFactory.LiteralExpression(kind: SyntaxKind.FalseLiteralExpression, token: SyntaxFactory.Token(kind: SyntaxKind.FalseKeyword)))))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}