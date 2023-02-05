using FileCabinetApp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
        private const string Csv = "csv";
        private const string Xml = "xml";
        private static string outputType = Csv;
        private static string outputFile = "data.csv";
        private static int recordsAmount = 0;
        private static int startId = 0;
        private static IRecordValidator validator = new DefaultValidator();


        private static List<FileCabinetRecord> list = new ();

        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Init(args);

            GenerateData();

            var writtenAmountOfRecords = outputType switch
            {
                //Csv => ExportToCsv(list),
                //Xml => ExportToXml(list),
                _ => recordsAmount,
            };

            Console.WriteLine("{0} records were written to {1}.", writtenAmountOfRecords, outputFile);
        }

        /// <summary>
        /// Exports records to an xml-file.
        /// </summary>
        /// <param name="records">A <see cref="List"/> of <see cref="FileCabinetRecord"/> records.</param>
        /// <returns>The <see cref="int"/> instance of the total exported records.</returns>
        private static int ExportToXml(List<FileCabinetRecord> records)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Exports records to a csv-file.
        /// </summary>
        /// <param name="records">A <see cref="List"/> of <see cref="FileCabinetRecord"/> records.</param>
        /// <returns>The <see cref="int"/> instance of the total exported records.</returns>
        private static int ExportToCsv(List<FileCabinetRecord> records)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates records data.
        /// </summary>
        private static void GenerateData()
        {
            Random random = new ();


            for (int i = 0; i < recordsAmount; i++)
            {
                FileCabinetRecord record = new ();
                record.Id = startId;
                record.FirstName = RandomString(random.Next(validator.FirstNameMinLength, validator.FirstNameMaxLength));
                record.LastName = RandomString(random.Next(validator.LastNameMinLength, validator.LastNameMaxLength));
                record.DateOfBirth = new DateTime(random.Next(validator.MinDate.Year, DateTime.Today.Year),
                    random.Next(validator.MinDate.Month, DateTime.Today.Month),
                    random.Next(validator.MinDate.Day, DateTime.Today.Day));
                record.WorkPlaceNumber = (short)random.Next(short.MaxValue);
                record.Salary = Convert.ToDecimal(random.Next());
                record.Department = Convert.ToChar(random.Next(26) + 65);

                list.Add(record);
                startId++;
            }

            string RandomString(int length)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            }
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
                int value = 0;
                if (parsedArgsDictionary.TryGetValue(flag, out strValue)
                    && int.TryParse(strValue, out value))
                {
                    recordsAmount = value;
                }
            }

            foreach (var flag in StartIdFlags)
            {
                var strValue = string.Empty;
                int value = 0;
                if (parsedArgsDictionary.TryGetValue(flag, out strValue)
                    && int.TryParse(strValue, out value))
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