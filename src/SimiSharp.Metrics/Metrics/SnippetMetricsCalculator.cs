// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SnippetMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SnippetMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SimiSharp.CodeAnalysis.Metrics
{
	public class SnippetMetricsCalculator
	{
		public Compilation Calculate(string snippet)
		{
			var tree = CSharpSyntaxTree.ParseText(text: snippet);
			var compilation = CSharpCompilation.Create(assemblyName: "x", syntaxTrees: new[] { tree });
			return compilation;
		}
	}
}
