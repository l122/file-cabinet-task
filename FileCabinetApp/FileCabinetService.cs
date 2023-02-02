using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// The <see cref="IRecordValidator"/> specialized instance.
        /// </summary>
        private readonly IRecordValidator validator;
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary;
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary;
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// </summary>
        /// <param name="value">The <see cref="IRecordValidator"/> specialized instance.</param>
        protected FileCabinetService(IRecordValidator value)
        {
            this.validator = value;
            this.firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
            this.lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
            this.dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        }

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
            string firstName = ReadInput(StringConverter, this.validator.FirstNameValidator);

            Console.Write("Last name: ");
            string lastName = ReadInput(StringConverter, this.validator.LastNameValidator);

            Console.Write("Date of birth: ");
            DateTime dateOfBirth = ReadInput(DateConverter, this.validator.DateOfBirthValidator);

            Console.Write("Work Place Number: ");
            short workPlaceNumber = ReadInput(ShortConverter, this.validator.WorkPlaceValidator);

            Console.Write("Salary: ");
            decimal salary = ReadInput(DecimalConverter, this.validator.SalaryValidator);

            Console.Write("Department (one letter): ");
            char department = ReadInput(CharConverter, this.validator.DepartmentValidator);

            return new FileCabinetRecord()
            {
                Id = 0,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                WorkPlaceNumber = workPlaceNumber,
                Salary = salary,
                Department = department,
            };
        }

        /// <summary>
        /// Searches for a record by first name.
        /// </summary>
        /// <param name="firstName">The <see cref="string"/> instance of the first name.</param>
        /// <returns>A read-only instance of all matched records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.TryGetValue(firstName.ToUpperInvariant(), out List<FileCabinetRecord>? result))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(result);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
        }

        /// <summary>
        /// Searches for a record by last name.
        /// </summary>
        /// <param name="lastName">The <see cref="string"/> instance of the last name.</param>
        /// <returns>A read-only instance of all matched records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.TryGetValue(lastName.ToUpperInvariant(), out List<FileCabinetRecord>? result))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(result);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
        }

        /// <summary>
        /// Searches for a record by date of birth.
        /// </summary>
        /// <param name="dateOfBirthString">The <see cref="string"/> instance of the date of birth.</param>
        /// <returns>A read-only instance of all matched records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString)
        {
            if (DateTime.TryParse(dateOfBirthString, out DateTime dateOfBirth))
            {
                dateOfBirthString = dateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
            }

            if (this.dateOfBirthDictionary.TryGetValue(dateOfBirthString, out List<FileCabinetRecord>? result))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(result);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
        }

        /// <summary>
        /// Adds a record data to the search dictionaries.
        /// </summary>
        /// <param name="record">The <see cref="FileCabinetRecord"/> instance.</param>
        protected void AddRecordToSearchDictionaries(in FileCabinetRecord record)
        {
            // Add record to firstNameDictionary
            if (this.firstNameDictionary.TryGetValue(record.FirstName.ToUpperInvariant(), out var value))
            {
                value.Add(record);
            }
            else
            {
                this.firstNameDictionary.Add(record.FirstName.ToUpperInvariant(), new List<FileCabinetRecord> { record });
            }

            // Add record to lastNameDictionary
            if (this.lastNameDictionary.TryGetValue(record.LastName.ToUpperInvariant(), out value))
            {
                value.Add(record);
            }
            else
            {
                this.lastNameDictionary.Add(record.LastName.ToUpperInvariant(), new List<FileCabinetRecord> { record });
            }

            // Add record to dateOfBirthDictionary
            string dateOfBirthString = record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
            if (this.dateOfBirthDictionary.TryGetValue(dateOfBirthString, out value))
            {
                value.Add(record);
            }
            else
            {
                this.dateOfBirthDictionary.Add(dateOfBirthString, new List<FileCabinetRecord> { record });
            }
        }

        /// <summary>
        /// Removes records that match id from all searching dictionaries.
        /// </summary>
        /// <param name="record">The <see cref="FileCabinetRecord"/> id input.</param>
        protected void RemoveRecordFromSearchDictionaries(FileCabinetRecord record)
        {
            // Update firstNameDictionary
            var recordList = this.firstNameDictionary[record.FirstName.ToUpperInvariant()];
            recordList.RemoveAll(p => p.Id == record.Id);
            if (recordList.Count == 0)
            {
                this.firstNameDictionary.Remove(record.FirstName.ToUpperInvariant());
            }

            // Update lastNameDictionary
            recordList = this.lastNameDictionary[record.LastName.ToUpperInvariant()];
            recordList.RemoveAll(p => p.Id == record.Id);
            if (recordList.Count == 0)
            {
                this.lastNameDictionary.Remove(record.LastName.ToUpperInvariant());
            }

            // Update dateOfBirthDictionary
            string dateOfBirthString = record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
            recordList = this.dateOfBirthDictionary[dateOfBirthString];
            recordList.RemoveAll(p => p.Id == record.Id);
            if (recordList.Count == 0)
            {
                this.dateOfBirthDictionary.Remove(dateOfBirthString);
            }
        }
    }
}