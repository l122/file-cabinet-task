using System;
using System.Collections.Generic;
using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.StaticClasses
{
    /// <summary>
    /// Helper class that implements memoization for the select records function in FileCabinetMemoryService.
    /// </summary>
    public static class Memoizer
    {
        private static readonly Dictionary<string, IEnumerable<FileCabinetRecord>> Cache = new ();

        /// <summary>
        /// Stores the results of the input function.
        /// </summary>
        /// <param name="func">The input function.</param>
        /// <returns>A <see cref="Func{T, TResult}"/> instance of the function result.</returns>
        public static Func<string, IEnumerable<FileCabinetRecord>> Memoize(Func<string, IEnumerable<FileCabinetRecord>> func)
        {
            return a =>
            {
                if (Cache.TryGetValue(a, out var value))
                {
                    return value;
                }

                value = func(a);
                Cache.Add(a, value);
                return value;
            };
        }

        /// <summary>
        /// Resets the memo.
        /// </summary>
        public static void MemoizeReset()
        {
            Cache.Clear();
        }
    }
}
