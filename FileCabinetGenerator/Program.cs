using FileCabinetApp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

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

        private static readonly List<FileCabinetRecord> list = new ();
        
        private const string Csv = "csv";
        private const string Xml = "xml";
        private static string outputType = Csv;
        private static string outputFile = "data.csv";
        private static int recordsAmount = 0;
        private static int startId = 0;

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
                Csv => ExportToCsv(),
                Xml => ExportToXml(),
                _ => recordsAmount,
            };

            Console.WriteLine("{0} records were written to {1}.", writtenAmountOfRecords, outputFile);
        }

        /// <summary>
        /// Exports records to an xml-file.
        /// </summary>
        /// <returns>The <see cref="int"/> instance of the total exported records.</returns>
        private static int ExportToXml()
        {
            var xOver = new XmlAttributeOverrides();
            var attrs = new XmlAttributes();
            var xRoot = new XmlRootAttribute
            {
                ElementName = "records"
            };
            attrs.XmlRoot = xRoot;

            xOver.Add(typeof(List<FileCabinetRecord>), attrs);

            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using (var sw = File.Create(outputFile))
            {
                var serializer = new XmlSerializer(typeof(List<FileCabinetRecord>), xOver);
                serializer.Serialize(sw, list, ns);
            }

            return list.Count;
        }

        /// <summary>
        /// Exports records to a csv-file.
        /// </summary>
        /// <returns>The <see cref="int"/> instance of the total exported records.</returns>
        private static int ExportToCsv()
        {
            int counter = 0;
            using (var sw = File.CreateText(outputFile))
            {
                var writer = new FileCabinetRecordCsvWriter(sw);
                foreach (var record in list)
                {
                    writer.Write(record);
                    counter++;
                }
            }

            return counter;
        }

        /// <summary>
        /// Generates records data.
        /// </summary>
        private static void GenerateData()
        {
            Random random = new ();


            for (int i = 0; i < recordsAmount; i++)
            {
                FileCabinetRecord record = new ()
                {
                    Id = startId,
                    FirstName = RandomString(random.Next(2, 60)),
                    LastName = RandomString(random.Next(2, 60)),
                    DateOfBirth = new DateTime(random.Next(1950, DateTime.Today.Year),
                    random.Next(1, DateTime.Today.Month),
                    random.Next(1, DateTime.Today.Day)),
                    WorkPlaceNumber = (short)random.Next(short.MaxValue),
                    Salary = Convert.ToDecimal(random.Next()),
                    Department = Convert.ToChar(random.Next(26) + 65)
                };

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
                if (parsedArgsDictionary.TryGetValue(flag, out string? value))
                {
                    outputType = value;
                }
            }

            foreach (var flag in OutputFileFlags)
            {
                if (parsedArgsDictionary.TryGetValue(flag, out string? value))
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
                if (parsedArgsDictionary.TryGetValue(flag, out string? strValue)
                    && int.TryParse(strValue, out int value))
                {
                    recordsAmount = value;
                }
            }

            foreach (var flag in StartIdFlags)
            {
                if (parsedArgsDictionary.TryGetValue(flag, out string? strValue)
                    && int.TryParse(strValue, out int value))
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