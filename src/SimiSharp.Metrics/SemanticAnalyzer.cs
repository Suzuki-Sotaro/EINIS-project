// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SemanticAnalyzer.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SemanticAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common;

namespace SimiSharp.CodeAnalysis
{
	public class SemanticAnalyzer
	{
		private readonly SemanticModel _model;

		public SemanticAnalyzer(SemanticModel model)
		{
			_model = model;
		}

		public IEnumerable<ParameterSyntax> GetUnusedParameters(BaseMethodDeclarationSyntax method)
		{
			if (method.ParameterList.Parameters.Count == 0 || method.Body == null || !method.Body.ChildNodes().Any())
			{
				return new ParameterSyntax[0];
			}

			var bodyNodes = method.Body.ChildNodes();
			var dataflow = _model.AnalyzeDataFlow(firstStatement: bodyNodes.First(), lastStatement: bodyNodes.Last());

			var usedParameterNames = dataflow.DataFlowsIn
				.Where(predicate: x => x.Kind == SymbolKind.Parameter)
				.Select(selector: x => x.Name)
				.AsArray();

			var unusedParameters = method.ParameterList.Parameters
				.Where(predicate: p => !usedParameterNames.Contains(value: p.Identifier.ValueText))
				.AsArray();
			return unusedParameters;
		}

		[SuppressMessage(category: "Microsoft.Design", checkId: "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "TypeDeclaration constraint intended.")]
		public IEnumerable<MethodDeclarationSyntax> GetPossibleStaticMethods(TypeDeclarationSyntax type)
		{
			return type.DescendantNodes()
				.OfType<MethodDeclarationSyntax>()
				.Where(predicate: x => !x.Modifiers.Any(kind: SyntaxKind.StaticKeyword))
				.Where(predicate: CanBeMadeStatic)
				.AsArray();
		}

		public bool CanBeMadeStatic(BaseMethodDeclarationSyntax method)
		{
			if (method.Modifiers.Any(kind: SyntaxKind.StaticKeyword)
				|| method.Body == null
				|| !method.Body.ChildNodes().Any())
			{
				return false;
			}

			var bodyNodes = method.Body.ChildNodes();
			var dataflow = _model.AnalyzeDataFlow(firstStatement: bodyNodes.First(), lastStatement: bodyNodes.Last());
			var hasThisReference = dataflow.DataFlowsIn
				.Any(predicate: x => x.Kind == SymbolKind.Parameter && x.Name == SyntaxFactory.Token(kind: SyntaxKind.ThisKeyword).ToFullString());
			return !hasThisReference;
		}
	}
}