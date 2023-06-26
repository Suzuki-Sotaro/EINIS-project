// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnusedMethodRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnusedMethodRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp;

namespace SimiSharp.CodeReview.Rules.Semantic
{
	internal class UnusedMethodRule : UnusedCodeRule
	{
		public override string ID => "AM0065";

		public override SyntaxKind EvaluatedKind => SyntaxKind.MethodDeclaration;
	}
}