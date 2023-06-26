// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FaultMemberDocumentation.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the FaultMemberDocumentation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using SimiSharp.CodeAnalysis.Common.Metrics;

namespace SimiSharp.CodeAnalysis.Metrics
{
    internal class FaultMemberDocumentation : IMemberDocumentation
    {
        public FaultMemberDocumentation(string rawComment)
        {
            Summary = rawComment;
        }

        public string Summary { get; }

        public string Returns => Summary;

        public string Code => Summary;

        public string Example => Summary;

        public string Remarks => Summary;

        public IEnumerable<ParameterDocumentation> Parameters { get { yield break; } }

        public IEnumerable<TypeParameterDocumentation> TypeParameters { get { yield break; } }

        public IEnumerable<ExceptionDocumentation> Exceptions { get { yield break; } }
    }
}