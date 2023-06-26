// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterDocumentation.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ParameterDocumentation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimiSharp.CodeAnalysis.Common.Metrics
{
	public class ParameterDocumentation
	{
		public ParameterDocumentation(string parameterName, string parameterType, string description)
		{
			ParameterName = parameterName;
			ParameterType = parameterType;
			Description = description;
		}

		public string ParameterName { get; }

		public string ParameterType { get; }

		public string Description { get; }
	}
}