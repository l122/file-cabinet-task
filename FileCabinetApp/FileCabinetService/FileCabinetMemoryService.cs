using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FileCabinetApp.Validators;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// Helper class for storing data in a temporary memory.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private const string DateMask = "yyyy-MMM-dd";
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary;
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary;
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary;
        private readonly IRecordValidator validator;
        private readonly List<FileCabinetRecord> list;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">The <see cref="IRecordValidator"/> specialised instance.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
        {
            this.validator = validator;
            this.list = new List<FileCabinetRecord>();
            this.firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
            this.lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
            this.dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        }

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetRecord record)
        {
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
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            return new MemoryEnumerable(this.list);
        }

        /// <inheritdoc/>
        public (int, int) GetStat()
        {
            return (this.list.Count, 0);
        }

        /// <inheritdoc/>
        public bool EditRecord(FileCabinetRecord record)
        {
            int listId = this.GetListId(record.Id);
            if (listId == -1)
            {
                return false;
            }

            this.RemoveRecordFromSearchDictionaries(this.list[listId]);

            // Update record
            this.list[listId].FirstName = record.FirstName;
            this.list[listId].LastName = record.LastName;
            this.list[listId].DateOfBirth = record.DateOfBirth;
            this.list[listId].WorkPlaceNumber = record.WorkPlaceNumber;
            this.list[listId].Salary = record.Salary;
            this.list[listId].Department = record.Department;

            this.AddRecordToSearchDictionaries(record);

            return true;
        }

        /// <inheritdoc/>
        public IFileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(new MemoryIterator(this.list));
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.TryGetValue(firstName.ToUpperInvariant(), out List<FileCabinetRecord>? result))
            {
                return new MemoryEnumerable(result);
            }

            return new MemoryEnumerable();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.TryGetValue(lastName.ToUpperInvariant(), out List<FileCabinetRecord>? result))
            {
                return new MemoryEnumerable(result);
            }

            return new MemoryEnumerable();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString)
        {
            if (DateTime.TryParse(dateOfBirthString, out DateTime dateOfBirth))
            {
                dateOfBirthString = dateOfBirth.ToString(DateMask, CultureInfo.InvariantCulture);
            }

            if (this.dateOfBirthDictionary.TryGetValue(dateOfBirthString, out List<FileCabinetRecord>? result))
            {
                return new MemoryEnumerable(result);
            }

            return new MemoryEnumerable();
        }

        /// <inheritdoc/>
        public int Restore(IFileCabinetServiceSnapshot snapshot)
        {
            var records = snapshot.Records.Where(p => this.IsValidRecord(p)).ToArray();

            foreach (var record in records)
            {
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

            return records.Length;
        }

        /// <inheritdoc/>
        public bool RemoveRecord(int id)
        {
            var listId = this.list.FindIndex(p => p.Id == id);

            if (listId == -1)
            {
                return false;
            }

            this.RemoveRecordFromSearchDictionaries(this.list[listId]);
            this.list.RemoveAt(listId);

            return true;
        }

        /// <inheritdoc/>
        public (int, int) Purge()
        {
            return (0, this.list.Count);
        }

        /// <inheritdoc/>
        public FileCabinetRecord? FindById(int id)
        {
            var listId = this.GetListId(id);
            if (listId == -1)
            {
                return null;
            }

            return this.list[listId];
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
            string dateOfBirthString = record.DateOfBirth.ToString(DateMask, CultureInfo.InvariantCulture);
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
            var valueList = this.firstNameDictionary[record.FirstName.ToUpperInvariant()];
            valueList.RemoveAll(p => p.Id == record.Id);
            if (valueList.Count == 0)
            {
                this.firstNameDictionary.Remove(record.FirstName.ToUpperInvariant());
            }

            // Update lastNameDictionary
            valueList = this.lastNameDictionary[record.LastName.ToUpperInvariant()];
            valueList.RemoveAll(p => p.Id == record.Id);
            if (valueList.Count == 0)
            {
                this.lastNameDictionary.Remove(record.LastName.ToUpperInvariant());
            }

            // Update dateOfBirthDictionary
            string dateOfBirthString = record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
            valueList = this.dateOfBirthDictionary[dateOfBirthString];
            valueList.RemoveAll(p => p.Id == record.Id);
            if (valueList.Count == 0)
            {
                this.dateOfBirthDictionary.Remove(dateOfBirthString);
            }
        }

        /// <summary>
        /// Validates the <see cref="FileCabinetRecord"/> instance.
        /// </summary>
        /// <param name="record">A <see cref="FileCabinetRecord"/> instance.</param>
        /// <returns>true if record is valid, false otherwise.</returns>
        private bool IsValidRecord(FileCabinetRecord record)
        {
            // Validate Record
            var validationResult = this.validator.ValidateParameters(record);
            if (!validationResult.Item1)
            {
                Console.WriteLine("#{0}: {1}", record.Id, validationResult.Item2);
                return false;
            }

            return true;
        }

        private int GetListId(int id)
        {
            return this.list.FindIndex(p => p.Id == id);
        }
    }
}
