// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodLocalVariablesAnalyzer.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MethodLocalVariablesAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SimiSharp.CodeAnalysis.Metrics
{
    internal sealed class MethodLocalVariablesAnalyzer : CSharpSyntaxWalker
    {
        private int _numLocalVariables;

        public MethodLocalVariablesAnalyzer()
            : base(depth: SyntaxWalkerDepth.Node)
        {
        }

        public int Calculate(SyntaxNode node)
        {
            if (node != null)
            {
                Visit(node: node);
            }

            return _numLocalVariables;
        }

        public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            base.VisitVariableDeclaration(node: node);
            _numLocalVariables++;
        }
    }
}
