// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberCollector.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberCollector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common.Metrics;

namespace SimiSharp.CodeAnalysis.Metrics
{
    internal sealed class MemberCollector : CSharpSyntaxWalker
    {
        private readonly List<SyntaxNode> _members;

        public MemberCollector()
            : base(depth: SyntaxWalkerDepth.Node)
        {
            _members = new List<SyntaxNode>();
        }

        public IEnumerable<SyntaxNode> GetMembers(TypeDeclarationSyntaxInfo type)
        {
            Visit(node: type.Syntax);
            return _members.ToList();
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            base.VisitConstructorDeclaration(node: node);
            _members.Add(item: node);
        }

        public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            base.VisitDestructorDeclaration(node: node);
            _members.Add(item: node);
        }

        public override void VisitEventDeclaration(EventDeclarationSyntax node)
        {
            base.VisitEventDeclaration(node: node);
            if (node.AccessorList != null)
            {
                foreach (var accessor in node.AccessorList.Accessors)
                {
                    _members.Add(item: accessor);
                }
            }
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            base.VisitMethodDeclaration(node: node);
            _members.Add(item: node);
        }

        /// <summary>
        /// Called when the visitor visits a ArrowExpressionClauseSyntax node.
        /// </summary>
        public override void VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
        {
            var accessor = SyntaxFactory.ReturnStatement(expression: node.Expression);

            _members.Add(item: accessor);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            base.VisitPropertyDeclaration(node: node);
            if (node.AccessorList != null)
            {
                foreach (var accessor in node.AccessorList.Accessors)
                {
                    _members.Add(item: accessor);
                }
            }
        }
    }
}
