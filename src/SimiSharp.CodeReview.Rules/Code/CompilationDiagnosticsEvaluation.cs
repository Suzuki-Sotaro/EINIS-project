// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompilationDiagnosticsEvaluation.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CompilationDiagnosticsEvaluation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class CompilationDiagnosticsEvaluation : CodeEvaluationBase
	{
		public override string ID => "AMC9999";

		public override SyntaxKind EvaluatedKind => SyntaxKind.CompilationUnit;

		public override string Title => "Compilation Failure";

		public override string Suggestion => "Check the compilation error for details about reason for failure.";

		public override CodeQuality Quality => CodeQuality.Broken;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality;

		public override ImpactLevel ImpactLevel => ImpactLevel.Project;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			// Roslyn does not handle async await keywords.
			var diagnostics = node.GetDiagnostics();
			if (diagnostics.Any(predicate: d => d.Severity != DiagnosticSeverity.Info))
			{
				return new EvaluationResult
						   {
							   Snippet = node.ToFullString()
						   };
			}

			return null;
		}
	}
}
