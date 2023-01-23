using System;
using System.Globalization;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Oleg Shkadov";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static FileCabinetService fileCabinetService = new FileCabinetService();
        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints the number of records", "The 'stat' command prints the number of records." },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "list", "prints all records", "The 'list' command prints all records." },
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
                Console.Write("First name: ");
                string? firstName = Console.ReadLine();
                Console.Write("Last name: ");
                string? lastName = Console.ReadLine();
                Console.Write("Date of birth: ");
                string? dateOfBirth = Console.ReadLine();
                Console.Write("Age: ");
                string? age = Console.ReadLine();
                Console.Write("Savings: ");
                string? savings = Console.ReadLine();
                Console.Write("Favorite English letter: ");
                string? letter = Console.ReadLine();

                try
                {
                    Console.WriteLine(
                        "Record #{0} is created.",
                        Program.fileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, age, savings, letter));
                    isDone = true;
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                    isDone = CheckIfEscPressed();
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                    isDone = CheckIfEscPressed();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    isDone = CheckIfEscPressed();
                }
            }
        }

        private static void List(string parameters)
        {
            foreach (var record in fileCabinetService.GetRecords())
            {
                Console.WriteLine(
                    "#{0}, {1}, {2}, {3:yyyy-MMM-dd}, {4}, {5:C2}, {6}",
                    record.Id,
                    record.FirstName,
                    record.LastName,
                    record.DateOfBirth,
                    record.Age,
                    record.Savings,
                    record.Letter);
            }
        }

        private static bool CheckIfEscPressed()
        {
            Console.Write("Press ESC to cancel entry or any other key to try again...");
            var key = Console.ReadKey(true);
            Console.WriteLine();

            return key.Key == ConsoleKey.Escape;
        }
    }
}