using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp.StaticClasses
{
    /// <summary>
    /// Helper static class with parsing static functions.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Parses input arguments input a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="args">The <see cref="string"/> array instance.</param>
        /// <returns>The <see cref="Dictionary{TKey, TValue}"/> object.</returns>
        public static Dictionary<string, string> ParseArgs(string[] args)
        {
            Dictionary<string, string> result = new ();
            int i = 0;
            while (i < args.Length)
            {
                args[i] = args[i].ToLower(CultureInfo.InvariantCulture);
                if (args[i].StartsWith("--", StringComparison.InvariantCulture))
                {
                    var arg = args[i].Split("=");
                    if (arg.Length >= 2)
                    {
                        result.Add(arg[0], arg[1]);
                    }
                    else
                    {
                        result.Add(arg[0], string.Empty);
                    }
                }
                else if (args[i].StartsWith("-", StringComparison.InvariantCulture))
                {
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-", StringComparison.InvariantCulture))
                    {
                        result.Add(args[i], args[i + 1]);
                        i++;
                    }
                    else
                    {
                        result.Add(args[i], string.Empty);
                    }
                }

                i++;
            }

            return result;
        }
    }
}
