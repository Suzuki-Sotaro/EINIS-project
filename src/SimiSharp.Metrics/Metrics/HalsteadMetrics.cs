// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HalsteadMetrics.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the HalsteadMetrics type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using SimiSharp.CodeAnalysis.Common.Metrics;

namespace SimiSharp.CodeAnalysis.Metrics
{
	internal sealed class HalsteadMetrics : IHalsteadMetrics
	{
		public static readonly IHalsteadMetrics GenericInstanceGetPropertyMetrics;
		public static readonly IHalsteadMetrics GenericInstanceSetPropertyMetrics;
		public static readonly IHalsteadMetrics GenericStaticGetPropertyMetrics;
		public static readonly IHalsteadMetrics GenericStaticSetPropertyMetrics;

		static HalsteadMetrics()
		{
			GenericInstanceSetPropertyMetrics = new HalsteadMetrics(numOperands: 5, numOperators: 3, numUniqueOperands: 4, numUniqueOperators: 3);
			GenericStaticSetPropertyMetrics = new HalsteadMetrics(numOperands: 4, numOperators: 3, numUniqueOperands: 3, numUniqueOperators: 3);
			GenericInstanceGetPropertyMetrics = new HalsteadMetrics(numOperands: 3, numOperators: 2, numUniqueOperands: 3, numUniqueOperators: 2);
			GenericStaticGetPropertyMetrics = new HalsteadMetrics(numOperands: 2, numOperators: 1, numUniqueOperands: 2, numUniqueOperators: 1);
		}

		public HalsteadMetrics(int numOperands, int numOperators, int numUniqueOperands, int numUniqueOperators)
		{
			NumberOfOperands = numOperands;
			NumberOfOperators = numOperators;
			NumberOfUniqueOperands = numUniqueOperands;
			NumberOfUniqueOperators = numUniqueOperators;
		}

		public int NumberOfOperands { get; }

		public int NumberOfOperators { get; }

		public int NumberOfUniqueOperands { get; }

		public int NumberOfUniqueOperators { get; }

		public IHalsteadMetrics Merge(IHalsteadMetrics other)
		{
			if (other == null)
			{
				return this;
			}

			return new HalsteadMetrics(
				numOperands: NumberOfOperands + other.NumberOfOperands, 
				numOperators: NumberOfOperators + other.NumberOfOperators, 
				numUniqueOperands: NumberOfUniqueOperands + other.NumberOfUniqueOperands, 
				numUniqueOperators: NumberOfUniqueOperators + other.NumberOfUniqueOperators);
		}

		public int GetBugs()
		{
			var volume = GetVolume();

			return (int)(volume / 3000);
		}

		public double GetDifficulty()
		{
			return NumberOfUniqueOperands == 0
				? 0
				: ((NumberOfUniqueOperators / 2.0) * (NumberOfOperands / ((double)NumberOfUniqueOperands)));
		}

		public TimeSpan GetEffort()
		{
			var effort = GetDifficulty() * GetVolume();
			return TimeSpan.FromSeconds(value: effort / 18.0);
		}

		public int GetLength()
		{
			return NumberOfOperators + NumberOfOperands;
		}

		public int GetVocabulary()
		{
			return NumberOfUniqueOperators + NumberOfUniqueOperands;
		}

		public double GetVolume()
		{
			const double newBase = 2.0;
			double vocabulary = GetVocabulary();
			double length = GetLength();
			if (vocabulary.Equals(obj: 0.0))
			{
				return 0.0;
			}

			return length * Math.Log(a: vocabulary, newBase: newBase);
		}
	}
}
