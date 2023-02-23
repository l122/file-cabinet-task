using System;
using System.Collections.Generic;
using System.IO;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// File Cabinet Service Snapshot class.
    /// </summary>
    public class FileCabinetServiceSnapshot : IFileCabinetServiceSnapshot
    {
        private IEnumerable<FileCabinetRecord> enumerableRecords;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        public FileCabinetServiceSnapshot()
        {
            this.enumerableRecords = new MemoryEnumerable(new List<FileCabinetRecord>());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="enumerable">An <see cref="IEnumerator{T}"/> specialized instance.</param>
        public FileCabinetServiceSnapshot(IEnumerable<FileCabinetRecord> enumerable)
        {
            this.enumerableRecords = enumerable;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> Records { get => this.enumerableRecords; }

        /// <inheritdoc/>
        public void LoadFromCsv(FileStream fileStream)
        {
            var reader = new FileCabinetRecordCsvReader(new StreamReader(fileStream));
            this.enumerableRecords = new MemoryEnumerable(reader.ReadAll());
        }

        /// <inheritdoc/>
        public void LoadFromXml(FileStream fileStream)
        {
            var reader = new FileCabinetRecordXmlReader(new StreamReader(fileStream));
            this.enumerableRecords = new MemoryEnumerable(reader.ReadAll());
        }

        /// <inheritdoc/>
        public void SaveToCsv(StreamWriter sw)
        {
            var writer = new FileCabinetRecordCsvWriter(sw);
            var record = this.enumerableRecords.GetEnumerator();
            while (record.MoveNext())
            {
                writer.Write(record.Current);
            }
        }

        /// <inheritdoc/>
        public void SaveToXml(StreamWriter sw)
        {
            using var writer = new FileCabinetRecordXmlWriter(sw);
            var record = this.enumerableRecords.GetEnumerator();
            while (record.MoveNext())
            {
                writer.Write(record.Current);
            }
        }
    }
}