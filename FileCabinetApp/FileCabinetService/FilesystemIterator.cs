using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// The iterator for FileCabinetFilesystemService.
    /// </summary>
    public class FilesystemIterator : IRecordIterator
    {
        private const int RecordSize = 278;
        private const int StringBufferSize = 120;
        private readonly List<long> list = new List<long>();
        private readonly FileStream fileStream;
        private int current = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemIterator"/> class.
        /// </summary>
        /// <param name="fileStream">A <see cref="FileStream"/> instance.</param>
        public FilesystemIterator(FileStream fileStream)
        {
            this.fileStream = fileStream;
            this.Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemIterator"/> class.
        /// </summary>
        /// <param name="fileStream">A <see cref="FileStream"/> instance.</param>
        /// <param name="list">A <see cref="List{T}"/> instance.</param>
        public FilesystemIterator(FileStream fileStream, List<long> list)
        {
            this.fileStream = fileStream;
            this.list = list;
        }

        private enum Status : short
        {
            NotDeleted,
            Deleted,
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetNext()
        {
            this.current++;
            this.fileStream.Position = this.list[this.current];
            byte[] buffer = new byte[RecordSize];

            try
            {
                this.fileStream.Read(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in reading data in {0} : {1}", this.fileStream.Name, e.ToString());
                return new FileCabinetRecord();
            }

            return ParseRecord(buffer);
        }

        /// <inheritdoc/>
        public bool HasMore()
        {
            return (this.current + 1) < this.list.Count;
        }

        /// <inheritdoc/>
        public long GetPosition()
        {
            return this.list[this.current];
        }

        private static FileCabinetRecord ParseRecord(byte[] buffer)
        {
            FileCabinetRecord record = new ();

            // Skip status bytes
            int offset = sizeof(short);

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

        private void Init()
        {
            byte[] buffer = new byte[sizeof(short)];

            for (long i = 0; i < this.fileStream.Length; i += RecordSize)
            {
                try
                {
                    this.fileStream.Position = i;
                    this.fileStream.Read(buffer, 0, buffer.Length);
                    var status = BitConverter.ToInt16(buffer, 0);
                    if (status == (short)Status.NotDeleted)
                    {
                        this.list.Add(i);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in reading record status at position {0} in {1} : {2}", i, this.fileStream.Name, e.ToString());
                }
            }
        }
    }
}
