using System;
using System.Collections.Generic;
using System.Globalization;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.FileCabinetService;
using FileCabinetApp.Loggers;
using FileCabinetApp.StaticClasses;
using FileCabinetApp.Validators;

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
        private const string UseStopwatch = "--use-stopwatch";
        private const string UseLogger = "--use-logger";

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

        private static IRecordValidator validator = new CompositeValidator(new List<IRecordValidator>());
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(validator);
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

            Init(args);

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
        /// Gets input data from console and creates a new record with id = 0.
        /// </summary>
        /// <returns>The <see cref="FileCabinetRecord"/> instance with id = 0.</returns>
        public static FileCabinetRecord GetInputData()
        {
            FileCabinetRecord record;

            do
            {
                Console.Write("First name: ");
                string firstName = ReadInput(Converter.StringConverter);

                Console.Write("Last name: ");
                string lastName = ReadInput(Converter.StringConverter);

                Console.Write("Date of birth: ");
                DateTime dateOfBirth = ReadInput(Converter.DateConverter);

                Console.Write("Work Place Number: ");
                short workPlaceNumber = ReadInput(Converter.ShortConverter);

                Console.Write("SalaryType: ");
                decimal salary = ReadInput(Converter.DecimalConverter);

                Console.Write("DepartmentType (one letter): ");
                char department = ReadInput(Converter.CharConverter);

                record = new FileCabinetRecord()
                {
                    Id = 1,
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = dateOfBirth,
                    WorkPlaceNumber = workPlaceNumber,
                    Salary = salary,
                    Department = department.ToString().ToUpper(CultureInfo.InvariantCulture)[0],
                };

                var validationResult = validator.ValidateParameters(record);
                if (!validationResult.Item1)
                {
                    Console.WriteLine("Validation failed: {0}", validationResult.Item2);
                    continue;
                }

                return record;
            }
            while (true);
        }

        /// <summary>
        /// Reads input in a loop until the input data is acquired and validated.
        /// </summary>
        /// <typeparam name="T">The type to be read to.</typeparam>
        /// <param name="converter">Converts a string into a needed type.</param>
        /// <returns>The <c>T</c>-type instance of input data.</returns>
        private static T ReadInput<T>(Func<string?, Tuple<bool, string, T>> converter)
        {
            do
            {
                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                return conversionResult.Item3;
            }
            while (true);
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
            var listHandler = new ListCommandHandler(fileCabinetService, DefaultRecordPrint);
            var findHandler = new FindCommandHandler(fileCabinetService, DefaultRecordPrint);
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
            var parsedArgsDictionary = Parser.ParseArgs(args);

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

            validator = Validators[validatorIndex].Item2();

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

            if (parsedArgsDictionary.TryGetValue(UseStopwatch, out var _))
            {
                fileCabinetService = new ServiceMeter(fileCabinetService);
            }

            if (parsedArgsDictionary.TryGetValue(UseLogger, out var _))
            {
                fileCabinetService = new ServiceLogger(fileCabinetService);
            }
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
            return new ValidatorBuilder().CreateCustomValidator();
        }

        private static IRecordValidator GetDefaultValidatorObject()
        {
            return new ValidatorBuilder().CreateDefaultValidator();
        }

        private static void DefaultRecordPrint(IEnumerable<FileCabinetRecord> records)
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