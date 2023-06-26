// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeMetric.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ITypeMetric type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace SimiSharp.CodeAnalysis.Common.Metrics
{
    public interface ITypeMetric : ICodeMetric
    {
        AccessModifierKind AccessModifier { get; }

        TypeMetricKind Kind { get; }

        IEnumerable<IMemberMetric> MemberMetrics { get; }

        int DepthOfInheritance { get; }

        int ClassCoupling { get; }

        int AfferentCoupling { get; }

        int EfferentCoupling { get; }

        double Instability { get; }

        bool IsAbstract { get; }

        ITypeDocumentation Documentation { get; }
    }
}