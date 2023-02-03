﻿using System;
using System.Collections;
using System.Collections.Generic;
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
            List<FileCabinetRecord> records = new ();
            byte[] buffer = new byte[RecordSize];
            this.fileStream.Position = 0;

            while (this.fileStream.Position < this.fileStream.Length)
            {
                int offset = 0;
                try
                {
                    this.fileStream.Read(buffer, 0, buffer.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in reading data in {0} : {1}", FileName, e.ToString());
                }

                // Skip parsing the record if it's marked for deletion
                var status = BitConverter.ToInt16(buffer, offset);
                offset += sizeof(short);
                if (status == (short)Status.Deleted)
                {
                    continue;
                }

                FileCabinetRecord record = new ();

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

                // Parse Salary
                int[] parts = new int[4];
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i] = BitConverter.ToInt32(buffer, offset);
                    offset += sizeof(int);
                }

                bool sign = (parts[3] & 0x80000000) != 0;

                byte scale = (byte)((parts[3] >> 16) & 0x7F);
                record.Salary = new decimal(parts[0], parts[1], parts[2], sign, scale);

                // Parse Department
                record.Department = BitConverter.ToChar(buffer, offset);

                records.Add(record);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <summary>
        /// Returns the number of records.
        /// </summary>
        /// <returns>The <see cref="int"/> instance of total number of records.</returns>
        public int GetStat()
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

            return counter;
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
