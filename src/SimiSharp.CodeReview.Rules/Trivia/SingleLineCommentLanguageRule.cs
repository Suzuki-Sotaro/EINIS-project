// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleLineCommentLanguageRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SingleLineCommentLanguageRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Trivia
{
	internal class SingleLineCommentLanguageRule : CommentLanguageRuleBase
	{
		public SingleLineCommentLanguageRule(ISpellChecker spellChecker)
			: base(spellChecker: spellChecker)
		{
		}

		public override string ID => "AM0068";

		public override string Title => "Suspicious Language Single Line Comment";

		public override SyntaxKind EvaluatedKind => SyntaxKind.SingleLineCommentTrivia;
	}
}
