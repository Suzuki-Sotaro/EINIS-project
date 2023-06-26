// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberDocumentation.cs" company="Reimers.dk">
//   Copyright � 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberDocumentation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.Metrics;

namespace SimiSharp.CodeAnalysis.Metrics
{
	internal class MemberDocumentation : IMemberDocumentation
	{
		public MemberDocumentation(string summary, string code, string example, string remarks, string returns, IEnumerable<TypeParameterDocumentation> typeParameters, IEnumerable<ParameterDocumentation> parameters, IEnumerable<ExceptionDocumentation> exceptions)
		{
			Summary = summary;
			Code = code;
			Example = example;
			Remarks = remarks;
			Returns = returns;
			TypeParameters = typeParameters.AsArray();
			Parameters = parameters.AsArray();
			Exceptions = exceptions.AsArray();
		}

		public string Summary { get; }

		public string Code { get; }

		public string Example { get; }

		public string Remarks { get; }

		public string Returns { get; }

		public IEnumerable<TypeParameterDocumentation> TypeParameters { get; }

		public IEnumerable<ParameterDocumentation> Parameters { get; }

		public IEnumerable<ExceptionDocumentation> Exceptions { get; }
	}
}
