// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommentLanguageRuleBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CommentLanguageRuleBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Trivia
{
	internal abstract class CommentLanguageRuleBase : TriviaEvaluationBase
	{
		private static readonly Regex StrippedRegex = new Regex(pattern: @"[""'*©®º()!%\[\]{}/]+", options: RegexOptions.Compiled);
		private static readonly Regex LineDashRegex = new Regex(pattern: @"-{3,}", options: RegexOptions.Compiled);
		private static readonly Regex NumberRegex = new Regex(pattern: "[1-9]+", options: RegexOptions.Compiled);
		private static readonly Regex XmlRegex = new Regex(pattern: "<.+?>", options: RegexOptions.Compiled);
		private readonly ISpellChecker _spellChecker;

		protected CommentLanguageRuleBase(ISpellChecker spellChecker)
		{
			_spellChecker = spellChecker;
		}

		public override string Suggestion => "Check spelling of comment.";

		public override CodeQuality Quality => CodeQuality.NeedsReview;

		public override QualityAttribute QualityAttribute => QualityAttribute.Maintainability | QualityAttribute.Conformance;

		public override ImpactLevel ImpactLevel => ImpactLevel.Member;

		protected override EvaluationResult EvaluateImpl(SyntaxTrivia node)
		{
			var trimmed = StrippedRegex.Replace(input: node.ToFullString(), replacement: string.Empty).Trim();
			var commentWords = RemoveLineDashes(input: RemoveXml(input: trimmed))
				.Split(separator: ' ')
				.Select(selector: RemoveXml)
				.Select(selector: s => s.TrimEnd('.', ',', '_'))
				.Where(predicate: IsNotNumber)
				.AsArray();
			var errorCount = commentWords.Aggregate(seed: 0, func: (i, s) => i + (_spellChecker.Spell(word: s) ? 0 : 1));
			if (errorCount >= 0.50 * commentWords.Length)
			{
				return new EvaluationResult
						   {
							   Snippet = node.ToFullString()
						   };
			}

			return null;
		}

		private bool IsNotNumber(string input)
		{
			return !NumberRegex.IsMatch(input: input);
		}

		private string RemoveLineDashes(string input)
		{
			return LineDashRegex.Replace(input: input, replacement: string.Empty);
		}

		private string RemoveXml(string input)
		{
			return XmlRegex.Replace(input: input, replacement: string.Empty);
		}
	}
}
