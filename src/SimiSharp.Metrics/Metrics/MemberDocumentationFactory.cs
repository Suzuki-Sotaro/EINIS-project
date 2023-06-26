// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberDocumentationFactory.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberDocumentationFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.Metrics;

namespace SimiSharp.CodeAnalysis.Metrics
{
	public class MemberDocumentationFactory : IAsyncFactory<ISymbol, IMemberDocumentation>
    {
        private static readonly MethodKind[] ChildMethods = {MethodKind.PropertyGet, MethodKind.PropertySet, MethodKind.EventAdd, MethodKind.EventRemove};

        /// <summary>
        /// Creates the requested instance as an asynchronous operation.
        /// </summary>
        /// <param name="memberSymbol">The memberSymbol to pass to the object creation.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to use for cancelling the object creation.</param>
        /// <returns>Returns a <see cref="Task{T}"/> which represents the instance creation task.</returns>
        public Task<IMemberDocumentation> Create(ISymbol memberSymbol, CancellationToken cancellationToken)
        {
            var doc = GetDocumentationText(symbol: memberSymbol);
            if (string.IsNullOrWhiteSpace(value: doc))
            {
                return Task.FromResult<IMemberDocumentation>(result: null);
            }

            var docRoot = TryParseXmlComment(xml: doc);
            if (docRoot == null)
            {
                return Task.FromResult<IMemberDocumentation>(result: new FaultMemberDocumentation(rawComment: doc));
            }

            var summaryElement = docRoot.Element(name: "summary");
            var summary = summaryElement?.Value.Trim() ?? string.Empty;
            var codeElement = docRoot.Element(name: "code");
            var code = codeElement?.Value.Trim() ?? string.Empty;
            var exampleElement = docRoot.Element(name: "example");
            var example = exampleElement?.Value.Trim() ?? string.Empty;
            var remarksElement = docRoot.Element(name: "remarks");
            var remarks = remarksElement?.Value.Trim() ?? string.Empty;
            var returnsElement = docRoot.Element(name: "returns");
            var returns = returnsElement?.Value.Trim() ?? string.Empty;
            var typeParameterElements = docRoot.Elements(name: "typeparam");
            var parameterElements = docRoot.Elements(name: "param")
                .Select(selector: _ => new KeyValuePair<string, string>(key: _.Attribute(name: "name").Value.Trim(), value: _.Value.Trim()))
                .ToDictionary(keySelector: _ => _.Key, elementSelector: _ => _.Value);
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
            var parameters = GetParameters(symbol: memberSymbol as IMethodSymbol, parameterDocumentations: parameterElements);
            var exceptionElements = docRoot.Elements(name: "exception");
            var exceptions =
                exceptionElements.Select(
                    selector: x => new ExceptionDocumentation(exceptionType: x.Attribute(name: "cref").Value.Trim(), description: x.Value.Trim()));

            var documentation = new MemberDocumentation(summary: summary, code: code, example: example, remarks: remarks, returns: returns, typeParameters: typeParameters,
                parameters: parameters, exceptions: exceptions);

            return Task.FromResult<IMemberDocumentation>(result: documentation);
        }

        private static XElement TryParseXmlComment(string xml)
        {
            try
            {
                var xmldoc = XDocument.Parse(text: xml);
                return xmldoc.Root;
            }
            catch (XmlException)
            {
                return null;
            }
	    }

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
		}

		private static string GetDocumentationText(ISymbol symbol)
		{
			var methodSymbol = symbol as IMethodSymbol;
			var isChildMethod = methodSymbol != null && methodSymbol.MethodKind.In(collection: ChildMethods);
			return isChildMethod
				? GetDocumentationText(symbol: methodSymbol.AssociatedSymbol)
				: symbol.GetDocumentationCommentXml();
		}

		private static IEnumerable<ParameterDocumentation> GetParameters(IMethodSymbol symbol, IDictionary<string, string> parameterDocumentations)
		{
			return symbol == null
					   ? Enumerable.Empty<ParameterDocumentation>()
					   : symbol.Parameters.Select(
						   selector: x =>
						   new ParameterDocumentation(
							   parameterName: x.Name,
							   parameterType: x.Type.ToDisplayString(),
							   description: parameterDocumentations.ContainsKey(key: x.Name) ? parameterDocumentations[key: x.Name] : string.Empty)).ToArray();
		}

		private static IDictionary<string, string> GetTypeContraints(ISymbol symbol)
		{
			var method = symbol as IMethodSymbol;
			if (method == null)
			{
				return new Dictionary<string, string>();
			}

			var enumerable = method.TypeParameters.Select(selector: CreateTypeConstraint);
			var typeParameterConstraints = enumerable.ToDictionary(keySelector: _ => _.Key, elementSelector: _ => _.Value);

			return typeParameterConstraints;
		}

		private static KeyValuePair<string, string> CreateTypeConstraint(ITypeParameterSymbol typeParameter)
		{
			var parts = new List<string>();
			if (typeParameter.HasReferenceTypeConstraint)
			{
				parts.Add(item: "class");
			}

			if (typeParameter.HasValueTypeConstraint)
			{
				parts.Add(item: "struct");
			}

			if (typeParameter.HasConstructorConstraint)
			{
				parts.Add(item: "new()");
			}

			parts.AddRange(collection: typeParameter.ConstraintTypes.Select(selector: constraintType => constraintType.ToDisplayString()));

			return new KeyValuePair<string, string>(key: typeParameter.Name, value: string.Join(separator: ", ", values: parts));
		}
	}
}