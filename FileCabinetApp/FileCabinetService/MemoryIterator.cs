using System;
using System.Collections.Generic;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// The Iterator for FileCabinetMemoryService.
    /// </summary>
    public class MemoryIterator : IRecordIterator
    {
        private readonly List<FileCabinetRecord> records;
        private int current;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        /// <param name="list">A <see cref="List{T}"/> instance.</param>
        public MemoryIterator(List<FileCabinetRecord> list)
        {
            this.records = list;
            this.current = -1;
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetNext()
        {
            this.current++;
            return this.records[this.current];
        }

        /// <inheritdoc/>
        public long GetPosition()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool HasMore()
        {
            return (this.current + 1) < this.records.Count;
        }
    }
}
