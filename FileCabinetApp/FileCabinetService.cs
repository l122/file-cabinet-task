using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Helper class that contains method implementations.
    /// </summary>
    public class FileCabinetService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();
        private readonly IRecordValidator validator;
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// </summary>
        /// <param name="validator">The <see cref="IRecordValidator"/> specialised instance.</param>
        public FileCabinetService(IRecordValidator validator)
        {
            this.validator = validator;
        }

        /// <summary>
        /// Creates a new record.
        /// </summary>
        /// <returns>The <see cref="int"/> instance of record's id.</returns>
        public int CreateRecord()
        {
            var record = this.GetInputData();

            this.list.Add(record);

            // Update record id, because the default id = 0
            record.Id = this.list.Count;

            this.AddRecordToSearchDictionaries(record);

            return record.Id;
        }

        /// <summary>
        /// Returns all records.
        /// </summary>
        /// <returns>A read-only instance of all records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.list);
        }

        /// <summary>
        /// Returns the number of records.
        /// </summary>
        /// <returns>The <see cref="int"/> instance of total number of records.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Edits a record.
        /// </summary>
        /// <param name="id">The <see cref="int"/> instance of record's id.</param>
        public void EditRecord(int id)
        {
            var record = this.GetInputData();

            this.RemoveRecordFromSearchDictionaries(id);

            // Update record
            int listId = id - 1;
            this.list[listId].FirstName = record.FirstName;
            this.list[listId].LastName = record.LastName;
            this.list[listId].DateOfBirth = record.DateOfBirth;
            this.list[listId].WorkPlaceNumber = record.WorkPlaceNumber;
            this.list[listId].Salary = record.Salary;
            this.list[listId].Department = record.Department;

            // Assign the correct id to the record, because the default id = 0
            record.Id = id;

            this.AddRecordToSearchDictionaries(record);
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
        /// Creates an instance of <see cref="IFileCabinetServiceSnapshot"/>.
        /// </summary>
        /// <returns>An <see cref="IFileCabinetServiceSnapshot"/> instance.</returns>
        public IFileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceShanpshot(this.list.ToArray());
        }

        /// <summary>
        /// Reads input in a loop until the input data is acquired and validated.
        /// </summary>
        /// <typeparam name="T">The type to be read to.</typeparam>
        /// <param name="converter">Converts a string into a needed type.</param>
        /// <param name="validator">Validates input type with rules.</param>
        /// <returns>The <c>T</c>-type instance of input data.</returns>
        private static T ReadInput<T>(Func<string?, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
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
        /// Adds a record data to the search dictionaries.
        /// </summary>
        /// <param name="record">The <see cref="FileCabinetRecord"/> instance.</param>
        private void AddRecordToSearchDictionaries(in FileCabinetRecord record)
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
        /// <param name="inputId">The <see cref="int"/> id input.</param>
        private void RemoveRecordFromSearchDictionaries(int inputId)
        {
            int listId = inputId - 1;

            // Update firstNameDictionary
            var recordList = this.firstNameDictionary[this.list[listId].FirstName.ToUpperInvariant()];
            recordList.RemoveAll(p => p.Id == inputId);
            if (recordList.Count == 0)
            {
                this.firstNameDictionary.Remove(this.list[listId].FirstName.ToUpperInvariant());
            }

            // Update lastNameDictionary
            recordList = this.lastNameDictionary[this.list[listId].LastName.ToUpperInvariant()];
            recordList.RemoveAll(p => p.Id == inputId);
            if (recordList.Count == 0)
            {
                this.lastNameDictionary.Remove(this.list[listId].LastName.ToUpperInvariant());
            }

            // Update dateOfBirthDictionary
            string dateOfBirthString = this.list[listId].DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
            recordList = this.dateOfBirthDictionary[dateOfBirthString];
            recordList.RemoveAll(p => p.Id == inputId);
            if (recordList.Count == 0)
            {
                this.dateOfBirthDictionary.Remove(dateOfBirthString);
            }
        }

        /// <summary>
        /// Gets input data from console and creates a new record with id = 0.
        /// </summary>
        /// <returns>The <see cref="FileCabinetRecord"/> instance with id = 0.</returns>
        private FileCabinetRecord GetInputData()
        {
            Console.Write("First name: ");
            string firstName = ReadInput(this.StringConverter, this.validator.FirstNameValidator);

            Console.Write("Last name: ");
            string lastName = ReadInput(this.StringConverter, this.validator.LastNameValidator);

            Console.Write("Date of birth: ");
            DateTime dateOfBirth = ReadInput(this.DateConverter, this.validator.DateOfBirthValidator);

            Console.Write("Work Place Number: ");
            short workPlaceNumber = ReadInput(this.ShortConverter, this.validator.WorkPlaceValidator);

            Console.Write("Salary: ");
            decimal salary = ReadInput(this.DecimalConverter, this.validator.SalaryValidator);

            Console.Write("Department (one letter): ");
            char department = ReadInput(this.CharConverter, this.validator.DepartmentValidator);

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

        private Tuple<bool, string, char> CharConverter(string? arg)
        {
            if (!char.TryParse(arg, out char result))
            {
                return Tuple.Create(false, $"{arg} should be a letter.", default(char));
            }

            return Tuple.Create(true, string.Empty, result);
        }

        private Tuple<bool, string, decimal> DecimalConverter(string? arg)
        {
            if (!decimal.TryParse(arg, CultureInfo.InvariantCulture, out decimal result))
            {
                return Tuple.Create(false, $"{arg} is not a valid decimal type.", result);
            }

            return Tuple.Create(true, string.Empty, result);
        }

        private Tuple<bool, string, short> ShortConverter(string? arg)
        {
            if (!short.TryParse(arg, CultureInfo.InvariantCulture, out short result))
            {
                return Tuple.Create(false, $"{arg} is not a valid short number.", result);
            }

            return Tuple.Create(true, string.Empty, result);
        }

        private Tuple<bool, string, DateTime> DateConverter(string? arg)
        {
            if (!DateTime.TryParse(arg, out DateTime dateOfBirth))
            {
                return Tuple.Create(false, $"{arg} is an invalid DateTime.", dateOfBirth);
            }

            return Tuple.Create(true, string.Empty, dateOfBirth);
        }

        private Tuple<bool, string, string> StringConverter(string? arg)
        {
            if (arg == null)
            {
                return Tuple.Create(false, "String cannot be empty.", string.Empty);
            }

            return Tuple.Create(true, string.Empty, arg);
        }
    }
}
