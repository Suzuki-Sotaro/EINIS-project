// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyFinalizerRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EmptyFinalizerRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules.Semantic
{
    internal class EmptyFinalizerRule : SemanticEvaluationBase
    {
        public override string ID => "CA1821";

        public override string Title => "Empty Finalizer Detected";

        public override string Suggestion => "Finalizer should call dispose method.";

        public override CodeQuality Quality => CodeQuality.NeedsReview;

        public override QualityAttribute QualityAttribute => QualityAttribute.Performance;

        public override ImpactLevel ImpactLevel => ImpactLevel.Type;

        public override SyntaxKind EvaluatedKind => SyntaxKind.DestructorDeclaration;

        protected override Task<EvaluationResult> EvaluateImpl(SyntaxNode node, SemanticModel semanticModel, Solution solution)
        {
            if (IsEmptyFinalizer(node: node, model: semanticModel))
            {
                var result = new EvaluationResult
                {
                    Snippet = node.ToFullString()
                };

                return Task.FromResult(result: result);
            }

            return Task.FromResult<EvaluationResult>(result: null);
        }

        private bool IsEmptyFinalizer(SyntaxNode node, SemanticModel model)
        {
            // NOTE: FxCop only checks if there is any method call within a given destructor to decide an empty finalizer.
            // Here in order to minimize false negatives, we conservatively treat it as non-empty finalizer if its body contains any statements.
            // But, still conditional methods like Debug.Fail() will be considered as being empty as FxCop currently does.
            return node.DescendantNodes().OfType<StatementSyntax>()
                .Where(predicate: n => !n.IsKind(kind: SyntaxKind.Block) && !n.IsKind(kind: SyntaxKind.EmptyStatement))
                .Select(selector: exp => exp as ExpressionStatementSyntax)
                .All(predicate: method => method != null && HasConditionalAttribute(root: method.Expression, model: model));
        }

        private bool HasConditionalAttribute(SyntaxNode root, SemanticModel model)
        {
            var node = root as InvocationExpressionSyntax;
            if (node != null)
            {
                var exp = node.Expression as MemberAccessExpressionSyntax;
                if (exp != null)
                {
                    var symbolInfo = model.GetSymbolInfo(expression: exp);
                    var symbol = symbolInfo.Symbol;
                    if (symbol != null && symbol.GetAttributes().Any(predicate: n => n.AttributeClass.MetadataName.Equals(value: "ConditionalAttribute")))
                    {
                        //// System.Diagnostics.ConditionalAttribute
                        return true;
                    }
                }
            }

            return false;
        }
    }
}