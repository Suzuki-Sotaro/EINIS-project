// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LockingOnWeakIdentityObjectRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LockingOnWeakIdentityObjectRule type.
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
	internal class LockingOnWeakIdentityObjectRule : SemanticEvaluationBase
	{
		private static readonly string[] WeakIdentities =
		{
			"System.MarshalByRefObject", 
			"System.ExecutionEngineException", 
			"System.OutOfMemoryException", 
			"System.StackOverflowException", 
			"string", 
			"System.Reflection.MemberInfo", 
			"System.Reflection.ParameterInfo", 
			"System.Threading.Thread"
		};

		public override string ID => "AM0056";

		public override string Suggestion => "Change lock object to strong identity object, ex. new object()";

		public override CodeQuality Quality => CodeQuality.Broken;

		public override QualityAttribute QualityAttribute => QualityAttribute.CodeQuality;

		public override ImpactLevel ImpactLevel => ImpactLevel.Type;

		public override SyntaxKind EvaluatedKind => SyntaxKind.LockStatement;

		public override string Title => "No locking on Weak Identity Items";

		protected override Task<EvaluationResult> EvaluateImpl(SyntaxNode node, SemanticModel semanticModel, Solution solution)
		{
			var lockStatement = (LockStatementSyntax)node;
			var lockExpression = lockStatement.Expression as IdentifierNameSyntax;
			if (lockExpression != null)
			{
				var lockObjectSymbolInfo = semanticModel.GetSymbolInfo(expression: lockExpression);
				var symbol = lockObjectSymbolInfo.Symbol as IFieldSymbol;
				if (symbol != null && IsWeakIdentity(typeSymbol: symbol.Type))
				{
					return Task.FromResult(result: new EvaluationResult
						   {
							   Snippet = node.ToFullString()
						   });
				}
			}

			return Task.FromResult(result: (EvaluationResult)null);
		}

		private bool IsWeakIdentity(ITypeSymbol typeSymbol)
		{
			return typeSymbol != null && (WeakIdentities.Contains(value: typeSymbol.ToString()) || IsWeakIdentity(typeSymbol: typeSymbol.BaseType));
		}
	}
}