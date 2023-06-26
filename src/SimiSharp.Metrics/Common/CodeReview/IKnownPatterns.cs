// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IKnownPatterns.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IKnownPatterns type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace SimiSharp.CodeAnalysis.Common.CodeReview
{
	public interface IKnownPatterns : IEnumerable<string>
	{
		bool IsExempt(string word);

		void Add(params string[] patterns);

		void Remove(string pattern);

		void Clear();
	}
}
