// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.Metrics;

namespace SimiSharp.CodeAnalysis.Metrics
{
	internal sealed class NamespaceMetricsCalculator : SemanticModelMetricsCalculator
	{
		public NamespaceMetricsCalculator(SemanticModel semanticModel)
			: base(semanticModel: semanticModel)
		{
		}

		public INamespaceMetric CalculateFrom(NamespaceDeclarationSyntaxInfo namespaceNode, IEnumerable<ITypeMetric> metrics)
		{
			const string documentationTypeName = "NamespaceDoc";
			var typeMetrics = metrics.AsArray();
			var documentationType = typeMetrics.FirstOrDefault(predicate: x => x.Name == documentationTypeName);
			IDocumentation documentation = null;
			if (documentationType != null)
			{
				documentation = documentationType.Documentation;
				typeMetrics = typeMetrics.Where(predicate: x => x.Name != documentationTypeName).AsArray();
			}

			var linesOfCode = typeMetrics.Sum(selector: x => x.LinesOfCode);
			var source = typeMetrics.SelectMany(selector: x => x.Dependencies)
						  .GroupBy(keySelector: x => x.ToString())
						  .Select(selector: x => new TypeCoupling(typeName: x.First().TypeName, namespaceName: x.First().Namespace, assemblyName: x.First().Assembly, usedMethods: x.SelectMany(selector: y => y.UsedMethods), usedProperties: x.SelectMany(selector: y => y.UsedProperties), useEvents: x.SelectMany(selector: y => y.UsedEvents)))
						  .Where(predicate: x => x.Namespace != namespaceNode.Name)
						  .OrderBy(keySelector: x => x.Assembly + x.Namespace + x.TypeName)
						  .AsArray();
			var maintainabilitySource = typeMetrics.Select(selector: x => new Tuple<int, double>(item1: x.LinesOfCode, item2: x.MaintainabilityIndex)).AsArray();
			var maintainabilityIndex = linesOfCode > 0 && maintainabilitySource.Any() ? maintainabilitySource.Sum(selector: x => x.Item1 * x.Item2) / linesOfCode : 100.0;
			var cyclomaticComplexity = typeMetrics.Sum(selector: x => x.CyclomaticComplexity);
			var depthOfInheritance = typeMetrics.Any() ? typeMetrics.Max(selector: x => x.DepthOfInheritance) : 0;
			return new NamespaceMetric(
				maintainabilityIndex: maintainabilityIndex,
				cyclomaticComplexity: cyclomaticComplexity,
				linesOfCode: linesOfCode,
				classCouplings: source,
				depthOfInheritance: depthOfInheritance,
				name: namespaceNode.Name,
				typeMetrics: typeMetrics,
				documentation: documentation);
		}
	}
}
