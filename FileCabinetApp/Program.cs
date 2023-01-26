using System;
using System.Globalization;

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

        private static readonly FileCabinetService FileCabinetService = new ();
        private static readonly Tuple<string, Action<string>>[] Commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
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
        };

        private static bool isRunning = true;

        /// <summary>
        /// The Main method class of the program.
        /// </summary>
        /// <param name="args">The <see cref="string"/> array instance of input arguments.</param>
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
            var recordsCount = Program.FileCabinetService.GetStat();
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
                    out string? workPlaceNumber,
                    out string? salary,
                    out string? department);

                try
                {
                    Console.WriteLine(
                        "Record #{0} is created.",
                        Program.FileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, workPlaceNumber, salary, department));
                    isDone = true;
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine($"{Environment.NewLine}ERROR: {e.Message}");
                    isDone = CheckIfEscPressed();
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine($"{Environment.NewLine}ERROR: {e.Message}");
                    isDone = CheckIfEscPressed();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{Environment.NewLine}ERROR: {e.Message}");
                    isDone = CheckIfEscPressed();
                }
            }
        }

        private static bool CheckIfEscPressed()
        {
            Console.Write("Press ESC to cancel entry or any other key to try again...");
            var key = Console.ReadKey(true);
            Console.WriteLine(Environment.NewLine);

            return key.Key == ConsoleKey.Escape;
        }

        private static void List(string parameters)
        {
            PrintRecords(FileCabinetService.GetRecords());
        }

        private static void Edit(string parameters)
        {
            var recordsCount = Program.FileCabinetService.GetStat();
            if (!int.TryParse(parameters, out int id) || id < 1 || id > recordsCount)
            {
                Console.WriteLine("#{0} record is not found.", id);
                return;
            }

            GetData(
                out string? firstName,
                out string? lastName,
                out string? dateOfBirth,
                out string? workPlaceNumber,
                out string? salary,
                out string? department);

            try
            {
                FileCabinetService.EditRecord(id, firstName, lastName, dateOfBirth, workPlaceNumber, salary, department);
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
                        PrintRecords(FileCabinetService.FindByFirstName(criterion));
                        break;

                    case lastNameField:
                        PrintRecords(FileCabinetService.FindByLastName(criterion));
                        break;

                    case dateOfBirthField:
                        PrintRecords(FileCabinetService.FindByDateOfBirth(criterion));
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

        private static void GetData(
            out string? firstName,
            out string? lastName,
            out string? dateOfBirth,
            out string? workPlaceNumber,
            out string? salary,
            out string? department)
        {
            Console.Write("First name: ");
            firstName = Console.ReadLine();
            Console.Write("Last name: ");
            lastName = Console.ReadLine();
            Console.Write("Date of birth: ");
            dateOfBirth = Console.ReadLine();
            Console.Write("Work Place Number: ");
            workPlaceNumber = Console.ReadLine();
            Console.Write("Salary: ");
            salary = Console.ReadLine();
            Console.Write("Department (one letter): ");
            department = Console.ReadLine();
        }

        private static void PrintRecords(FileCabinetRecord[] records)
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
    }
}