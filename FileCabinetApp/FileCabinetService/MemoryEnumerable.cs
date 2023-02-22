using System.Collections;
using System.Collections.Generic;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// The Enumerable class for FileCabinetMemoryService.
    /// </summary>
    public class MemoryEnumerable : IEnumerable<FileCabinetRecord>
    {
        private readonly List<FileCabinetRecord> list;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryEnumerable"/> class.
        /// </summary>
        public MemoryEnumerable()
        {
            this.list = new ();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryEnumerable"/> class.
        /// </summary>
        /// <param name="list">A <see cref="List{T}"/> instance of records.</param>
        public MemoryEnumerable(List<FileCabinetRecord> list)
        {
            this.list = list;
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            return new MemoryEnumerator(this.list);
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
