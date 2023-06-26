// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicInterfaceImplementationWarningRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the PublicInterfaceImplementationWarningRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Code
{
	internal class PublicInterfaceImplementationWarningRule : CodeEvaluationBase
	{
		private static IEnumerable<Type> _appDomainTypes;

		public override string ID => "AM0037";

		public override SyntaxKind EvaluatedKind => SyntaxKind.ClassDeclaration;

		public override string Title => "Public Interface Implementation";

		public override string Suggestion => "Consider whether the interface implementation also needs to be public.";

		public override CodeQuality Quality => CodeQuality.NeedsReview;

		public override QualityAttribute QualityAttribute => QualityAttribute.Modifiability;

		public override ImpactLevel ImpactLevel => ImpactLevel.Project;

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var classDeclaration = (ClassDeclarationSyntax)node;
			if (classDeclaration.BaseList != null
				&& (classDeclaration.BaseList.Types.Any(predicate: x => x.Type.IsKind(kind: SyntaxKind.IdentifierName)) || classDeclaration.BaseList.Types.Any(predicate: x => x.Type.IsKind(kind: SyntaxKind.GenericName))))
			{
				var s = classDeclaration.BaseList.Types.First(predicate: x => x.Type.IsKind(kind: SyntaxKind.IdentifierName) || x.Type.IsKind(kind: SyntaxKind.GenericName));
				if (((SimpleNameSyntax)s.Type).Identifier.ValueText.StartsWith(value: "I")
					&& classDeclaration.Modifiers.Any(kind: SyntaxKind.PublicKeyword))
				{
					var interfaceName = ((SimpleNameSyntax)s.Type).Identifier.ValueText;
					if (!IsKnownInterface(interfaceName: interfaceName))
					{
						var snippet = classDeclaration.ToFullString();

						return new EvaluationResult
								   {
									   Snippet = snippet
								   };
					}
				}
			}

			return null;
		}

		private bool IsKnownInterface(string interfaceName)
		{
			try
			{
				var types = _appDomainTypes ?? (_appDomainTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(selector: a => a.GetTypes()));
				return types
								.Any(
									predicate: t =>
									string.Equals(a: t.Name, b: interfaceName, comparisonType: StringComparison.InvariantCultureIgnoreCase)
									|| string.Equals(a: t.FullName, b: interfaceName));
			}
			catch
			{
				return false;
			}
		}
	}
}
