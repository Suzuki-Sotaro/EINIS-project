// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberMetric.cs" company="Reimers.dk">
//   Copyright � 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberMetric type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.Metrics;

namespace SimiSharp.CodeAnalysis.Metrics
{
	internal class MemberMetric : IMemberMetric
	{
		private readonly IHalsteadMetrics _halstead;

		public MemberMetric(
			string codeFile,
			AccessModifierKind accessModifier,
			IHalsteadMetrics halstead,
			int lineNumber,
			int linesOfCode,
			double maintainabilityIndex,
			int cyclomaticComplexity,
			string name,
			IEnumerable<ITypeCoupling> classCouplings,
			int numberOfParameters,
			int numberOfLocalVariables,
			int afferentCoupling,
			IMemberDocumentation documentation)
		{
			_halstead = halstead;
			CodeFile = codeFile;
			AccessModifier = accessModifier;
			LineNumber = lineNumber;
			LinesOfCode = linesOfCode;
			MaintainabilityIndex = maintainabilityIndex;
			CyclomaticComplexity = cyclomaticComplexity;
			Name = name;
		    Dependencies = classCouplings.AsArray();
			NumberOfParameters = numberOfParameters;
			NumberOfLocalVariables = numberOfLocalVariables;
			AfferentCoupling = afferentCoupling;
			Documentation = documentation;
		}

		public string CodeFile { get; }

		public AccessModifierKind AccessModifier { get; }

		public int LineNumber { get; }

		public int LinesOfCode { get; }

		public double MaintainabilityIndex { get; }

		public int CyclomaticComplexity { get; }

		public string Name { get; }

		public IEnumerable<ITypeCoupling> Dependencies { get; }

		public int NumberOfParameters { get; }

		public int NumberOfLocalVariables { get; }

		public int AfferentCoupling { get; }

		public IMemberDocumentation Documentation { get; }

        public int ClassCoupling => Dependencies.Count();

	    public IHalsteadMetrics GetHalsteadMetrics()
		{
			return _halstead;
		}

		public double GetVolume()
		{
			return _halstead?.GetVolume() ?? 0d;
		}
	}
}
