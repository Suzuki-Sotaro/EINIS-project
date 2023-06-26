// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NameSpellingRuleBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NameSpellingRuleBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal abstract class NameSpellingRuleBase : CodeEvaluationBase
	{
		private readonly ISpellChecker _speller;

		protected NameSpellingRuleBase(ISpellChecker speller)
		{
			_speller = speller;
		}

		public override CodeQuality Quality => CodeQuality.NeedsReview;

		public override QualityAttribute QualityAttribute => QualityAttribute.Conformance;

		public override ImpactLevel ImpactLevel => ImpactLevel.Node;

		protected bool IsSpelledCorrectly(string name)
		{
			return name.ToTitleCase()
				.Split(separator: new[] { " " }, options: StringSplitOptions.RemoveEmptyEntries)
				.Aggregate(seed: true, func: (b, s) => b && _speller.Spell(word: s));
		}
	}
}
