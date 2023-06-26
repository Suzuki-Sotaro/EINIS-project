// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodTooDeepNestingRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MethodTooDeepNestingRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class MethodTooDeepNestingRule : TooDeepNestingRuleBase
	{
		public override string ID => "AM0026";

		public override SyntaxKind EvaluatedKind => SyntaxKind.MethodDeclaration;

		protected override string NestingMember => "Method";

		protected override BlockSyntax GetBody(SyntaxNode node)
		{
			var member = (MethodDeclarationSyntax)node;
			return member.Body;
		}
	}
}