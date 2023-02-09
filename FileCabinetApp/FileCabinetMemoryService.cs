using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// Helper class for storing data in a temporary memory.
    /// </summary>
    public class FileCabinetMemoryService : FileCabinetService, IFileCabinetService
    {
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary;
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary;
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary;
        private readonly List<FileCabinetRecord> list;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">The <see cref="IRecordValidator"/> specialised instance.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
            : base(validator)
        {
            this.list = new List<FileCabinetRecord>();
            this.firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
            this.lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
            this.dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        }

        /// <inheritdoc/>
        public int CreateRecord()
        {
            var record = this.GetInputData();

            // Update record id, because the default id = 1
            if (this.list.Count > 0)
            {
                record.Id = this.list.Last().Id + 1;
            }

            this.list.Add(record);
            this.AddRecordToSearchDictionaries(record);

            return record.Id;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.list);
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <inheritdoc/>
        public void EditRecord(int id)
        {
            int listId = this.list.FindIndex(p => p.Id == id);
            if (listId == -1)
            {
                Console.WriteLine("#{0} record is not found.", id);
                return;
            }

            var record = this.GetInputData();

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
            Console.WriteLine("Record #{0} is updated.", id);
        }

        /// <inheritdoc/>
        public IFileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list.ToArray());
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.TryGetValue(firstName.ToUpperInvariant(), out List<FileCabinetRecord>? result))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(result);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.TryGetValue(lastName.ToUpperInvariant(), out List<FileCabinetRecord>? result))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(result);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void Restore(IFileCabinetServiceSnapshot snapshot)
        {
            var records = snapshot.Records;

            foreach (var record in records)
            {
                if (!this.IsValidRecord(record))
                {
                    continue;
                }

                this.AddRecordToSearchDictionaries(record);

                var listId = this.list.FindIndex(p => p.Id == record.Id);
                if (listId != -1)
                {
                    this.list[listId] = record;
                }
                else
                {
                    this.list.Add(record);
                }
            }
        }

        /// <inheritdoc/>
        public void RemoveRecord(int id)
        {
            var listId = this.list.FindIndex(p => p.Id == id);

            if (listId == -1)
            {
                Console.WriteLine("Record #{0} doesn't exit.", id);
                return;
            }

            this.list.RemoveAt(listId);
            this.RemoveRecordFromSearchDictionaries(this.list[listId]);

            Console.WriteLine("Record #{0} is removed.", id);
        }

        /// <summary>
        /// Does nothing for FileCabinetMemorySerive.
        /// </summary>
        public void Purge()
        {
            // Do nothing because it's not applied to the memory service.
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
        /// <param name="record">The <see cref="FileCabinetRecord"/> id input.</param>
        private void RemoveRecordFromSearchDictionaries(FileCabinetRecord record)
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
