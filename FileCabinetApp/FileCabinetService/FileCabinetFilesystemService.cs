using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using FileCabinetApp.StaticClasses;
using FileCabinetApp.Validators;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// Helper class for storing data in a binary file.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService, IDisposable
    {
        private const string FileName = "cabinet-records.db";
        private const int RecordSize = 278;
        private const int StringBufferSize = 120;
        private readonly IRecordValidator validator;
        private FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="validator">The <see cref="IRecordValidator"/> specialised instance.</param>
        public FileCabinetFilesystemService(IRecordValidator validator)
        {
            this.validator = validator;
            this.fileStream = File.Open(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
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

            return this.WriteToFile(record, pos);
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
            List<FileCabinetRecord> result = new ();

            // Loop over file and count the records with the status "NotDelete"
            for (long i = 0; i < this.fileStream.Length; i += RecordSize)
            {
                var record = this.ReadRecord(i);
                if (record != null && record.FirstName.Equals(firstName.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(record);
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(result);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            List<FileCabinetRecord> result = new ();

            // Loop over file and count the records with the status "NotDelete"
            for (long i = 0; i < this.fileStream.Length; i += RecordSize)
            {
                var record = this.ReadRecord(i);
                if (record != null && record.LastName.Equals(lastName.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(record);
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(result);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString)
        {
            List<FileCabinetRecord> result = new ();
            DateTime? date;
            var conversionResult = Converter.DateConverter(dateOfBirthString);
            if (!conversionResult.Item1)
            {
                Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
            }

            date = conversionResult.Item3;

            // Loop over file and count the records with the status "NotDelete"
            for (long i = 0; i < this.fileStream.Length; i += RecordSize)
            {
                var record = this.ReadRecord(i);
                if (record != null && date.Equals(record.DateOfBirth))
                {
                    result.Add(record);
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(result);
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
                if (position != -1)
                {
                    this.WriteToFile(record, position);
                }
                else
                {
                    this.WriteToFile(record, this.fileStream.Length);
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
                    this.WriteToFile(record, position);
                    position += RecordSize;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while clearing file {0} : {1}", FileName, e.ToString());
                return (-1, -1);
            }

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
    }
}
