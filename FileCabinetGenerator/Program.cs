using System;
using System.Collections.Generic;
using System.Globalization;

[assembly: CLSCompliant(true)]

namespace FileCabinetGenerator
{
    /// <summary>
    /// The class that contains the Main method.
    /// </summary>
    public static class Program
    {
        private static readonly string[] OutputTypeFlags = { "--output-type", "-t" };
        private static readonly string[] OutputFileFlags = { "--output", "-o" };
        private static readonly string[] RecordsAmountFlags = { "--records-amount", "-a" };
        private static readonly string[] StartIdFlags = { "--start-id", "-i" };
        private static string outputType = "csv";
        private static string outputFile = "data.csv";
        private static ulong recordsAmount = 0;
        private static ulong startId = 0;

        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Init(args);

            Console.WriteLine("{0} records were written to {1}.", recordsAmount, outputFile);
        }

        /// <summary>
        /// Sets initial values according to application parameters.
        /// </summary>
        /// <param name="args">The <see cref="string"/> array instance input parameters.</param>
        private static void Init(string[] args)
        {
            var parsedArgsDictionary = ParseArgs(args);

            foreach (var flag in OutputTypeFlags)
            {
                var value = string.Empty;
                if (parsedArgsDictionary.TryGetValue(flag, out value))
                {
                    outputType = value;
                }
            }

            foreach (var flag in OutputFileFlags)
            {
                var value = string.Empty;
                if (parsedArgsDictionary.TryGetValue(flag, out value))
                {
                    outputFile = value;
                }
                else if (outputType.Equals("xml", StringComparison.OrdinalIgnoreCase))
                {
                    outputFile = "data.xml";
                }
            }

            foreach (var flag in RecordsAmountFlags)
            {
                var strValue = string.Empty;
                ulong value = 0;
                if (parsedArgsDictionary.TryGetValue(flag, out strValue)
                    && ulong.TryParse(strValue, out value))
                {
                    recordsAmount = value;
                }
            }

            foreach (var flag in StartIdFlags)
            {
                var strValue = string.Empty;
                ulong value = 0;
                if (parsedArgsDictionary.TryGetValue(flag, out strValue)
                    && ulong.TryParse(strValue, out value))
                {
                    startId = value;
                }
            }
        }

        /// <summary>
        /// Parses input arguments input a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="args">The <see cref="string"/> array instance.</param>
        /// <returns>The <see cref="Dictionary{TKey, TValue}"/> object.</returns>
        private static Dictionary<string, string> ParseArgs(string[] args)
        {
            Dictionary<string, string> result = new();
            int i = 0;
            while (i < args.Length)
            {
                var splitedArg = args[i].Split("=");
                if (splitedArg.Length == 2)
                {
                    result.Add(splitedArg[0].ToLower(CultureInfo.InvariantCulture), splitedArg[1].ToLower(CultureInfo.InvariantCulture));
                }
                else if (splitedArg.Length == 1 && i + 1 < args.Length)
                {
                    result.Add(args[i].ToLower(CultureInfo.InvariantCulture), args[i + 1].ToLower(CultureInfo.InvariantCulture));
                    i++;
                }

                i++;
            }

            return result;
        }
    }
}