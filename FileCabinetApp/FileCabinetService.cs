using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Base class for File Cabinet Service classes.
    /// It provides common methods and fields.
    /// </summary>
    public class FileCabinetService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// </summary>
        /// <param name="value">The <see cref="IRecordValidator"/> specialized instance.</param>
        protected FileCabinetService(IRecordValidator value)
        {
            this.Validator = value;
        }

        /// <summary>
        /// Gets the <see cref="IRecordValidator"/> specialized instance.
        /// </summary>
        /// <value>The <see cref="IRecordValidator"/> specialized instance.</value>
        protected IRecordValidator Validator { get; init; }

        /// <summary>
        /// Converts a <see cref="string"/> argument into a <see cref="char"/>.
        /// </summary>
        /// <param name="arg">The nullable <see cref="char"/> argument.</param>
        /// <returns>The <see cref="Tuple{T1, T2, T3}"/> value.</returns>
        protected static Tuple<bool, string, char> CharConverter(string? arg)
        {
            if (!char.TryParse(arg, out char result))
            {
                return Tuple.Create(false, $"{arg} should be a letter.", default(char));
            }

            return Tuple.Create(true, string.Empty, result);
        }

        /// <summary>
        /// Converts a <see cref="string"/> argument into a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="arg">The nullable <see cref="DateTime"/> argument.</param>
        /// <returns>The <see cref="Tuple{T1, T2, T3}"/> value.</returns>
        protected static Tuple<bool, string, DateTime> DateConverter(string? arg)
        {
            if (!DateTime.TryParse(arg, out DateTime dateOfBirth))
            {
                return Tuple.Create(false, $"{arg} is an invalid DateTime.", dateOfBirth);
            }

            return Tuple.Create(true, string.Empty, dateOfBirth);
        }

        /// <summary>
        /// Converts a <see cref="string"/> argument into a <see cref="decimal"/>.
        /// </summary>
        /// <param name="arg">The nullable <see cref="decimal"/> argument.</param>
        /// <returns>The <see cref="Tuple{T1, T2, T3}"/> value.</returns>
        protected static Tuple<bool, string, decimal> DecimalConverter(string? arg)
        {
            if (!decimal.TryParse(arg, CultureInfo.InvariantCulture, out decimal result))
            {
                return Tuple.Create(false, $"{arg} is not a valid decimal type.", result);
            }

            return Tuple.Create(true, string.Empty, result);
        }

        /// <summary>
        /// Converts a <see cref="string"/> argument into a <see cref="short"/>.
        /// </summary>
        /// <param name="arg">The nullable <see cref="short"/> argument.</param>
        /// <returns>The <see cref="Tuple{T1, T2, T3}"/> value.</returns>
        protected static Tuple<bool, string, short> ShortConverter(string? arg)
        {
            if (!short.TryParse(arg, CultureInfo.InvariantCulture, out short result))
            {
                return Tuple.Create(false, $"{arg} is not a valid short number.", result);
            }

            return Tuple.Create(true, string.Empty, result);
        }

        /// <summary>
        /// Converts a <see cref="string"/> argument into a <see cref="string"/>.
        /// </summary>
        /// <param name="arg">The nullable <see cref="string"/> argument.</param>
        /// <returns>The <see cref="Tuple{T1, T2, T3}"/> value.</returns>
        protected static Tuple<bool, string, string> StringConverter(string? arg)
        {
            if (arg == null)
            {
                return Tuple.Create(false, "String cannot be empty.", string.Empty);
            }

            return Tuple.Create(true, string.Empty, arg);
        }

        /// <summary>
        /// Reads input in a loop until the input data is acquired and validated.
        /// </summary>
        /// <typeparam name="T">The type to be read to.</typeparam>
        /// <param name="converter">Converts a string into a needed type.</param>
        /// <param name="validator">Validates input type with rules.</param>
        /// <returns>The <c>T</c>-type instance of input data.</returns>
        protected static T ReadInput<T>(Func<string?, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        /// <summary>
        /// Gets input data from console and creates a new record with id = 0.
        /// </summary>
        /// <returns>The <see cref="FileCabinetRecord"/> instance with id = 0.</returns>
        protected FileCabinetRecord GetInputData()
        {
            Console.Write("First name: ");
            string firstName = ReadInput(StringConverter, this.Validator.ValidateParameters);

            Console.Write("Last name: ");
            string lastName = ReadInput(StringConverter, this.Validator.ValidateParameters);

            Console.Write("Date of birth: ");
            DateTime dateOfBirth = ReadInput(DateConverter, this.Validator.ValidateParameters);

            Console.Write("Work Place Number: ");
            short workPlaceNumber = ReadInput(ShortConverter, this.Validator.ValidateParameters);

            Console.Write("Salary: ");
            decimal salary = ReadInput(DecimalConverter, this.Validator.ValidateParameters);

            Console.Write("Department (one letter): ");
            char department = ReadInput(CharConverter, this.Validator.ValidateParameters);

            return new FileCabinetRecord()
            {
                Id = 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                WorkPlaceNumber = workPlaceNumber,
                Salary = salary,
                Department = department,
            };
        }

        /// <summary>
        /// Validates the <see cref="FileCabinetRecord"/> instance.
        /// </summary>
        /// <param name="record">A <see cref="FileCabinetRecord"/> instance.</param>
        /// <returns>true if record is valid, false otherwise.</returns>
        protected bool IsValidRecord(FileCabinetRecord record)
        {
            // Validate First Name
            var validationResult = this.Validator.FirstNameValidator(record.FirstName);
            if (!validationResult.Item1)
            {
                Console.WriteLine("#{0}: {1}", record.Id, validationResult.Item2);
                return false;
            }

            // Validate Last Name
            validationResult = this.Validator.LastNameValidator(record.LastName);
            if (!validationResult.Item1)
            {
                Console.WriteLine("#{0}: {1}", record.Id, validationResult.Item2);
                return false;
            }

            // Date of birth validator
            validationResult = this.Validator.DateOfBirthValidator(record.DateOfBirth);
            if (!validationResult.Item1)
            {
                Console.WriteLine("#{0}: {1}", record.Id, validationResult.Item2);
                return false;
            }

            return true;
        }
    }
}