using FileCabinetApp.FileCabinetService;
using FileCabinetApp.Models;
using FileCabinetApp.StaticClasses;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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
        private static readonly string[] ValidationRulesFlags = { "--validation-rules", "-v" };

        private static readonly List<FileCabinetRecord> list = new ();
        
        private const string Csv = "csv";
        private const string Xml = "xml";
        private static string outputType = Csv;
        private static string outputFile = "data.csv";
        private static int recordsAmount = 0;
        private static int startId = 0;
        private const string ValidationRulesFile = "validation-rules.json";
        private static string ValidationRulesSection = "default";

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
            var rules = GetValidationRules(ValidationRulesSection);

            if (rules == null)
            {
                Console.WriteLine("Error: validation-rules.json is not found.");
                return;
            }

            Random random = new ();


            for (int i = 0; i < recordsAmount; i++)
            {
                var ticksFrom = rules.DateOfBirth.From.Ticks;
                var ticksTo = rules.DateOfBirth.To.Ticks;

                FileCabinetRecord record = new ()
                {
                    Id = startId,
                    FirstName = RandomString(random.Next(rules.FirstName.Min, rules.FirstName.Max)),
                    LastName = RandomString(random.Next(rules.LastName.Min, rules.LastName.Max)),
                    DateOfBirth = new DateTime(random.NextInt64(ticksFrom, ticksTo)),
                    Workplace = (short)random.Next(rules.Workplace.Min, rules.Workplace.Max),
                    Salary = Convert.ToDecimal(random.Next(Convert.ToInt32(rules.Salary.Min), Convert.ToInt32(rules.Salary.Max))),
                    Department = Convert.ToChar(random.Next(rules.Department.Start, rules.Department.End))
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
            var parsedArgsDictionary = Parser.ParseArgs(args);

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

            foreach (var flag in ValidationRulesFlags)
            {
                if (parsedArgsDictionary.TryGetValue(flag, out string? strValue)
                    && (strValue != null))
                {
                    ValidationRulesSection = strValue;
                }
            }
        }

        private static ValidationRules? GetValidationRules(string section)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(ValidationRulesFile, true, true)
                .Build();

            return configuration.GetSection(section).Get<ValidationRules>();
        }
    }
}