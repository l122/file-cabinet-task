using System;
using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    /// Helper class that contains method implementations.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Creates a new record.
        /// </summary>
        /// <param name="parameters">The <see cref="RecordParameters"/> instance that represents the employee's data.</param>
        /// <returns>The <see cref="int"/> instance of record's id.</returns>
        public int CreateRecord(RecordParameters parameters);

        /// <summary>
        /// Returns all records.
        /// </summary>
        /// <returns>A read-only instance of all records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Returns the number of records.
        /// </summary>
        /// <returns>The <see cref="int"/> instance of total number of records.</returns>
        public int GetStat();

        /// <summary>
        /// Edits a record.
        /// </summary>
        /// <param name="id">The <see cref="int"/> instance of record's id.</param>
        /// <param name="parameters">The <see cref="RecordParameters"/> instance of the input data.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="id"/> is less than 1 or greater than total number of records.
        /// </exception>
        public void EditRecord(int id, RecordParameters parameters);

        /// <summary>
        /// Searches for a record by first name.
        /// </summary>
        /// <param name="firstName">The <see cref="string"/> instance of the first name.</param>
        /// <returns>A read-only instance of all matched records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Searches for a record by last name.
        /// </summary>
        /// <param name="lastName">The <see cref="string"/> instance of the last name.</param>
        /// <returns>A read-only instance of all matched records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Searches for a record by date of birth.
        /// </summary>
        /// <param name="dateOfBirthString">The <see cref="string"/> instance of the date of birth.</param>
        /// <returns>A read-only instance of all matched records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString);

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
