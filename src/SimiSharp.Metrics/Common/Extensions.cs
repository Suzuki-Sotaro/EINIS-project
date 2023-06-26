// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the Extensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimiSharp.CodeAnalysis.Common
{
    public static class Extensions
    {
        private static readonly Regex CapitalRegex = new Regex(pattern: "[A-Z]", options: RegexOptions.Compiled);
        private static readonly string[] KnownTestAttributes = { "Test", "TestCase", "TestMethod", "Fact", "Theory" };

        public static bool IsKnownTestAttribute(this string text)
        {
            return KnownTestAttributes.Contains(value: text);
        }

        public static void DisposeNotNull(this IDisposable disposable)
        {
            disposable?.Dispose();
        }

        public static string ToTitleCase(this string input)
        {
            return CapitalRegex.Replace(input: input, evaluator: m => " " + m).Replace(oldValue: "_", newValue: " ").Trim();
        }

        public static async Task<T> FirstMatch<T>(this IEnumerable<Task<T>> tasks, Func<T, bool> predicate)
        {
            var taskArray = tasks.AsArray();
            var finished = await Task.WhenAny(tasks: taskArray).ConfigureAwait(continueOnCapturedContext: false);
            if (predicate(arg: finished.Result))
            {
                return finished.Result;
            }

            var remaining = taskArray.Except(second: new[] { finished }).AsArray();
            if (remaining.Length == 0)
            {
                return default(T);
            }

            return await FirstMatch(tasks: remaining, predicate: predicate).ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}
