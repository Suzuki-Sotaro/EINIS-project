// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionDocumentation.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ExceptionDocumentation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimiSharp.CodeAnalysis.Common.Metrics
{
	public class ExceptionDocumentation
	{
		public ExceptionDocumentation(string exceptionType, string description)
		{
			ExceptionType = exceptionType;
			Description = description;
		}

		public string ExceptionType { get; }

		public string Description { get; }
	}
}