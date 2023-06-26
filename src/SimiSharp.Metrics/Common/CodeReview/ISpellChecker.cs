// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISpellChecker.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ISpellChecker type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace SimiSharp.CodeAnalysis.Common.CodeReview
{
	public interface ISpellChecker : IDisposable
	{
		bool Spell(string word);
	}
}
