﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceRepository.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ReferenceRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using SimiSharp.CodeAnalysis.Common;

namespace SimiSharp.CodeAnalysis.ReferenceResolvers
{
	public class ReferenceRepository : IProvider<ISymbol, IEnumerable<ReferenceLocation>>
	{
		private readonly ConcurrentDictionary<ISymbol, IEnumerable<ReferenceLocation>> _resolvedReferences = new ConcurrentDictionary<ISymbol, IEnumerable<ReferenceLocation>>();
		private readonly Task _scanTask;

		public ReferenceRepository(Solution solution)
		{
			_scanTask = Scan(solution: solution);
		}

		public IEnumerable<ReferenceLocation> Get(ISymbol key)
		{
			_scanTask.Wait();

			IEnumerable<ReferenceLocation> locations;
			return _resolvedReferences.TryGetValue(key: key, value: out locations)
				? locations
				: Enumerable.Empty<ReferenceLocation>();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(isDisposing: true);
		}

		private void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_resolvedReferences.Clear();
			}
		}

		private async Task Scan(Solution solution)
		{
			var roots = await GetDocData(solution: solution).ConfigureAwait(continueOnCapturedContext: false);

			var groups = from root in roots
						 let compilation = root.Compilation
						 from syntaxNode in root.DocRoots
						 from @group in compilation.Resolve(root: syntaxNode)
						 select @group;

			foreach (var @group in groups)
			{
				_resolvedReferences.AddOrUpdate(key: @group.Key, addValue: @group.AsArray(), updateValueFactory: (s, r) => r.Concat(second: @group).AsArray());
			}
		}

		private async Task<IEnumerable<DocData>> GetDocData(Solution solution)
		{
			var roots = (from project in solution.Projects
						 let compilation = project.GetCompilationAsync()
						 let docRoots = project.Documents.Select(selector: x => x.GetSyntaxRootAsync())
						 select new { compilation, docRoots }).AsArray();

			await Task.WhenAll(tasks: roots.SelectMany(selector: x => new Task[] { x.compilation }.Concat(second: x.docRoots))).ConfigureAwait(continueOnCapturedContext: false);

			return roots.Select(selector: x => new DocData
			{
				Compilation = x.compilation.Result,
				DocRoots = x.docRoots.Select(selector: y => y.Result).AsArray()
			});
		}

		private class DocData
		{
			public Compilation Compilation { get; set; }

			public IEnumerable<SyntaxNode> DocRoots { get; set; }
		}
	}
}
