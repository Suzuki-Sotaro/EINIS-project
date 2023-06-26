// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinesOfCodeCalculator.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LinesOfCodeCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SimiSharp.CodeAnalysis.Metrics
{
	public sealed class LinesOfCodeCalculator
	{
		public int Calculate(SyntaxNode node)
		{
			var innerCalculator = new InnerLinesOfCodeCalculator();
			return innerCalculator.Calculate(node: node);
		}

		private class InnerLinesOfCodeCalculator : CSharpSyntaxWalker
		{
			private int _counter;

			public InnerLinesOfCodeCalculator()
				: base(depth: SyntaxWalkerDepth.Node)
			{
			}

			public int Calculate(SyntaxNode node)
			{
				if (node != null)
				{
					Visit(node: node);
				}

				return _counter;
			}

			public override void VisitCheckedStatement(CheckedStatementSyntax node)
			{
				base.VisitCheckedStatement(node: node);
				_counter++;
			}

			public override void VisitDoStatement(DoStatementSyntax node)
			{
				base.VisitDoStatement(node: node);
				_counter++;
			}

			public override void VisitEmptyStatement(EmptyStatementSyntax node)
			{
				base.VisitEmptyStatement(node: node);
				_counter++;
			}

			public override void VisitExpressionStatement(ExpressionStatementSyntax node)
			{
				base.VisitExpressionStatement(node: node);
				_counter++;
			}

			/// <summary>
			/// Called when the visitor visits a AccessorDeclarationSyntax node.
			/// </summary>
			public override void VisitAccessorDeclaration(AccessorDeclarationSyntax node)
			{
				if (node.Body == null)
				{
					_counter++;
				}

				base.VisitAccessorDeclaration(node: node);
			}

			public override void VisitFixedStatement(FixedStatementSyntax node)
			{
				base.VisitFixedStatement(node: node);
				_counter++;
			}

			public override void VisitForEachStatement(ForEachStatementSyntax node)
			{
				base.VisitForEachStatement(node: node);
				_counter++;
			}

			public override void VisitForStatement(ForStatementSyntax node)
			{
				base.VisitForStatement(node: node);
				_counter++;
			}

			public override void VisitGlobalStatement(GlobalStatementSyntax node)
			{
				base.VisitGlobalStatement(node: node);
				_counter++;
			}

			public override void VisitGotoStatement(GotoStatementSyntax node)
			{
				base.VisitGotoStatement(node: node);
				_counter++;
			}

			public override void VisitIfStatement(IfStatementSyntax node)
			{
				base.VisitIfStatement(node: node);
				_counter++;
			}

			public override void VisitInitializerExpression(InitializerExpressionSyntax node)
			{
				base.VisitInitializerExpression(node: node);
				_counter += node.Expressions.Count;
			}

			public override void VisitLabeledStatement(LabeledStatementSyntax node)
			{
				base.VisitLabeledStatement(node: node);
				_counter++;
			}

			public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
			{
				base.VisitLocalDeclarationStatement(node: node);
				if (!node.Modifiers.Any(kind: SyntaxKind.ConstKeyword))
				{
					_counter++;
				}
			}

			public override void VisitLockStatement(LockStatementSyntax node)
			{
				base.VisitLockStatement(node: node);
				_counter++;
			}

			public override void VisitReturnStatement(ReturnStatementSyntax node)
			{
				base.VisitReturnStatement(node: node);
				if (node.Expression != null)
				{
					_counter++;
				}
			}

			public override void VisitSwitchStatement(SwitchStatementSyntax node)
			{
				base.VisitSwitchStatement(node: node);
				_counter++;
			}

			public override void VisitThrowStatement(ThrowStatementSyntax node)
			{
				base.VisitThrowStatement(node: node);
				_counter++;
			}

			public override void VisitUnsafeStatement(UnsafeStatementSyntax node)
			{
				base.VisitUnsafeStatement(node: node);
				_counter++;
			}

			public override void VisitUsingDirective(UsingDirectiveSyntax node)
			{
				base.VisitUsingDirective(node: node);
				_counter++;
			}

			public override void VisitUsingStatement(UsingStatementSyntax node)
			{
				base.VisitUsingStatement(node: node);
				_counter++;
			}

			/// <summary>
			/// Called when the visitor visits a ConstructorDeclarationSyntax node.
			/// </summary>
			public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
			{
				base.VisitConstructorDeclaration(node: node);
				_counter++;
			}

			public override void VisitWhileStatement(WhileStatementSyntax node)
			{
				base.VisitWhileStatement(node: node);
				_counter++;
			}

			public override void VisitYieldStatement(YieldStatementSyntax node)
			{
				base.VisitYieldStatement(node: node);
				_counter++;
			}
		}
	}
}