using System;
using System.Collections;
using System.Collections.Generic;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// The Enumerator Class for FileCabinetMemoryService.
    /// </summary>
    public class MemoryEnumerator : IEnumerator<FileCabinetRecord>
    {
        private readonly IList<FileCabinetRecord> list;
        private FileCabinetRecord? current;
        private int index;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryEnumerator"/> class.
        /// </summary>
        /// <param name="list">An <see cref="IList{T}"/> specialized instance of records.</param>
        public MemoryEnumerator(IList<FileCabinetRecord> list)
        {
            this.list = list;
            this.index = 0;
            this.current = null;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MemoryEnumerator"/> class.
        /// </summary>
        ~MemoryEnumerator()
        {
            this.Dispose(false);
        }

        /// <inheritdoc/>
        public FileCabinetRecord Current
        {
            get
            {
                if (this.list == null || this.current == null)
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
                this.current = this.list[this.index];
                this.index++;
                return true;
            }

            this.current = null;
            return false;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            this.index = 0;
            this.current = null;
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
                this.list.Clear();
            }
        }
    }
}
