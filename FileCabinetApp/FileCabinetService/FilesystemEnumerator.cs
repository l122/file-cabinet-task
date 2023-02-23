using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// The Enumerator Class for FileCabinetFilesystemService.
    /// </summary>
    public class FilesystemEnumerator : IEnumerator<FileCabinetRecord>
    {
        private const int RecordSize = 278;
        private const int StringBufferSize = 120;
        private readonly List<long> list;
        private readonly FileStream fileStream;
        private FileCabinetRecord? current;
        private int index;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemEnumerator"/> class.
        /// </summary>
        /// <param name="fileStream">A <see cref="FileStream"/> instance.</param>
        /// <param name="list">A <see cref="list"/> instance of records' positions in the fileStream.</param>
        public FilesystemEnumerator(FileStream fileStream, List<long> list)
        {
            this.fileStream = fileStream;
            this.index = 0;
            this.current = null;
            this.list = list;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="FilesystemEnumerator"/> class.
        /// </summary>
        ~FilesystemEnumerator()
        {
            this.Dispose(false);
        }

        /// <inheritdoc/>
        public FileCabinetRecord Current
        {
            get
            {
                if (this.list == null || this.fileStream == null || this.current == null)
                {
                    throw new InvalidOperationException();
                }

                return this.current;
            }
        }

        /// <inheritdoc/>
        object IEnumerator.Current
        {
            get { return this.Current1; }
        }

        private object Current1
        {
            get { return this.Current; }
        }

        /// <inheritdoc/>
        public bool MoveNext()
        {
            if ((uint)this.index < (uint)this.list.Count)
            {
                try
                {
                    this.fileStream.Position = this.list[this.index];
                    byte[] buffer = new byte[RecordSize];
                    this.fileStream.Read(buffer, 0, buffer.Length);
                    this.current = ParseRecord(buffer);
                    this.index++;

                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in reading data in {0} : {1}", this.fileStream.Name, e.ToString());
                }
            }

            this.current = null;
            return false;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            this.current = null;
            this.fileStream.Position = 0;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Desposes resources.
        /// </summary>
        /// <param name="disposing">A <see cref="bool"/> parameter instance.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.current = null;
            }
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
    }
}
