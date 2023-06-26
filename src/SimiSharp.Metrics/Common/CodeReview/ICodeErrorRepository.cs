// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICodeErrorRepository.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ICodeErrorRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimiSharp.CodeAnalysis.Common.CodeReview
{
	public interface ICodeErrorRepository : IDisposable
	{
		Task<IEnumerable<EvaluationResult>> GetErrors(string solutionPath, CancellationToken cancellationToken);
	}
}
