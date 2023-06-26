// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMemberDocumentation.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IMemberDocumentation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace SimiSharp.CodeAnalysis.Common.Metrics
{
	public interface IMemberDocumentation : IDocumentation
	{
		IEnumerable<ParameterDocumentation> Parameters { get; }

		IEnumerable<TypeParameterDocumentation> TypeParameters { get; }

		IEnumerable<ExceptionDocumentation> Exceptions { get; }
	}
}