// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectMetric.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectMetric type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.Metrics;

namespace SimiSharp.CodeAnalysis.Metrics
{
    internal class ProjectMetric : IProjectMetric
    {
        private static readonly IEqualityComparer<ITypeCoupling> Comparer = new ComparableComparer<ITypeCoupling>();

        public ProjectMetric(string name, IEnumerable<INamespaceMetric> namespaceMetrics, IEnumerable<string> referencedProjects, double relationalCohesion)
        {
            Name = name;
            RelationalCohesion = relationalCohesion;
            AssemblyDependencies = referencedProjects.AsArray();
            EfferentCoupling = AssemblyDependencies.Count();
            NamespaceMetrics = namespaceMetrics.AsArray();
            LinesOfCode = NamespaceMetrics.Sum(selector: x => x.LinesOfCode);
            MaintainabilityIndex = LinesOfCode == 0 ? 100 : NamespaceMetrics.Sum(selector: x => x.MaintainabilityIndex * x.LinesOfCode) / LinesOfCode;
            CyclomaticComplexity = LinesOfCode == 0 ? 0 : NamespaceMetrics.Sum(selector: x => x.CyclomaticComplexity * x.LinesOfCode) / LinesOfCode;
            Dependencies = NamespaceMetrics.SelectMany(selector: x => x.Dependencies).Where(predicate: x => x.Assembly != Name).Distinct(comparer: Comparer).AsArray();
            Dependants = Dependencies.Select(selector: x => x.Assembly)
                .Distinct()
                .AsArray();
            AfferentCoupling = Dependants.Count();
            var typeMetrics = NamespaceMetrics.SelectMany(selector: x => x.TypeMetrics).AsArray();
            Abstractness = typeMetrics.Count(predicate: x => x.IsAbstract) / (double)typeMetrics.Length;
        }

        public IEnumerable<string> AssemblyDependencies { get; }

        public double Abstractness { get; }

        public int EfferentCoupling { get; }

        public int AfferentCoupling { get; }

        public int LinesOfCode { get; }

        public double MaintainabilityIndex { get; }

        public int CyclomaticComplexity { get; }

        public string Name { get; }

        public double RelationalCohesion { get; }

        public IEnumerable<ITypeCoupling> Dependencies { get; }

        public IEnumerable<string> Dependants { get; }

        public IEnumerable<INamespaceMetric> NamespaceMetrics { get; }
    }
}