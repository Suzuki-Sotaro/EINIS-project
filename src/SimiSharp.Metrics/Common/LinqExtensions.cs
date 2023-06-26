// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinqExtensions.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LinqExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SimiSharp.CodeAnalysis.Common
{
    public static class LinqExtensions
    {
        public static T[] AsArray<T>(this IEnumerable<T> items)
        {
            var array = items as T[];
            return array ?? items.ToArray();
        }

        public static IEnumerable<T> DistinctBy<T, TOut>(this IEnumerable<T> source, Func<T, TOut> func)
        {
            var comparer = new FuncComparer<T, TOut>(func: func);
            return source.Distinct(comparer: comparer);
        }

        public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> source, Func<T, bool> filter)
        {
            return source.Where(predicate: x => !filter(arg: x));
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source) where T : class
        {
            return source.Where(predicate: x => x != null);
        }

        public static IEnumerable<string> WhereNotNullOrWhitespace(this IEnumerable<string> source)
        {
            return source.Where(predicate: x => !string.IsNullOrWhiteSpace(value: x));
        }

        public static Collection<T> ToCollection<T>(this IEnumerable<T> source)
        {
            return new Collection<T>(list: source.AsArray());
        }

        public static bool In<T>(this T item, IEnumerable<T> collection)
        {
            return collection.Contains(value: item);
        }

        private class FuncComparer<T, TOut> : IEqualityComparer<T>
        {
            private readonly Func<T, TOut> _func;

            public FuncComparer(Func<T, TOut> func)
            {
                _func = func;
            }

            public bool Equals(T x, T y)
            {
                return _func(arg: x).Equals(obj: _func(arg: y));
            }

            public int GetHashCode(T obj)
            {
                return _func(arg: obj).GetHashCode();
            }
        }
    }
}