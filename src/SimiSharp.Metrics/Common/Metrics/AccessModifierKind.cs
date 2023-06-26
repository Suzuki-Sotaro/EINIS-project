// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessModifierKind.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the AccessModifierKind type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace SimiSharp.CodeAnalysis.Common.Metrics
{
	[Flags]
	public enum AccessModifierKind
	{
		Private = 1, 
		Protected = 2, 
		Public = 4, 
		Internal = 8
	}
}