// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INodeInspector.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the INodeInspector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace SimiSharp.CodeAnalysis.Common.CodeReview
{
    public interface INodeInspector
    {
        Task<IEnumerable<EvaluationResult>> Inspect(Solution solution, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<EvaluationResult>> Inspect(string projectPath, string projectName, SyntaxNode node, SemanticModel semanticModel, Solution solution);
    }
}
