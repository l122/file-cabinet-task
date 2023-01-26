using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Contains the Main method.
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
        /// <param name="firstName">The <see cref="string"/> instance that represents the employee's first name.</param>
        /// <param name="lastName">The <see cref="string"/> instance that represents the employee's last name.</param>
        /// <param name="dateOfBirth">The <see cref="string"/> instance that represents the employee's date of birth.</param>
        /// <param name="workPlaceNumber">The <see cref="string"/> instance that represents the employee's work place number.</param>
        /// <param name="salary">The <see cref="string"/> instance that represents the employee's salary.</param>
        /// <param name="department">The <see cref="string"/> instance that represents the employee's department. Should be one letter.</param>
        /// <returns>The <see cref="int"/> instance of record's id.</returns>
        public int CreateRecord(
            string? firstName,
            string? lastName,
            string? dateOfBirth,
            string? workPlaceNumber,
            string? salary,
            string? department)
        {
            var record = this.GetValidRecord(firstName, lastName, dateOfBirth, workPlaceNumber, salary, department);

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
        /// <param name="firstName">The nullable <see cref="string"/> instance of the first name.</param>
        /// <param name="lastName">The nullable <see cref="string"/> instance of the last name.</param>
        /// <param name="dateOfBirth">The nullable <see cref="string"/> instance of the date of birth.</param>
        /// <param name="workPlaceNumber">The nullable <see cref="string"/> instance of the place number.</param>
        /// <param name="salary">The nullable <see cref="string"/> instance of the salary.</param>
        /// <param name="department">The nullable <see cref="string"/> instance of the department letter.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="id"/> is less than 1 or greater than total number of records.
        /// </exception>
        public void EditRecord(
            int id,
            string? firstName,
            string? lastName,
            string? dateOfBirth,
            string? workPlaceNumber,
            string? salary,
            string? department)
        {
            if (id < 1 || id > this.list.Count)
            {
                throw new ArgumentException("id is not found.", nameof(id));
            }

            var record = this.GetValidRecord(firstName, lastName, dateOfBirth, workPlaceNumber, salary, department);

            this.RemoveRecordFromSearchDictionaries(id);

            // Update record
            int listId = id - 1;
            this.list[listId].FirstName = record.FirstName;
            this.list[listId].LastName = record.LastName;
            this.list[listId].DateOfBirth = record.DateOfBirth;
            this.list[listId].WorkPlaceNumber = record.WorkPlaceNumber;
            this.list[listId].Salary = record.Salary;
            this.list[listId].Department = record.Department;

            // Assign the correct id to the record, because the function 'GetValidRecord' returned a record with id = list.count
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

        private FileCabinetRecord GetValidRecord(
            string? firstName,
            string? lastName,
            string? dateOfBirthString,
            string? workPlaceNumberString,
            string? salaryString,
            string? departmentString)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("First name cannot be empty or have only white spaces.", nameof(firstName));
            }

            firstName = firstName.Trim();
            if (firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException("First name has to have at least 2 and maximum 60 characters.", nameof(firstName));
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("Last name cannot be empty or have only white spaces.", nameof(lastName));
            }

            lastName = lastName.Trim();
            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException("Last name has to have at least 2 and maximum 60 characters.", nameof(lastName));
            }

            if (dateOfBirthString == null)
            {
                throw new ArgumentNullException(nameof(dateOfBirthString));
            }

            if (!DateTime.TryParse(dateOfBirthString, out DateTime dateOfBirth))
            {
                throw new ArgumentException("Date of birth is invalid.", nameof(dateOfBirthString));
            }

            if (DateTime.Compare(dateOfBirth, MinDate) < 0
                || DateTime.Compare(dateOfBirth, DateTime.Today) > 0)
            {
                throw new ArgumentException("Date of birth should be within 01-Jan-1950 and today.", nameof(dateOfBirthString));
            }

            if (workPlaceNumberString == null)
            {
                throw new ArgumentNullException(nameof(workPlaceNumberString));
            }

            if (!short.TryParse(workPlaceNumberString, CultureInfo.InvariantCulture, out short workPlaceNumber))
            {
                throw new ArgumentException("WorkPlaceNumber is not a valid number.", nameof(workPlaceNumberString));
            }

            if (workPlaceNumber < 0 || DateTime.Compare(dateOfBirth.AddYears(workPlaceNumber), DateTime.Today) > 0)
            {
                throw new ArgumentException("WorkPlaceNumber cannot be less than zero or be passed today.", nameof(workPlaceNumberString));
            }

            if (salaryString == null)
            {
                throw new ArgumentNullException(nameof(salaryString));
            }

            if (!decimal.TryParse(salaryString, CultureInfo.InvariantCulture, out decimal salary))
            {
                throw new ArgumentException("salary is not a valid number.", nameof(salaryString));
            }

            if (salary < 0)
            {
                throw new ArgumentException("Saving cannot be less than zero.", nameof(salaryString));
            }

            if (departmentString == null)
            {
                throw new ArgumentNullException(nameof(departmentString));
            }

            if (departmentString.Length != 1)
            {
                throw new ArgumentException("Department should have one character", nameof(departmentString));
            }

            char department = departmentString[0];
            if (!char.IsLetter(department))
            {
                throw new ArgumentException("Not an English letter.", nameof(departmentString));
            }

            return new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
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
    }
}
