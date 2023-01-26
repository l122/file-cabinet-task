﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Helper class that contains method implementations.
    /// </summary>
    public class FileCabinetService
    {
        private static readonly DateTime MinDate = new (1950, 1, 1);
        private readonly List<FileCabinetRecord> list = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new ();

        /// <summary>
        /// Creates a new record.
        /// </summary>
        /// <param name="inputParameters">The <see cref="RecordParameters"/> instance that represents the employee's data.</param>
        /// <returns>The <see cref="int"/> instance of record's id.</returns>
        public int CreateRecord(RecordParameters inputParameters)
        {
            var record = this.GetValidRecord(inputParameters);

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
        /// <param name="inputParameters">The <see cref="RecordParameters"/> instance of the input data.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="id"/> is less than 1 or greater than total number of records.
        /// </exception>
        public void EditRecord(int id, RecordParameters inputParameters)
        {
            if (id < 1 || id > this.list.Count)
            {
                throw new ArgumentException("id is not found.", nameof(id));
            }

            var newRecord = this.GetValidRecord(inputParameters);

            this.RemoveRecordFromSearchDictionaries(id);

            // Update record
            int listId = id - 1;
            this.list[listId].FirstName = newRecord.FirstName;
            this.list[listId].LastName = newRecord.LastName;
            this.list[listId].DateOfBirth = newRecord.DateOfBirth;
            this.list[listId].WorkPlaceNumber = newRecord.WorkPlaceNumber;
            this.list[listId].Salary = newRecord.Salary;
            this.list[listId].Department = newRecord.Department;

            // Assign the correct id to the record, because the function 'GetValidRecord' returned a record with id = list.count
            newRecord.Id = id;
            this.AddRecordToSearchDictionaries(newRecord);
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

        private FileCabinetRecord GetValidRecord(RecordParameters data)
        {
            if (string.IsNullOrWhiteSpace(data.FirstName))
            {
                throw new ArgumentException("First name cannot be empty or have only white spaces.", nameof(data));
            }

            data.FirstName = data.FirstName.Trim();
            if (data.FirstName.Length < 2 || data.FirstName.Length > 60)
            {
                throw new ArgumentException("First name has to have at least 2 and maximum 60 characters.", nameof(data));
            }

            if (string.IsNullOrWhiteSpace(data.LastName))
            {
                throw new ArgumentException("Last name cannot be empty or have only white spaces.", nameof(data));
            }

            data.LastName = data.LastName.Trim();
            if (data.LastName.Length < 2 || data.LastName.Length > 60)
            {
                throw new ArgumentException("Last name has to have at least 2 and maximum 60 characters.", nameof(data));
            }

            if (data.DateOfBirth == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!DateTime.TryParse(data.DateOfBirth, out DateTime dateOfBirth))
            {
                throw new ArgumentException("Date of birth is invalid.", nameof(data));
            }

            if (DateTime.Compare(dateOfBirth, MinDate) < 0
                || DateTime.Compare(dateOfBirth, DateTime.Today) > 0)
            {
                throw new ArgumentException("Date of birth should be within 01-Jan-1950 and today.", nameof(data));
            }

            if (data.WorkPlaceNumber == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!short.TryParse(data.WorkPlaceNumber, CultureInfo.InvariantCulture, out short workPlaceNumber))
            {
                throw new ArgumentException("WorkPlaceNumber is not a valid number.", nameof(data));
            }

            if (workPlaceNumber < 0 || DateTime.Compare(dateOfBirth.AddYears(workPlaceNumber), DateTime.Today) > 0)
            {
                throw new ArgumentException("WorkPlaceNumber cannot be less than zero or be passed today.", nameof(data));
            }

            if (data.Salary == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!decimal.TryParse(data.Salary, CultureInfo.InvariantCulture, out decimal salary))
            {
                throw new ArgumentException("salary is not a valid number.", nameof(data));
            }

            if (salary < 0)
            {
                throw new ArgumentException("Saving cannot be less than zero.", nameof(data));
            }

            if (data.Department == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Department.Length != 1)
            {
                throw new ArgumentException("Department should have one character", nameof(data));
            }

            char department = data.Department[0];
            if (!char.IsLetter(department))
            {
                throw new ArgumentException("Not an English letter.", nameof(data));
            }

            return new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = data.FirstName,
                LastName = data.LastName,
                DateOfBirth = dateOfBirth,
                WorkPlaceNumber = workPlaceNumber,
                Salary = salary,
                Department = department,
            };
        }

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
