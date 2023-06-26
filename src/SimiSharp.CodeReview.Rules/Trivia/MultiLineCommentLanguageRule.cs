// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiLineCommentLanguageRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MultiLineCommentLanguageRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Trivia
{
	internal class MultiLineCommentLanguageRule : CommentLanguageRuleBase
	{
		public MultiLineCommentLanguageRule(ISpellChecker spellChecker)
			: base(spellChecker: spellChecker)
		{
		}

		public override string ID => "AM0067";

		public override string Title => "Suspicious Language Multi Line Comment";

		public override SyntaxKind EvaluatedKind => SyntaxKind.MultiLineCommentTrivia;
	}
}
