using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FileCabinetApp.StaticClasses;
using FileCabinetApp.Validators;
using Newtonsoft.Json.Linq;

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
            this.list = new ();
            this.firstNameDictionary = new ();
            this.lastNameDictionary = new ();
            this.dateOfBirthDictionary = new ();
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
        public IEnumerable<FileCabinetRecord> SelectRecords(string expression)
        {
            return Memoizer.Memoize(this.SelectRecords1)(expression);
        }

        /// <inheritdoc/>
        public (int, int) GetStat()
        {
            return (this.list.Count, 0);
        }

        /// <inheritdoc/>
        public bool Insert(FileCabinetRecord record)
        {
            int listId = this.GetListId(record.Id);
            if (listId != -1)
            {
                Console.WriteLine("Record #{0} already exists. To update the record use command 'update'.", record.Id);
                return false;
            }

            // validate record
            var validationResult = this.validator.ValidateParameters(record);
            if (!validationResult.Item1)
            {
                Console.WriteLine("Validation failed: {0}", validationResult.Item2);
                return false;
            }

            this.list.Add(record);
            this.list.Sort((x, y) => x.Id.CompareTo(y.Id));

            this.AddRecordToSearchDictionaries(record);

            Memoizer.MemoizeReset();

            return true;
        }

        /// <inheritdoc/>
        public IFileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(new MemoryEnumerable(this.list));
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindById(int id)
        {
            var listId = this.GetListId(id);
            if (listId == -1)
            {
                return new MemoryEnumerable();
            }

            return new MemoryEnumerable(new List<FileCabinetRecord>() { this.list[listId] });
        }

        /// <inheritdoc/>
        public int Restore(IFileCabinetServiceSnapshot snapshot)
        {
            var records = snapshot.Records.Where(p => this.IsValidRecord(p)).ToArray();

            foreach (var record in records)
            {
                var listId = this.list.FindIndex(p => p.Id == record.Id);
                if (listId != -1)
                {
                    this.RemoveRecordFromSearchDictionaries(record);
                    this.list[listId] = record;
                }
                else
                {
                    this.list.Add(record);
                }

                this.AddRecordToSearchDictionaries(record);
            }

            // Sort the list.
            this.list.Sort((x, y) => x.Id.CompareTo(y.Id));

            return records.Length;
        }

        /// <inheritdoc/>
        public (int, int) Purge()
        {
            return (0, this.list.Count);
        }

        /// <inheritdoc/>
        public string Delete(string expression)
        {
            const string errorMessage = "Invalid parameters. Call 'help delete' for help.";

            if (string.IsNullOrEmpty(expression))
            {
                return errorMessage;
            }

            IEnumerable<FileCabinetRecord> recordsForDeletion;
            try
            {
                recordsForDeletion = Parser.ParseWhereExpression(new MemoryEnumerable(this.list), expression);
            }
            catch (ArgumentException)
            {
                return errorMessage;
            }

            StringBuilder returnMessage = new ();
            List<int> deletedIds = new ();
            foreach (var record in recordsForDeletion)
            {
                if (deletedIds.Count == 0)
                {
                    returnMessage.Append('#');
                }
                else
                {
                    returnMessage.Append(", #");
                }

                returnMessage.Append(record.Id);
                deletedIds.Add(record.Id);
                this.RemoveRecordFromSearchDictionaries(record);
            }

            this.list.RemoveAll(p => deletedIds.Contains(p.Id));

            if (returnMessage.Length == 0)
            {
                return "No record is deleted." + Environment.NewLine;
            }

            if (deletedIds.Count == 1)
            {
                returnMessage.Insert(0, "Record ");
                returnMessage.Append(" is deleted.");
            }
            else
            {
                returnMessage.Insert(0, "Records ");
                returnMessage.Append(" are deleted.");
            }

            returnMessage.Append(Environment.NewLine);

            Memoizer.MemoizeReset();

            return returnMessage.ToString();
        }

        /// <inheritdoc/>
        public string Update(string expression)
        {
            const string setStr = "set ";
            const string errorMessage = "Invalid parameters. Call 'help update' for help.";
            const string norecordUpdateMessage = "No record is updated.";

            if (string.IsNullOrEmpty(expression))
            {
                return errorMessage;
            }

            var whereIndex = expression.IndexOf("where ", StringComparison.InvariantCultureIgnoreCase);
            if (!expression.StartsWith(setStr, StringComparison.InvariantCultureIgnoreCase)
                || whereIndex == -1)
            {
                return errorMessage;
            }

            IEnumerable<FileCabinetRecord> recordsForUpdate;
            Dictionary<string, string> fieldsToUpdate;
            try
            {
                fieldsToUpdate = Parser.ParseFieldsAndValues(expression[setStr.Length..whereIndex]);
                recordsForUpdate = Parser.ParseWhereExpression(new MemoryEnumerable(this.list), expression[whereIndex..]);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return errorMessage;
            }

            StringBuilder returnMessage = new ();
            List<int> updatedIds = new ();
            foreach (var record in recordsForUpdate.ToArray())
            {
                if (updatedIds.Count == 0)
                {
                    returnMessage.Append('#');
                }
                else
                {
                    returnMessage.Append(", #");
                }

                var newRecord = Parser.GetUpdatedRecord(record, fieldsToUpdate, this.validator);
                if (newRecord == null)
                {
                    return norecordUpdateMessage;
                }

                this.RemoveRecordFromSearchDictionaries(record);
                var i = this.list.FindIndex(p => p.Id.Equals(record.Id));
                this.list[i] = newRecord;
                this.AddRecordToSearchDictionaries(newRecord);
                returnMessage.Append(record.Id);
                updatedIds.Add(record.Id);
            }

            if (updatedIds.Count == 0)
            {
                return norecordUpdateMessage;
            }

            if (updatedIds.Count == 1)
            {
                returnMessage.Insert(0, "Record ");
                returnMessage.Append(" is updated.");
            }
            else
            {
                returnMessage.Insert(0, "Records ");
                returnMessage.Append(" are updated.");
            }

            returnMessage.Append(Environment.NewLine);

            Memoizer.MemoizeReset();

            return returnMessage.ToString();
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

        private IEnumerable<FileCabinetRecord> SelectRecords1(string expression)
        {
            const string errorMessage = "Invalid parameters. Call 'help select' for help.";

            if (string.IsNullOrEmpty(expression))
            {
                return new MemoryEnumerable(this.list);
            }

            IEnumerable<FileCabinetRecord> result;
            var whereIndex = expression.IndexOf("where ", StringComparison.InvariantCultureIgnoreCase);
            try
            {
                if (whereIndex != -1)
                {
                    result = Parser.ParseWhereExpression(new MemoryEnumerable(this.list), expression[whereIndex..]);
                }
                else
                {
                    result = new MemoryEnumerable(this.list);
                }
            }
            catch (ArgumentException)
            {
                Console.WriteLine(errorMessage);
                return new MemoryEnumerable(new List<FileCabinetRecord>());
            }

            return result;
        }
    }
}
