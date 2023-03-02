using System;
using System.Collections.Generic;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// Helper class that contains method implementations.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Create a record.
        /// </summary>
        /// <param name="record">A <see cref="FileCabinetRecord"/> instance.</param>
        /// <returns>An <see cref="int"/> instance.</returns>
        public int CreateRecord(FileCabinetRecord record);

        /// <summary>
        /// Returns all records.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> specialized instance.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Returns a pair of (total, deleted) number of records.
        /// </summary>
        /// <returns>The <see cref="Tuple"/> instance of total and deleted number of records.</returns>
        public (int, int) GetStat();

        /// <summary>
        /// Searches for a record by first name.
        /// </summary>
        /// <param name="firstName">The <see cref="string"/> instance of the first name.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> specialized instance.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Searches for a record by last name.
        /// </summary>
        /// <param name="lastName">The <see cref="string"/> instance of the last name.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> specialized instance.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Searches for a record by date of birth.
        /// </summary>
        /// <param name="dateOfBirthString">The <see cref="string"/> instance of the date of birth.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> specialized instance.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString);

        /// <summary>
        /// Creates an instance of <see cref="IFileCabinetServiceSnapshot"/>.
        /// </summary>
        /// <returns>An <see cref="IFileCabinetServiceSnapshot"/> instance.</returns>
        public IFileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Restores a <see cref="FileCabinetServiceSnapshot"/> instance.
        /// </summary>
        /// <param name="snapshot">A <see cref="IFileCabinetServiceSnapshot"/> specialized instance with data to be restored from.</param>
        /// <returns>an <see cref="int"/> of restored number of records.</returns>
        public int Restore(IFileCabinetServiceSnapshot snapshot);

        /// <summary>
        /// Removes the records that are marked as deleted from a database.
        /// </summary>
        /// <returns>The <see cref="Tuple"/> instance of total and purged number of records.</returns>
        public (int, int) Purge();

        /// <summary>
        /// Finds a record by id.
        /// </summary>
        /// <param name="id">An <see cref="int"/> instance.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> specialized instance of found record.</returns>
        public IEnumerable<FileCabinetRecord> FindById(int id);

        /// <summary>
        /// Inserts a record.
        /// </summary>
        /// <param name="record">A <see cref="FileCabinetRecord"/> instance.</param>
        /// <returns>true if record is inserted; false otherwise.</returns>
        public bool Insert(FileCabinetRecord record);

        /// <summary>
        /// Deletes the record(s) that satisfy the expression.
        /// </summary>
        /// <param name="expression">A <see cref="string"/> instance of input expression.</param>
        /// <returns>A <see cref="string"/> confirmation of deleted records.</returns>
        public string Delete(string expression);

        /// <summary>
        /// Updates the record(s) that satisfy the expression.
        /// </summary>
        /// <param name="expression">A <see cref="string"/> instance of input fields and expression.</param>
        /// <returns>A <see cref="string"/> convfirmation of updated records.</returns>
        public string Update(string expression);
    }
}
