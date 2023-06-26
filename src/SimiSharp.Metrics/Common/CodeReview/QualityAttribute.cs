// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualityAttribute.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the QualityAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace SimiSharp.CodeAnalysis.Common.CodeReview
{
	[Flags]
	public enum QualityAttribute
	{
		CodeQuality = 1, 
		Maintainability = 2, 
		Testability = 4, 
		Modifiability = 8, 
		Reusability = 16, 
		Conformance = 32, 
		Security = 64,
		Performance = 128
	}
}
