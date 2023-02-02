using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Helper class for storing data in a temporary memory.
    /// </summary>
    public class FileCabinetMemoryService : FileCabinetService, IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">The <see cref="IRecordValidator"/> specialised instance.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
            : base(validator)
        {
            this.list = new List<FileCabinetRecord>();
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

            int listId = id - 1;
            this.RemoveRecordFromSearchDictionaries(this.list[listId]);

            // Update record
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
        /// Creates an instance of <see cref="IFileCabinetServiceSnapshot"/>.
        /// </summary>
        /// <returns>An <see cref="IFileCabinetServiceSnapshot"/> instance.</returns>
        public IFileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceShanpshot(this.list.ToArray());
        }
    }
}
