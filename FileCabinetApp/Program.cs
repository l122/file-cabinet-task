using System;
using System.Diagnostics.Metrics;
using System.Globalization;
using Microsoft.VisualBasic;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Oleg Shkadov";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static FileCabinetService fileCabinetService = new ();
        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints the number of records", "The 'stat' command prints the number of records." },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "list", "prints all records", "The 'list' command prints all records." },
            new string[] { "edit", "edits a record", "The 'edit #id' command edits record #id." },
            new string[] { "find", "searches records", "The 'find <field> <criterion>' command searches all records with <field> = <criterion>." },
        };

        public static void Main(string[] args)
        {
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

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
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
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
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
            bool isDone = false;
            while (!isDone)
            {
                GetData(
                    out string? firstName,
                    out string? lastName,
                    out string? dateOfBirth,
                    out string? age,
                    out string? savings,
                    out string? letter);

                try
                {
                    Console.WriteLine(
                        "Record #{0} is created.",
                        Program.fileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, age, savings, letter));
                    isDone = true;
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine("\nERROR: " + e.Message);
                    isDone = CheckIfEscPressed();
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("\nERROR: " + e.Message);
                    isDone = CheckIfEscPressed();
                }
                catch (Exception e)
                {
                    Console.WriteLine("\n" + e.Message);
                    isDone = CheckIfEscPressed();
                }
            }

            bool CheckIfEscPressed()
            {
                Console.Write("Press ESC to cancel entry or any other key to try again...");
                var key = Console.ReadKey(true);
                Console.WriteLine(Environment.NewLine);

                return key.Key == ConsoleKey.Escape;
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

            GetData(
                out string? firstName,
                out string? lastName,
                out string? dateOfBirth,
                out string? age,
                out string? savings,
                out string? letter);

            try
            {
                fileCabinetService.EditRecord(id, firstName, lastName, dateOfBirth, age, savings, letter);
                Console.WriteLine("Record #{0} is updated.", id);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void Find(string parameters)
        {
            const string firstNameField = "firstname";
            const string lastNameField = "lastname";

            var input = parameters.Split(" ");
            if (input.Length != 2)
            {
                Console.WriteLine("Invalid parameters.");
                Console.WriteLine("Use syntax 'find <field> <criterion>'");
                return;
            }

            string field = input[0].ToLower(CultureInfo.InvariantCulture);
            string criterion = input[1].Trim('"');
            FileCabinetRecord[] foundRecords;
            try
            {
                switch (field)
                {
                    case firstNameField:
                        foundRecords = fileCabinetService.FindByFirstName(criterion);
                        PrintRecords(foundRecords);
                        break;

                    case lastNameField:
                        foundRecords = fileCabinetService.FindByLastName(criterion);
                        PrintRecords(foundRecords);
                        break;

                    default:
                        Console.WriteLine("Invalid parameters.");
                        Console.WriteLine("Use syntax 'find <field> <criterion>'");
                        break;
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void GetData(
            out string? firstName,
            out string? lastName,
            out string? dateOfBirth,
            out string? age,
            out string? savings,
            out string? letter)
        {
            Console.Write("First name: ");
            firstName = Console.ReadLine();
            Console.Write("Last name: ");
            lastName = Console.ReadLine();
            Console.Write("Date of birth: ");
            dateOfBirth = Console.ReadLine();
            Console.Write("Age: ");
            age = Console.ReadLine();
            Console.Write("Savings: ");
            savings = Console.ReadLine();
            Console.Write("Favorite English letter: ");
            letter = Console.ReadLine();
        }

        private static void PrintRecords(FileCabinetRecord[] records)
        {
            foreach (var record in records)
            {
                Console.WriteLine(record.ToString());
            }
        }
    }
}