using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Helper class for storing data in a binary file.
    /// </summary>
    public class FileCabinetFilesystemService : FileCabinetService, IFileCabinetService, IDisposable
    {
        private const string FileName = "cabinet-records.db";
        private const int RecordSize = 278;
        private const int StringBufferSize = 120;
        private readonly FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="validator">The <see cref="IRecordValidator"/> specialised instance.</param>
        public FileCabinetFilesystemService(IRecordValidator validator)
            : base(validator)
        {
            this.fileStream = File.Open(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        private enum Status : short
        {
            NotDeleted,
            Deleted,
        }

        /// <summary>
        /// Creates a new record.
        /// </summary>
        /// <returns>The <see cref="int"/> instance of record's id.</returns>
        public int CreateRecord()
        {
            var record = this.GetInputData();

            // Update record id, because the default id = 0
            record.Id = this.GetStat() + 1;

            if (this.WriteToFile(record))
            {
                return record.Id;
            }

            return -1;
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
            return (int)(this.fileStream.Length / RecordSize);
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

        /// <summary>
        /// Writes the <see cref="FileCabinetRecord"/> instance to a binary file.
        /// </summary>
        /// <param name="record">A <see cref="FileCabinetRecord"/> instance.</param>
        /// <returns>true if success, false otherwise.</returns>
        private bool WriteToFile(FileCabinetRecord record)
        {
            byte[] buffer = new byte[RecordSize];
            int offset = 0;

            this.fileStream.Seek(this.fileStream.Length, SeekOrigin.Begin);
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

            // Copy Salary
            CopyIntToBuffer(decimal.GetBits(record.Salary), buffer, ref offset);

            // Copy Department
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

            // Copies byte[] into a byte[] at specified position
            void CopyToBuffer(in byte[] data, byte[] buffer, ref int offset)
            {
                BitArray bitArray = new (data);
                bitArray.CopyTo(buffer, offset);
                offset += bitArray.Count / 8;
            }

            // Copies int[] into a byte[] at specified position
            void CopyIntToBuffer(in int[] data, byte[] buffer, ref int offset)
            {
                BitArray bitArray = new (data);
                bitArray.CopyTo(buffer, offset);
                offset += bitArray.Count / 8;
            }

            return true;
        }
    }
}
