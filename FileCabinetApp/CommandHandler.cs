using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    public class CommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

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
            new Tuple<string, Action<string>>("import", Import),
            new Tuple<string, Action<string>>("remove", Remove),
            new Tuple<string, Action<string>>("purge", Purge),
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
            new string[] { "export", "exports records to csv or xml", "The 'export <csv, xml> <file_name>' command exports records to a csv or xml file." },
            new string[] { "import", "imports records from csv or xml", "The 'import <csv, xml> <file_name>' command imports records from a csv or xml file." },
            new string[] { "remove", "removes a record", "The 'remove #id' command removes a records." },
            new string[] { "purge", "removes records marked as deleted from the database", "The 'purge' command removes records marked as deleted from the database." },
        };

        public override void Handle(AppCommandRequest appCommandRequest)
        {
            var index = Array.FindIndex(Commands, 0, Commands.Length, i => i.Item1.Equals(appCommandRequest.Command, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                Commands[index].Item2(appCommandRequest.Parameters);
            }
            else
            {
                PrintMissedCommandInfo(appCommandRequest.Command);
            }
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
                var snapshot = Program.fileCabinetService.MakeSnapshot();

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

        private static void Import(string parameters)
        {
            const string csvParameter = "csv";
            const string xmlParameter = "xml";
            int oldQuantity = Program.fileCabinetService.GetStat().Item1;

            var input = parameters.Split(" ");
            if (input.Length != 2)
            {
                Console.WriteLine("Invalid parameters.");
                Console.WriteLine("Use syntax 'import <csv, xml> <file_name>'");
                return;
            }

            // Create / open file
            string? file = input[1];
            if (!File.Exists(file))
            {
                Console.WriteLine("Import error: file {0} does not exist.", file);
                return;
            }

            try
            {
                using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                var snapshot = new FileCabinetServiceSnapshot();

                string parameter = input[0];
                switch (parameter)
                {
                    case csvParameter:
                        snapshot.LoadFromCsv(fileStream);
                        break;
                    case xmlParameter:
                        snapshot.LoadFromXml(fileStream);
                        break;
                    default:
                        Console.WriteLine("Invalid parameters.");
                        break;
                }

                Program.fileCabinetService.Restore(snapshot);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.ToString());
                return;
            }

            Console.WriteLine("{0} records were imported from {1}", Program.fileCabinetService.GetStat().Item1 - oldQuantity, file);
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
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][ExplanationHelpIndex]);
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
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            Program.isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine("{0} record(s), {1} deleted record(s).", recordsCount.Item1, recordsCount.Item2);
        }

        private static void Create(string parameters)
        {
            var id = Program.fileCabinetService.CreateRecord();
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
            PrintRecords(Program.fileCabinetService.GetRecords());
        }

        private static void Edit(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Incorrect id parameter: {0}", parameters);
                return;
            }

            Program.fileCabinetService.EditRecord(id);
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
                        PrintRecords(Program.fileCabinetService.FindByFirstName(criterion));
                        break;

                    case lastNameField:
                        PrintRecords(Program.fileCabinetService.FindByLastName(criterion));
                        break;

                    case dateOfBirthField:
                        PrintRecords(Program.fileCabinetService.FindByDateOfBirth(criterion));
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

        private static void Remove(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Incorrect id parameter: {0}", parameters);
                return;
            }

            Program.fileCabinetService.RemoveRecord(id);
        }

        private static void Purge(string parameters)
        {
            Program.fileCabinetService.Purge();
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

    }
}
