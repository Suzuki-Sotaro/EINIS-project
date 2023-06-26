// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoverageAnalyzer.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CoverageAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using SimiSharp.CodeAnalysis.Common;

namespace SimiSharp.CodeAnalysis
{
	internal class CoverageAnalyzer
	{
		private readonly Solution _solution;

		public CoverageAnalyzer(Solution solution)
		{
			_solution = solution;
		}

		public async Task<bool> IsReferencedInTest(ISymbol symbol)
		{
			var symbolReferences = await _solution.FindReferences(symbol: symbol).ConfigureAwait(continueOnCapturedContext: false);
			if (!symbolReferences.Locations.Any())
			{
				return false;
			}

			var referencingSymbolTasks = (from location in symbolReferences.Locations
										  let rootTask = location.Location.SourceTree.GetRootAsync()
										  select new { TokenTask = rootTask, Location = location })
										 .AsArray();

			await Task.WhenAll(tasks: referencingSymbolTasks.Select(selector: x => x.TokenTask)).ConfigureAwait(continueOnCapturedContext: false);

			var referencingMethods = referencingSymbolTasks
				.Select(selector: x => new
						   {
							   Token = x.TokenTask.Result.FindToken(position: x.Location.Location.SourceSpan.Start),
							   Model = x.Location.Model
						   })
				.Select(
					selector: x => new
						 {
							 Method = x.Token.GetMethod(),
							 Model = x.Model,
						 })
				.AsArray();

			var referencingTests = referencingMethods
				.Select(selector: x => x.Method)
				.Select(selector: x => x.AttributeLists.Any(predicate: a => a.Attributes.Any(predicate: b => b.Name.ToString().IsKnownTestAttribute())));

			if (referencingTests.Any(predicate: x => x))
			{
				return true;
			}

			var referencingSymbols = from reference in referencingMethods
									 let model = reference.Model
									 let referencingSymbol = model.GetDeclaredSymbol(declaration: reference.Method)
									 select IsReferencedInTest(symbol: referencingSymbol);

			return await referencingSymbols.AsArray().FirstMatch(predicate: x => x).ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}