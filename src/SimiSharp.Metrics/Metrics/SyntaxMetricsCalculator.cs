// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyntaxMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SyntaxMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.Metrics;

namespace SimiSharp.CodeAnalysis.Metrics
{
	public sealed class SyntaxMetricsCalculator
	{
		private readonly Func<SyntaxNode, bool> _isMethod = n => n.IsKind(kind: SyntaxKind.MethodDeclaration) && (n as MethodDeclarationSyntax).Body != null;

		public IEnumerable<IHalsteadMetrics> Calculate(string code)
		{
			try
			{
				var tree = CSharpSyntaxTree.ParseText(text: code);
				var root = tree.GetRoot();
				var metrics = Calculate(root: root);
				return metrics;
			}
			catch
			{
				return new[] { new HalsteadMetrics(numOperands: 0, numOperators: 0, numUniqueOperands: 0, numUniqueOperators: 0) };
			}
		}

		public IEnumerable<IHalsteadMetrics> Calculate(SyntaxNode root)
		{
			var analyzer = new HalsteadAnalyzer();
			var childNodes = root.ChildNodes().AsArray();

			var types = childNodes.Where(predicate: n => n.IsKind(kind: SyntaxKind.ClassDeclaration) || n.IsKind(kind: SyntaxKind.StructDeclaration))
				.AsArray();
			var methods = types.SelectMany(selector: n => n.ChildNodes().Where(predicate: _isMethod));
			var getProperties = types.SelectMany(selector: n => n.ChildNodes().Where(predicate: IsGetProperty));
			var setProperties = types.SelectMany(selector: n => n.ChildNodes().Where(predicate: IsSetProperty));
			var looseMethods = childNodes.Where(predicate: _isMethod);
			var looseGetProperties = childNodes.Where(predicate: IsGetProperty);
			var looseSetProperties = childNodes.Where(predicate: IsSetProperty);
			var members = methods.Concat(second: getProperties)
								 .Concat(second: setProperties)
								 .Concat(second: looseMethods)
								 .Concat(second: looseGetProperties)
								 .Concat(second: looseSetProperties)
								 .AsArray();
			if (members.Any())
			{
				return members.Select(selector: analyzer.Calculate);
			}

			var statements = childNodes.Length == 0
				? root.DescendantNodesAndTokens().Select(selector: x => SyntaxFactory.ParseStatement(text: x.ToFullString(), offset: 0, options: new CSharpParseOptions(preprocessorSymbols: new string[0])))
				: childNodes.Select(selector: x => SyntaxFactory.ParseStatement(text: x.ToFullString(), offset: 0, options: new CSharpParseOptions(preprocessorSymbols: new string[0])));

			var fakeMethod = SyntaxFactory.MethodDeclaration(returnType: SyntaxFactory.PredefinedType(keyword: SyntaxFactory.Token(kind: SyntaxKind.VoidKeyword)), identifier: "fake")
				.WithBody(body: SyntaxFactory.Block(statements: statements));
			return new[]
				   {
					   analyzer.Calculate(syntax: fakeMethod)
				   };
		}

		private static bool IsGetProperty(SyntaxNode n)
		{
			if (!n.IsKind(kind: SyntaxKind.PropertyDeclaration))
			{
				return false;
			}

			var propertyDeclarationSyntax = n as PropertyDeclarationSyntax;
			return propertyDeclarationSyntax != null && propertyDeclarationSyntax.AccessorList.Accessors.Any(predicate: a => a.IsKind(kind: SyntaxKind.GetAccessorDeclaration));
		}

		private static bool IsSetProperty(SyntaxNode n)
		{
			if (!n.IsKind(kind: SyntaxKind.PropertyDeclaration))
			{
				return false;
			}

			var propertyDeclarationSyntax = n as PropertyDeclarationSyntax;
			return propertyDeclarationSyntax != null && propertyDeclarationSyntax.AccessorList.Accessors.Any(predicate: a => a.IsKind(kind: SyntaxKind.SetAccessorDeclaration));
		}
	}
}
