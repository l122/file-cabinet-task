using System;
using System.Collections.ObjectModel;

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
        /// <returns>A read-only instance of all records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Returns a pair of (total, deleted) number of records.
        /// </summary>
        /// <returns>The <see cref="Tuple"/> instance of total and deleted number of records.</returns>
        public (int, int) GetStat();

        /// <summary>
        /// Edits a record.
        /// </summary>
        /// <param name="record">The <see cref="FileCabinetRecord"/> instance of the new record.</param>
        /// <returns>true if success, false - otherwise.</returns>
        public bool EditRecord(FileCabinetRecord record);

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
        /// Creates an instance of <see cref="IFileCabinetServiceSnapshot"/>.
        /// </summary>
        /// <returns>An <see cref="IFileCabinetServiceSnapshot"/> instance.</returns>
        public IFileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Restores a <see cref="FileCabinetServiceSnapshot"/> instance.
        /// </summary>
        /// <param name="snapshot">A <see cref="IFileCabinetServiceSnapshot"/> specialized instance.</param>
        public void Restore(IFileCabinetServiceSnapshot snapshot);

        /// <summary>
        /// Removes a record.
        /// </summary>
        /// <param name="id">A <see cref="int"/> instance of id.</param>
        /// <returns>true if success, false otherwise.</returns>
        public bool RemoveRecord(int id);

        /// <summary>
        /// Removes the records that are marked as deleted from a database.
        /// </summary>
        /// <returns>The <see cref="Tuple"/> instance of total and purged number of records.</returns>
        public (int, int) Purge();

        /// <summary>
        /// Finds a record by id.
        /// </summary>
        /// <param name="id">An <see cref="int"/> instance.</param>
        /// <returns>A nullable <see cref="FileCabinetRecord"/> instance if found, null - otherwise.</returns>
        public FileCabinetRecord? FindById(int id);
    }
}
