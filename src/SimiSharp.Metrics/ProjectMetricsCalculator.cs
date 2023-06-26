// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.Metrics;
using SimiSharp.CodeAnalysis.Metrics;

namespace SimiSharp.CodeAnalysis
{
    public class ProjectMetricsCalculator : IProjectMetricsCalculator
    {
        private readonly ICodeMetricsCalculator _metricsCalculator;

        public ProjectMetricsCalculator(ICodeMetricsCalculator metricsCalculator)
        {
            _metricsCalculator = metricsCalculator;
        }

        public async Task<IEnumerable<IProjectMetric>> Calculate(Solution solution)
        {
            var tasks = (from project in solution.Projects
                         where project != null
                         let compilation = project.GetCompilationAsync()
                         select new { project, compilation })
                        .AsArray();

            await Task.WhenAll(tasks: tasks.Select(selector: x => x.compilation)).ConfigureAwait(continueOnCapturedContext: false);

            var calculationTasks = tasks.Select(selector: x => InnerCalculate(project: x.project, compilationTask: x.compilation, solution: solution));

            return await Task.WhenAll(tasks: calculationTasks).ConfigureAwait(continueOnCapturedContext: false);
        }

        public Task<IProjectMetric> Calculate(Project project, Solution solution)
        {
            if (project == null)
            {
                return null;
            }

            var compilation = project.GetCompilationAsync();
            return InnerCalculate(project: project, compilationTask: compilation, solution: solution);
        }

        private async Task<IProjectMetric> InnerCalculate(Project project, Task<Compilation> compilationTask, Solution solution)
        {
            if (project == null)
            {
                return null;
            }

            var compilation = await compilationTask.ConfigureAwait(continueOnCapturedContext: false);
            var metricsTask = _metricsCalculator.Calculate(project: project, solution: solution);

            IEnumerable<string> dependencies;
            if (solution != null)
            {
                var dependencyGraph = solution.GetProjectDependencyGraph();

                dependencies = dependencyGraph.GetProjectsThatThisProjectTransitivelyDependsOn(projectId: project.Id)
                    .Select(selector: solution.GetProject)
                    .SelectMany(selector: x => x.MetadataReferences.Select(selector: y => y.Display).Concat(second: new[] { x.AssemblyName }));
            }
            else
            {
                dependencies = project.AllProjectReferences.SelectMany(selector: x => x.Aliases)
                    .Concat(second: project.MetadataReferences.Select(selector: y => y.Display));
            }

            var assemblyTypes = compilation.Assembly.TypeNames;
            var metrics = (await metricsTask.ConfigureAwait(continueOnCapturedContext: false)).AsArray();

            var internalTypesUsed = from metric in metrics
                                    from coupling in metric.Dependencies
                                    where coupling.Assembly == project.AssemblyName
                                    select coupling;

            var relationalCohesion = (internalTypesUsed.Count() + 1.0) / assemblyTypes.Count;

            return new ProjectMetric(name: project.Name, namespaceMetrics: metrics, referencedProjects: dependencies, relationalCohesion: relationalCohesion);
        }
    }
}