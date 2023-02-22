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
        private readonly FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemEnumerable"/> class.
        /// </summary>
        /// <param name="fileStream">A <see cref="FileStream"/> instance.</param>
        public FilesystemEnumerable(FileStream fileStream)
        {
            this.fileStream = fileStream;
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            return new FilesystemEnumerator(fileStream);
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
    }
}
