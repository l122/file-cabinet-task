using System;
using System.Collections.Generic;
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

            var commandHandler = CreateCommandHandlers(fileCabinetService);

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

                const int parametersIndex = 1;
                var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;

                commandHandler.Handle(new AppCommandRequest(command, parameters));
            }
            while (isRunning);
        }

        /// <summary>
        /// Creates the <see cref="ICommandHandler"/> Specialized Objects.
        /// </summary>
        /// <returns>The <see cref="HelpCommandHandler"/> instance.</returns>
        private static ICommandHandler CreateCommandHandlers(IFileCabinetService fileCabinetService)
        {
            var helpHandler = new HelpCommandHandler();
            var createHandler = new CreateCommandHandler(fileCabinetService);
            var editHandler = new EditCommandHandler(fileCabinetService);
            var statHandler = new StatCommandHandler(fileCabinetService);
            var listHandler = new ListCommandHandler(fileCabinetService);
            var findHandler = new FindCommandHandler(fileCabinetService);
            var removeHandler = new RemoveCommandHandler(fileCabinetService);
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var exitHandler = new ExitCommandHandler(p => isRunning = p);
            var missedCommandHandler = new MissedCommandHandler();

            helpHandler.SetNext(createHandler);
            createHandler.SetNext(editHandler);
            editHandler.SetNext(statHandler);
            statHandler.SetNext(listHandler);
            listHandler.SetNext(findHandler);
            findHandler.SetNext(removeHandler);
            removeHandler.SetNext(purgeHandler);
            purgeHandler.SetNext(exportHandler);
            exportHandler.SetNext(importHandler);
            importHandler.SetNext(exitHandler);
            exitHandler.SetNext(missedCommandHandler);

            return helpHandler;
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