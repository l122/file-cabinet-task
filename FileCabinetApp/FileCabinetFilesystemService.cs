using System;
using System.Collections.ObjectModel;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Helper class for storing data in a binary file.
    /// </summary>
    internal class FileCabinetFilesystemService : IFileCabinetService, IDisposable
    {
        private const string FileName = "cabinet-records.db";

        private readonly FileStream fileStream;
        private readonly IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="validator">The <see cref="IRecordValidator"/> specialised instance.</param>
        public FileCabinetFilesystemService(IRecordValidator validator)
        {
            this.validator = validator;
            this.fileStream = File.Open(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        /// <summary>
        /// Creates a new record.
        /// </summary>
        /// <returns>The <see cref="int"/> instance of record's id.</returns>
        public int CreateRecord()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns all records.
        /// </summary>
        /// <returns>A read-only instance of all records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the number of records.
        /// </summary>
        /// <returns>The <see cref="int"/> instance of total number of records.</returns>
        public int GetStat()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Edits a record.
        /// </summary>
        /// <param name="id">The <see cref="int"/> instance of record's id.</param>
        public void EditRecord(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches for a record by first name.
        /// </summary>
        /// <param name="firstName">The <see cref="string"/> instance of the first name.</param>
        /// <returns>A read-only instance of all matched records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches for a record by last name.
        /// </summary>
        /// <param name="lastName">The <see cref="string"/> instance of the last name.</param>
        /// <returns>A read-only instance of all matched records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches for a record by date of birth.
        /// </summary>
        /// <param name="dateOfBirthString">The <see cref="string"/> instance of the date of birth.</param>
        /// <returns>A read-only instance of all matched records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an instance of <see cref="IFileCabinetServiceSnapshot"/>.
        /// </summary>
        /// <returns>An <see cref="IFileCabinetServiceSnapshot"/> instance.</returns>
        public IFileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Releases resourses.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resourses.
        /// </summary>
        /// <param name="disposing">The <see cref="bool"/> instance parameter.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.fileStream != null)
            {
                this.fileStream.Dispose();
            }
        }
    }
}
