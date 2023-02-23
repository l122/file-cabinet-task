using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// The Enumerable class for FileCabinetFilesystemService.
    /// </summary>
    public class FilesystemEnumerable : IEnumerable<FileCabinetRecord>
    {
        private const int RecordSize = 278;
        private readonly FileStream fileStream;
        private readonly List<long> list;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemEnumerable"/> class.
        /// </summary>
        /// <param name="fileStream">A <see cref="FileStream"/> instance.</param>
        public FilesystemEnumerable(FileStream fileStream)
        {
            this.fileStream = fileStream;
            this.list = new ();
            this.InitList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemEnumerable"/> class.
        /// </summary>
        /// <param name="fileStream">A <see cref="FileStream"/> instance.</param>
        /// <param name="list">A <see cref="list"/> instance of records' positions in the fileStream.</param>
        public FilesystemEnumerable(FileStream fileStream, List<long> list)
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
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            return new FilesystemEnumerator(this.fileStream, this.list);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator1();
        }

        private IEnumerator GetEnumerator1()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Searches for non-deleted records and stores their positions in the list.
        /// </summary>
        private void InitList()
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
