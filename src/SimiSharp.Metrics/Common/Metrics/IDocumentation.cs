// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDocumentation.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IDocumentation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimiSharp.CodeAnalysis.Common.Metrics
{
    public interface IDocumentation
    {
        string Summary { get; }

        string Returns { get; }

        string Code { get; }

        string Example { get; }

        string Remarks { get; }
    }
}