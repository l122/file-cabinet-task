using System.Collections;
using System.Collections.Generic;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// The Enumerable class for FileCabinetMemoryService.
    /// </summary>
    public class MemoryEnumerable : IEnumerable<FileCabinetRecord>
    {
        private readonly IList<FileCabinetRecord> list;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryEnumerable"/> class.
        /// </summary>
        public MemoryEnumerable()
        {
            this.list = new List<FileCabinetRecord>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryEnumerable"/> class.
        /// </summary>
        /// <param name="list">An <see cref="IList{T}"/> specialized instance of records.</param>
        public MemoryEnumerable(IList<FileCabinetRecord> list)
        {
            this.list = list;
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            foreach (var record in this.list)
            {
                yield return record;
            }
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
