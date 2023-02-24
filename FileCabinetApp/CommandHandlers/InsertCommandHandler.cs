using System;
using System.Collections.Generic;
using System.Globalization;
using FileCabinetApp.FileCabinetService;
using FileCabinetApp.StaticClasses;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles the Insert Command Request.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        private const string Trigger = "insert";
        private const string Id = "id";
        private const string FirstName = "firstname";
        private const string LastName = "lastname";
        private const string DateOfBirth = "dateofbirth";
        private const string Workplace = "workplace";
        private const string Salary = "salary";
        private const string Department = "department";

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">A <see cref="IFileCabinetService"/> specialized instance.</param>
        public InsertCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                this.Insert(appCommandRequest.Parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static FileCabinetRecord? ParseArgs(string? args)
        {
            char[] trimChars = new char[] { ',', '(', ')', '\'', ' ', '"' };

            if (args == null)
            {
                return null;
            }

            var splittedArgs = args.Split(trimChars, StringSplitOptions.RemoveEmptyEntries);
            if (splittedArgs.Length != 15)
            {
                return null;
            }

            Dictionary<string, string> argsDict = new ();
            for (int i = 0; i < 7; i++)
            {
                argsDict[splittedArgs[i].ToLower(CultureInfo.InvariantCulture)] = splittedArgs[i + 8];
            }

            bool successfulConversion = true;
            FileCabinetRecord record = new ()
            {
                Id = ReadValue(Converter.IntConverter, argsDict[Id]),
                FirstName = ReadValue(Converter.StringConverter, argsDict[FirstName]),
                LastName = ReadValue(Converter.StringConverter, argsDict[LastName]),
                DateOfBirth = ReadValue(Converter.DateConverter, argsDict[DateOfBirth]),
                WorkPlaceNumber = ReadValue(Converter.ShortConverter, argsDict[Workplace]),
                Salary = ReadValue(Converter.DecimalConverter, argsDict[Salary]),
                Department = ReadValue(Converter.CharConverter, argsDict[Department]),
            };

            T ReadValue<T>(Func<string?, Tuple<bool, string, T>> converter, string input)
            {
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}.");
                }

                successfulConversion = conversionResult.Item1 && successfulConversion;
                return conversionResult.Item3;
            }

            return successfulConversion ? record : null;
        }

        private void Insert(string args)
        {
            // Parse parameters
            var record = ParseArgs(args);
            if (record == null)
            {
                Console.WriteLine("Incorrect insert parameters. Example of correct parameters: ");
                Console.WriteLine($"insert ({Id}, {FirstName}, {LastName}, {DateOfBirth}, {Workplace}, {Salary}, {Department}) values ('1', 'John', 'Doe', '01.01.2000', '5', '1000', 'A')");
                return;
            }

            // call insert(record)
            var result = this.service.Insert(record);
            if (result)
            {
                Console.WriteLine("Record #{0} is inserted.", record.Id);
            }
            else
            {
                Console.WriteLine("Record is not inserted.");
            }
        }
    }
}
