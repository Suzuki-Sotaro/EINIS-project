// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BeginEndPairRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the BeginEndPairRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class BeginEndPairRule : MethodNamePairRule
	{
		public override string ID => "AM0036";

		public override string Title => "Begin/End Method Pair";

		public override string Suggestion => "Methods names BeginSomething should have a matching EndSomething and vice versa.";

		protected override string BeginToken => "Begin";

		protected override string PairToken => "End";
	}
}
