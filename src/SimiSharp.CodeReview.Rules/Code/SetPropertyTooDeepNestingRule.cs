// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetPropertyTooDeepNestingRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SetPropertyTooDeepNestingRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class SetPropertyTooDeepNestingRule : PropertyTooDeepNestingRule
	{
		public override string ID => "AM0013";

		public override SyntaxKind EvaluatedKind => SyntaxKind.SetAccessorDeclaration;

		protected override string NestingMember => "Property Setter";
	}
}