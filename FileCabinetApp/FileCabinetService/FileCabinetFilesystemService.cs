using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// Helper class for storing data in a binary file.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService, IDisposable
    {
        private const string FileName = "cabinet-records.db";
        private const string DateMask = "yyyy-MMM-dd";
        private const int RecordSize = 278;
        private const int StringBufferSize = 120;
        private readonly IRecordValidator validator;
        private readonly Dictionary<string, List<long>> firstNameDictionary;
        private readonly Dictionary<string, List<long>> lastNameDictionary;
        private readonly Dictionary<string, List<long>> dateOfBirthDictionary;
        private FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="validator">The <see cref="IRecordValidator"/> specialised instance.</param>
        public FileCabinetFilesystemService(IRecordValidator validator)
        {
            this.validator = validator;
            this.fileStream = File.Open(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            this.firstNameDictionary = new Dictionary<string, List<long>>();
            this.lastNameDictionary = new Dictionary<string, List<long>>();
            this.dateOfBirthDictionary = new Dictionary<string, List<long>>();

            this.UpdateSearchDictionaries();
        }

        private enum Status : short
        {
            NotDeleted,
            Deleted,
        }

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetRecord record)
        {
            // Assign id
            record.Id = this.GetLastId() + 1;

            if (this.WriteToFile(record, this.fileStream.Length))
            {
                var pos = this.GetPosition(record.Id);
                this.AddRecordToSearchDictionaries(record, pos);
                return record.Id;
            }

            return -1;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> records = new ();
            long position = 0;

            while (position < this.fileStream.Length)
            {
                var record = this.ReadRecord(position);
                if (record != null)
                {
                    records.Add(record);
                }

                position += RecordSize;
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public (int, int) GetStat()
        {
            byte[] buffer = new byte[2];
            int counter = 0;

            // Loop over file and count the records with the status "NotDelete"
            for (long i = 0; i < this.fileStream.Length; i += RecordSize)
            {
                this.fileStream.Position = i;
                try
                {
                    this.fileStream.Read(buffer, 0, buffer.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in reading data in {0} : {1}", FileName, e.ToString());
                }

                var status = BitConverter.ToInt16(buffer, 0);
                if (status == (short)Status.NotDeleted)
                {
                    counter++;
                }
            }

            var deletedQuantity = (int)(this.fileStream.Length / RecordSize) - counter;

            return (counter, deletedQuantity);
        }

        /// <inheritdoc/>
        public bool EditRecord(FileCabinetRecord record)
        {
            if (record == null)
            {
                return false;
            }

            var pos = this.GetPosition(record.Id);
            if (pos == -1)
            {
                return false;
            }

            var oldRecord = this.ReadRecord(pos);
            if (oldRecord != null)
            {
                this.RemoveRecordFromSearchDictionaries(oldRecord, pos);
            }

            var result = this.WriteToFile(record, pos);
            if (result)
            {
                this.AddRecordToSearchDictionaries(record, pos);
            }

            return result;
        }

        /// <inheritdoc/>
        public IFileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.GetRecords().ToArray());
        }

        /// <summary>
        /// Releases resourses.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.TryGetValue(firstName.ToUpperInvariant(), out var list))
            {
                return this.ReadRecords(list);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.TryGetValue(lastName.ToUpperInvariant(), out var list))
            {
                return this.ReadRecords(list);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString)
        {
            if (DateTime.TryParse(dateOfBirthString, CultureInfo.InvariantCulture, out var value))
            {
                string date = value.ToString(DateMask, CultureInfo.InvariantCulture);
                if (this.dateOfBirthDictionary.TryGetValue(date, out var list))
                {
                    return this.ReadRecords(list);
                }
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

                var position = this.GetPosition(record.Id);
                bool recordWriten;
                if (position != -1)
                {
                    recordWriten = this.WriteToFile(record, position);
                }
                else
                {
                    position = this.fileStream.Length;
                    recordWriten = this.WriteToFile(record, position);
                }

                if (recordWriten)
                {
                    this.AddRecordToSearchDictionaries(record, position);
                }
            }
        }

        /// <inheritdoc/>
        public bool RemoveRecord(int id)
        {
            // find id
            var position = this.GetPosition(id);
            if (position == -1)
            {
                return false;
            }

            this.fileStream.Position = position;
            try
            {
                this.fileStream.Write(BitConverter.GetBytes((short)Status.Deleted), 0, sizeof(short));
                this.fileStream.Flush();

                var record = this.ReadRecord(position);
                if (record != null)
                {
                    this.RemoveRecordFromSearchDictionaries(record, position);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error deleting a record: {0}", e.ToString());
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public (int, int) Purge()
        {
            int oldQuantity = (int)(this.fileStream.Length / RecordSize);

            var records = this.GetRecords();
            try
            {
                if (this.fileStream != null)
                {
                    this.fileStream.Dispose();
                }

                this.fileStream = File.Open(FileName, FileMode.Create, FileAccess.ReadWrite);

                var position = 0;
                foreach (var record in records)
                {
                    if (this.WriteToFile(record, position))
                    {
                        position += RecordSize;
                    }
                    else
                    {
                        Console.WriteLine("Error writing a record id#{0} to the database at the file position {1}", record.Id, position);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while clearing file {0} : {1}", FileName, e.ToString());
                return (-1, -1);
            }

            this.UpdateSearchDictionaries();

            int newQuantity = (int)(this.fileStream.Length / RecordSize);
            int purgedQuantity = oldQuantity - newQuantity;
            return (purgedQuantity, oldQuantity);
        }

        /// <inheritdoc/>
        public FileCabinetRecord? FindById(int id)
        {
            var pos = this.GetPosition(id);

            if (pos >= 0)
            {
                return this.ReadRecord(pos);
            }

            return null;
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

        /// <summary>
        /// Copies byte[] into a byte[] at specified position.
        /// </summary>
        /// <param name="data">A <see cref="byte"/> source array.</param>
        /// <param name="buffer">A <see cref="byte"/> destination array.</param>
        /// <param name="offset">A <see cref="int"/> start index of the destination array.</param>
        private static void CopyToBuffer(in byte[] data, byte[] buffer, ref int offset)
        {
            BitArray bitArray = new (data);
            bitArray.CopyTo(buffer, offset);
            offset += bitArray.Count / 8;
        }

        /// <summary>
        /// Copies int[] into a byte[] at specified position.
        /// </summary>
        /// <param name="data">A <see cref="int"/> source array.</param>
        /// <param name="buffer">A <see cref="byte"/> destination array.</param>
        /// <param name="offset">A <see cref="int"/> start index of the destination array.</param>
        private static void CopyIntToBuffer(in int[] data, byte[] buffer, ref int offset)
        {
            BitArray bitArray = new (data);
            bitArray.CopyTo(buffer, offset);
            offset += bitArray.Count / 8;
        }

        /// <summary>
        /// Finds the position of the record in a binary file by record's id.
        /// </summary>
        /// <param name="id">An <see cref="int"/> instance.</param>
        /// <returns>A <see cref="long"/> instance.</returns>
        private long GetPosition(int id)
        {
            byte[] bufferStatus = new byte[2];
            byte[] bufferId = new byte[4];

            // Loop over file and count the records with the status "NotDelete"
            for (long i = 0; i < this.fileStream.Length; i += RecordSize)
            {
                this.fileStream.Position = i;
                try
                {
                    this.fileStream.Read(bufferStatus, 0, bufferStatus.Length);
                    this.fileStream.Read(bufferId, 0, bufferId.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in reading data in {0} : {1}", FileName, e.ToString());
                    return -1;
                }

                var status = BitConverter.ToInt16(bufferStatus, 0);
                if (status == (short)Status.NotDeleted && id == BitConverter.ToInt16(bufferId, 0))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Writes the <see cref="FileCabinetRecord"/> instance to a binary file.
        /// </summary>
        /// <param name="record">A <see cref="FileCabinetRecord"/> instance.</param>
        /// <returns>true if success, false otherwise.</returns>
        private bool WriteToFile(FileCabinetRecord record, long position)
        {
            byte[] buffer = new byte[RecordSize];
            int offset = 0;

            this.fileStream.Position = position;
            var initialPosition = this.fileStream.Position;

            // Copy status to buffer
            CopyToBuffer(BitConverter.GetBytes((short)Status.NotDeleted), buffer, ref offset);

            // Copy ID
            CopyToBuffer(BitConverter.GetBytes(record.Id), buffer, ref offset);

            // Append empty spaces to First and Last names,
            // so that their sizes would be exactly 120 bytes
            int appendedSpaces = StringBufferSize - record.FirstName.Length;
            string alignedString = new string(' ', appendedSpaces) + record.FirstName;

            // Copy First Name
            CopyToBuffer(Encoding.UTF8.GetBytes(alignedString), buffer, ref offset);

            appendedSpaces = StringBufferSize - record.LastName.Length;
            alignedString = new string(' ', appendedSpaces) + record.LastName;

            // Copy Last Name
            CopyToBuffer(Encoding.UTF8.GetBytes(alignedString), buffer, ref offset);

            // Copy Birth Year
            CopyToBuffer(BitConverter.GetBytes(record.DateOfBirth.Year), buffer, ref offset);

            // Copy Birth Month
            CopyToBuffer(BitConverter.GetBytes(record.DateOfBirth.Month), buffer, ref offset);

            // Copy Birth Day
            CopyToBuffer(BitConverter.GetBytes(record.DateOfBirth.Day), buffer, ref offset);

            // Copy Work Place Number
            CopyToBuffer(BitConverter.GetBytes(record.WorkPlaceNumber), buffer, ref offset);

            // Copy SalaryType
            CopyIntToBuffer(decimal.GetBits(record.Salary), buffer, ref offset);

            // Copy DepartmentType
            CopyToBuffer(BitConverter.GetBytes(record.Department), buffer, ref offset);

            // Write buffer to the end of file
            try
            {
                this.fileStream.Write(buffer, 0, buffer.Length);
                this.fileStream.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in writing data to {0} : {1}", FileName, e.ToString());
                return false;
            }

            // Read and verify written data
            this.fileStream.Position = initialPosition;
            try
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i] != this.fileStream.ReadByte())
                    {
                        // If written data is not verified, than mark it for deletion
                        this.fileStream.Position = initialPosition;
                        this.fileStream.Write(BitConverter.GetBytes((short)Status.Deleted), 0, sizeof(short));
                        this.fileStream.Flush();
                        Console.WriteLine("The record is written incorrectly and is deleted. Try creating the record again.");
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in verifying data in: {0}", e.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Return the id of the last non-deleted record.
        /// </summary>
        /// <returns>The <see cref="int"/> instance of id.</returns>
        private int GetLastId()
        {
            int id = 0;
            byte[] bufferStatus = new byte[2];
            byte[] bufferId = new byte[4];

            // Loop over file and count the records with the status "NotDelete"
            for (long i = this.fileStream.Length - RecordSize; i > 0; i -= RecordSize)
            {
                this.fileStream.Position = i;
                try
                {
                    this.fileStream.Read(bufferStatus, 0, bufferStatus.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in reading data in {0} : {1}", FileName, e.ToString());
                    return -1;
                }

                var status = BitConverter.ToInt16(bufferStatus, 0);
                if (status == (short)Status.NotDeleted)
                {
                    this.fileStream.Read(bufferId, 0, bufferId.Length);
                    id = BitConverter.ToInt16(bufferId, 0);
                    break;
                }
            }

            return id;
        }

        /// <summary>
        /// Reads a record from the binary file at current position.
        /// </summary>
        /// <returns>A Nullable <see cref="FileCabinetRecord"/> instance.</returns>
        private FileCabinetRecord? ReadRecord(long position)
        {
            byte[] buffer = new byte[RecordSize];
            this.fileStream.Position = position;
            FileCabinetRecord record = new ();

            try
            {
                this.fileStream.Read(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in reading data in {0} : {1}", FileName, e.ToString());
                return null;
            }

            int offset = 0;

            // Return if the record is marked for deletion
            var status = BitConverter.ToInt16(buffer, offset);
            offset += sizeof(short);
            if (status == (short)Status.Deleted)
            {
                return null;
            }

            // Parse Id
            record.Id = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);

            // Parse First Name
            record.FirstName = Encoding.UTF8.GetString(buffer, offset, StringBufferSize).Trim();
            offset += StringBufferSize;

            // Parse Last Name
            record.LastName = Encoding.UTF8.GetString(buffer, offset, StringBufferSize).Trim();
            offset += StringBufferSize;

            // Parse Birth Year
            var year = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);

            // Parse Birth Month
            var month = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);

            // Parse Birth Day
            var day = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);

            // Parse Date Of Birth
            record.DateOfBirth = new DateTime(year, month, day);

            // Parse Work Place Number
            record.WorkPlaceNumber = BitConverter.ToInt16(buffer, offset);
            offset += sizeof(short);

            // Parse SalaryType
            int[] parts = new int[4];
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = BitConverter.ToInt32(buffer, offset);
                offset += sizeof(int);
            }

            bool sign = (parts[3] & 0x80000000) != 0;

            byte scale = (byte)(parts[3] >> 16 & 0x7F);
            record.Salary = new decimal(parts[0], parts[1], parts[2], sign, scale);

            // Parse DepartmentType
            record.Department = BitConverter.ToChar(buffer, offset);

            return record;
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

        private void AddRecordToSearchDictionaries(in FileCabinetRecord record, in long pos)
        {
            // Add record to firstNameDictionary
            if (this.firstNameDictionary.TryGetValue(record.FirstName.ToUpperInvariant(), out var value))
            {
                value.Add(pos);
            }
            else
            {
                this.firstNameDictionary.Add(record.FirstName.ToUpperInvariant(), new List<long> { pos });
            }

            // Add record to lastNameDictionary
            if (this.lastNameDictionary.TryGetValue(record.LastName.ToUpperInvariant(), out value))
            {
                value.Add(pos);
            }
            else
            {
                this.lastNameDictionary.Add(record.LastName.ToUpperInvariant(), new List<long> { pos });
            }

            // Add record to dateOfBirthDictionary
            string dateOfBirthString = record.DateOfBirth.ToString(DateMask, CultureInfo.InvariantCulture);
            if (this.dateOfBirthDictionary.TryGetValue(dateOfBirthString, out value))
            {
                value.Add(pos);
            }
            else
            {
                this.dateOfBirthDictionary.Add(dateOfBirthString, new List<long> { pos });
            }
        }

        private void RemoveRecordFromSearchDictionaries(FileCabinetRecord record, in long pos)
        {
            // Update firstNameDictionary
            var recordList = this.firstNameDictionary[record.FirstName.ToUpperInvariant()];
            recordList.Remove(pos);
            if (recordList.Count == 0)
            {
                this.firstNameDictionary.Remove(record.FirstName.ToUpperInvariant());
            }

            // Update lastNameDictionary
            recordList = this.lastNameDictionary[record.LastName.ToUpperInvariant()];
            recordList.Remove(pos);
            if (recordList.Count == 0)
            {
                this.lastNameDictionary.Remove(record.LastName.ToUpperInvariant());
            }

            // Update dateOfBirthDictionary
            string dateOfBirthString = record.DateOfBirth.ToString(DateMask, CultureInfo.InvariantCulture);
            recordList = this.dateOfBirthDictionary[dateOfBirthString];
            recordList.Remove(pos);
            if (recordList.Count == 0)
            {
                this.dateOfBirthDictionary.Remove(dateOfBirthString);
            }
        }

        private ReadOnlyCollection<FileCabinetRecord> ReadRecords(List<long> list)
        {
            List<FileCabinetRecord> result = new ();

            foreach (var pos in list)
            {
                var record = this.ReadRecord(pos);
                if (record != null)
                {
                    result.Add(record);
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(result);
        }

        private void UpdateSearchDictionaries()
        {
            this.firstNameDictionary.Clear();
            this.lastNameDictionary.Clear();
            this.dateOfBirthDictionary.Clear();

            long position = 0;
            while (position < this.fileStream.Length)
            {
                var record = this.ReadRecord(position);
                if (record != null)
                {
                    this.AddRecordToSearchDictionaries(record, position);
                }

                position += RecordSize;
            }
        }
    }
}
