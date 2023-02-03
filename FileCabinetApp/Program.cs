using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

[assembly: CLSCompliant(true)]

namespace FileCabinetApp
{
    /// <summary>
    /// The class that contains the Main method.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Oleg Shkadov";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static readonly string[] ValidationRulesFlags = { "--validation-rules", "-v" };
        private static readonly string[] StorageFlags = { "--storage", "-s" };

        private static readonly Tuple<string, Func<IRecordValidator>>[] Validators = new Tuple<string, Func<IRecordValidator>>[]
        {
            new Tuple<string, Func<IRecordValidator>>("default", GetDefaultValidatorObject),
            new Tuple<string, Func<IRecordValidator>>("custom", GetCustomValidatorObject),
        };

        private static readonly Tuple<string, Func<IRecordValidator, IFileCabinetService>>[] MemorySystems = new Tuple<string, Func<IRecordValidator, IFileCabinetService>>[]
        {
            new Tuple<string, Func<IRecordValidator, IFileCabinetService>>("memory", GetFileCabinetMemoryServiceObject),
            new Tuple<string, Func<IRecordValidator, IFileCabinetService>>("file", GetFileCabinetFilesystemServiceObject),
        };

        private static readonly Tuple<string, Action<string>>[] Commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
        };

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints the number of records", "The 'stat' command prints the number of records." },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "list", "prints all records", "The 'list' command prints all records." },
            new string[] { "edit", "edits a record", "The 'edit #id' command edits record #id." },
            new string[] { "find", "searches records", "The 'find <firstname, lastname, dateofbirth> <criterion>' command searches all records with <field> = <criterion>." },
            new string[] { "export", "exports records to csv or xml", "The 'export <csv, xml> <file_name>' command exports records to a csv or xml file" },
        };

        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());

        private static bool isRunning = true;

        /// <summary>
        /// The Main method class of the program.
        /// </summary>
        /// <param name="args">The <see cref="string"/> array instance of input arguments.</param>
        public static void Main(string[] args)
        {
            Init(args);

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                var inputs = line != null ? line.Split(' ', 2) : new string[] { string.Empty, string.Empty };
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(Commands, 0, Commands.Length, i => i.Item1.Equals(command, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    Commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void Export(string parameters)
        {
            const string csvParameter = "csv";
            const string xmlParameter = "xml";
            const string yes = "Y";

            var input = parameters.Split(" ");
            if (input.Length != 2)
            {
                Console.WriteLine("Invalid parameters.");
                Console.WriteLine("Use syntax 'export <csv, xml> <file_name>'");
                return;
            }

            // Create / open file
            string file = input[1];
            if (File.Exists(file))
            {
                Console.Write("File alredy exists. Rewrite '{0}'? [Y/n] ", file);
                var response = Console.ReadLine();
                if (!string.Equals(response, yes, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            StreamWriter? sw = null;
            try
            {
                //// Create stream writer
                sw = File.CreateText(file);

                //// Make Snapshot
                var snapshot = fileCabinetService.MakeSnapshot();

                string parameter = input[0];
                switch (parameter)
                {
                    case csvParameter:
                        snapshot.SaveToCsv(sw);
                        break;
                    case xmlParameter:
                        snapshot.SaveToXml(sw);
                        break;
                    default:
                        Console.WriteLine("Invalid parameters.");
                        break;
                }

                //// Close stream writer
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Export failed: can't open file {0}.", file);
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            var id = fileCabinetService.CreateRecord();
            if (id > 0)
            {
                Console.WriteLine("Record #{0} is created.", id);
            }
            else
            {
                Console.WriteLine("Record is not created.");
            }
        }

        private static void List(string parameters)
        {
            PrintRecords(fileCabinetService.GetRecords());
        }

        private static void Edit(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            if (!int.TryParse(parameters, out int id) || id < 1 || id > recordsCount)
            {
                Console.WriteLine("#{0} record is not found.", id);
                return;
            }

            fileCabinetService.EditRecord(id);
        }

        private static void Find(string parameters)
        {
            const string firstNameField = "firstname";
            const string lastNameField = "lastname";
            const string dateOfBirthField = "dateofbirth";

            var input = parameters.Split(" ");
            if (input.Length != 2)
            {
                Console.WriteLine("Invalid parameters.");
                Console.WriteLine("Use syntax 'find <firstname, lastname, dateofbirth> <criterion>'");
                return;
            }

            string field = input[0].ToLower(CultureInfo.InvariantCulture);
            string criterion = input[1].Trim('"');
            try
            {
                switch (field)
                {
                    case firstNameField:
                        PrintRecords(fileCabinetService.FindByFirstName(criterion));
                        break;

                    case lastNameField:
                        PrintRecords(fileCabinetService.FindByLastName(criterion));
                        break;

                    case dateOfBirthField:
                        PrintRecords(fileCabinetService.FindByDateOfBirth(criterion));
                        break;

                    default:
                        Console.WriteLine("Invalid parameters.");
                        Console.WriteLine("Use syntax 'find <firstname, lastname, dateofbirth> <criterion>'");
                        break;
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void PrintRecords(ReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records == null)
            {
                return;
            }

            foreach (var record in records)
            {
                Console.WriteLine(record.ToString());
            }
        }

        /// <summary>
        /// Sets initial values according to application parameters.
        /// </summary>
        /// <param name="args">The <see cref="string"/> array instance input parameters.</param>
        private static void Init(string[] args)
        {
            var parsedArgsDictionary = ParseArgs(args);

            int validatorIndex = 0;
            foreach (var flag in ValidationRulesFlags)
            {
                var value = string.Empty;
                if (parsedArgsDictionary.TryGetValue(flag, out value))
                {
                    validatorIndex = Array.FindIndex(Validators, 0, Validators.Length, p => p.Item1.Equals(value, StringComparison.OrdinalIgnoreCase));
                    if (validatorIndex == -1)
                    {
                        validatorIndex = 0;
                    }
                }
            }

            var validator = Validators[validatorIndex].Item2();

            int memoryIndex = 0;
            foreach (var flag in StorageFlags)
            {
                var value = string.Empty;
                if (parsedArgsDictionary.TryGetValue(flag, out value))
                {
                    memoryIndex = Array.FindIndex(MemorySystems, 0, MemorySystems.Length, p => p.Item1.Equals(value, StringComparison.OrdinalIgnoreCase));
                    if (memoryIndex == -1)
                    {
                        memoryIndex = 0;
                    }
                }
            }

            fileCabinetService = MemorySystems[memoryIndex].Item2(validator);
        }

        private static IFileCabinetService GetFileCabinetFilesystemServiceObject(IRecordValidator validator)
        {
            return new FileCabinetFilesystemService(validator);
        }

        private static IFileCabinetService GetFileCabinetMemoryServiceObject(IRecordValidator validator)
        {
            return new FileCabinetMemoryService(validator);
        }

        private static IRecordValidator GetCustomValidatorObject()
        {
            return new CustomValidator();
        }

        private static IRecordValidator GetDefaultValidatorObject()
        {
            return new DefaultValidator();
        }

        /// <summary>
        /// Parses input arguments input a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="args">The <see cref="string"/> array instance.</param>
        /// <returns>The <see cref="Dictionary{TKey, TValue}"/> object.</returns>
        private static Dictionary<string, string> ParseArgs(string[] args)
        {
            Dictionary<string, string> result = new ();
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