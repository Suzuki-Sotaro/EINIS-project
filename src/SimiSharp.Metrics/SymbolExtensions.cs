// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SymbolExtensions.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SymbolExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.ReferenceResolvers;

namespace SimiSharp.CodeAnalysis
{
	public static class SymbolExtensions
	{
		private static readonly ConcurrentDictionary<SolutionId, Lazy<ReferenceRepository>> KnownReferences = new ConcurrentDictionary<SolutionId, Lazy<ReferenceRepository>>();

		public static Task<ReferencedSymbol> FindReferences(this Solution solution, ISymbol symbol)
		{
			if (solution == null)
			{
				return Task.FromResult(result: new ReferencedSymbol(symbol: symbol, locations: new ReferenceLocation[0]));
			}

			var lazyRepo = KnownReferences.GetOrAdd(key: solution.Id, valueFactory: x => new Lazy<ReferenceRepository>(valueFactory: () => new ReferenceRepository(solution: solution), mode: LazyThreadSafetyMode.ExecutionAndPublication));

			return Task.Run(
				function: () =>
				{
					var repo = lazyRepo.Value;
					var locations = repo.Get(key: symbol).AsArray();
					return new ReferencedSymbol(symbol: symbol, locations: locations);
				});
		}
	}
}