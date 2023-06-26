// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeDocumentationFactory.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeDocumentationFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.Metrics;

namespace SimiSharp.CodeAnalysis.Metrics
{
	internal class TypeDocumentationFactory : IAsyncFactory<ISymbol, ITypeDocumentation>
	{
		/// <summary>
		/// Creates the requested instance as an asynchronous operation.
		/// </summary>
		/// <param name="memberSymbol">The memberSymbol to pass to the object creation.</param>
		/// <param name="cancellationToken">A <see cref="CancellationToken"/> to use for cancelling the object creation.</param>
		/// <returns>Returns a <see cref="Task{T}"/> which represents the instance creation task.</returns>
		public Task<ITypeDocumentation> Create(ISymbol memberSymbol, CancellationToken cancellationToken)
		{
			var doc = memberSymbol.GetDocumentationCommentXml();
			if (string.IsNullOrWhiteSpace(value: doc))
			{
				return Task.FromResult<ITypeDocumentation>(result: null);
			}

			var xmldoc = XDocument.Parse(text: doc);
			var docRoot = xmldoc.Root;
			if (docRoot == null)
			{
				return Task.FromResult<ITypeDocumentation>(result: null);
			}

			var summaryElement = docRoot.Element(name: "summary");
			var summary = summaryElement == null ? string.Empty : summaryElement.Value.Trim();
			var codeElement = docRoot.Element(name: "code");
			var code = codeElement == null ? string.Empty : codeElement.Value.Trim();
			var exampleElement = docRoot.Element(name: "example");
			var example = exampleElement == null ? string.Empty : exampleElement.Value.Trim();
			var remarksElement = docRoot.Element(name: "remarks");
			var remarks = remarksElement?.Value.Trim() ?? string.Empty;
			var returnsElement = docRoot.Element(name: "returns");
			var returns = returnsElement == null ? string.Empty : returnsElement.Value.Trim();
			var typeParameterElements = docRoot.Elements(name: "typeparam");
			var typeConstraints = GetTypeContraints(symbol: memberSymbol);
			var typeParameters =
				typeParameterElements.Select(
					selector: x =>
						{
							var name = x.Attribute(name: "name").Value.Trim();
							return new TypeParameterDocumentation(
								typeParameterName: name,
								constraint: typeConstraints.ContainsKey(key: name) ? typeConstraints[key: name] : null,
								description: x.Value.Trim());
						});

			var documentation = new TypeDocumentation(summary: summary, code: code, example: example, remarks: remarks, returns: returns, typeParameters: typeParameters);

			return Task.FromResult<ITypeDocumentation>(result: documentation);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
		}

		private static IDictionary<string, string> GetTypeContraints(ISymbol symbol)
		{
			var method = symbol as IMethodSymbol;
			if (method == null)
			{
				return new Dictionary<string, string>();
			}

			var enumerable = method.TypeParameters.Select(selector: CreateTypeConstraint);
			IDictionary<string, string> typeParameterConstraints = enumerable.ToDictionary(keySelector: _ => _.Key, elementSelector: _ => _.Value);

			return typeParameterConstraints;
		}

		private static KeyValuePair<string, string> CreateTypeConstraint(ITypeParameterSymbol typeParameter)
		{
			return new KeyValuePair<string, string>(key: typeParameter.Name, value: typeParameter.ToDisplayString());
		}
	}
}