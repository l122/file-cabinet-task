using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Helper class that contains method implementations.
    /// </summary>
    public abstract class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new ();

        /// <summary>
        /// Gets collection of records.
        /// </summary>
        /// <value>Collection of records.</value>
        protected List<FileCabinetRecord> List { get => this.list; }

        /// <summary>
        /// Creates a new record.
        /// </summary>
        /// <param name="parameters">The <see cref="RecordParameters"/> instance that represents the employee's data.</param>
        /// <returns>The <see cref="int"/> instance of record's id.</returns>
        public int CreateRecord(RecordParameters parameters)
        {
            var record = this.CreateValidator().ValidateParameters(parameters);

            // update record's id, because it's 0 by default
            record.Id = this.list.Count + 1;

            this.list.Add(record);

            this.AddRecordToSearchDictionaries(record);

            return record.Id;
        }

        /// <summary>
        /// Returns all records.
        /// </summary>
        /// <returns>The <see cref="FileCabinetRecord"/> array instance of all records.</returns>
        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
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
        /// <param name="parameters">The <see cref="RecordParameters"/> instance of the input data.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="id"/> is less than 1 or greater than total number of records.
        /// </exception>
        public void EditRecord(int id, RecordParameters parameters)
        {
            if (id < 1 || id > this.list.Count)
            {
                throw new ArgumentException("id is not found.", nameof(id));
            }

            var record = this.CreateValidator().ValidateParameters(parameters);

            this.RemoveRecordFromSearchDictionaries(id);

            // Update record
            int listId = id - 1;
            this.list[listId].FirstName = record.FirstName;
            this.list[listId].LastName = record.LastName;
            this.list[listId].DateOfBirth = record.DateOfBirth;
            this.list[listId].WorkPlaceNumber = record.WorkPlaceNumber;
            this.list[listId].Salary = record.Salary;
            this.list[listId].Department = record.Department;

            // Assign the correct id to the record, because the function 'ValidateParameters' returned a record with id = 0
            record.Id = id;
            this.AddRecordToSearchDictionaries(record);
        }

        /// <summary>
        /// Searches for a record by first name.
        /// </summary>
        /// <param name="firstName">The <see cref="string"/> instance of the first name.</param>
        /// <returns>The <see cref="FileCabinetRecord"/> array instance of all matched records.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.TryGetValue(firstName.ToUpperInvariant(), out List<FileCabinetRecord>? result))
            {
                return result.ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Searches for a record by last name.
        /// </summary>
        /// <param name="lastName">The <see cref="string"/> instance of the last name.</param>
        /// <returns>The <see cref="FileCabinetRecord"/> array instance of all matched records.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.TryGetValue(lastName.ToUpperInvariant(), out List<FileCabinetRecord>? result))
            {
                return result.ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Searches for a record by date of birth.
        /// </summary>
        /// <param name="dateOfBirthString">The <see cref="string"/> instance of the date of birth.</param>
        /// <returns>The <see cref="FileCabinetRecord"/> array instance of all matched records.</returns>
        public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirthString)
        {
            if (DateTime.TryParse(dateOfBirthString, out DateTime dateOfBirth))
            {
                dateOfBirthString = dateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
            }

            if (this.dateOfBirthDictionary.TryGetValue(dateOfBirthString, out List<FileCabinetRecord>? result))
            {
                return result.ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Delegates the creation of validator to inheriting classes.
        /// </summary>
        /// <returns>The <see cref="IRecordValidator"/> concrete implementation instance.</returns>
        public abstract IRecordValidator CreateValidator();

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
        /// Used to pass parameters between methods.
        /// </summary>
        public class RecordParameters
        {
            /// <summary>
            /// Gets or sets the parameter 'First Name'.
            /// </summary>
            /// <value>First Name.</value>
            public string? FirstName { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the parameter 'Last Name'.
            /// </summary>
            /// <value>Last Name.</value>
            public string? LastName { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the parameter 'Date of birth'.
            /// </summary>
            /// <value>Date of birth.</value>
            public string? DateOfBirth { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the parameter 'Work Place Number'.
            /// </summary>
            /// <value>Work Place Number string.</value>
            public string? WorkPlaceNumber { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the parameter 'Salary'.
            /// </summary>
            /// <value>Salary string.</value>
            public string? Salary { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the parameter 'Department'.
            /// </summary>
            /// <value>Department letter.</value>
            public string? Department { get; set; } = string.Empty;
        }
    }
}
