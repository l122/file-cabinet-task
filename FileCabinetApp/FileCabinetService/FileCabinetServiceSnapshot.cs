using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// File Cabinet Service Snapshot class.
    /// </summary>
    public class FileCabinetServiceSnapshot : IFileCabinetServiceSnapshot
    {
        private readonly IRecordIterator iterator;
        private FileCabinetRecord[] records = Array.Empty<FileCabinetRecord>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        public FileCabinetServiceSnapshot()
        {
            this.iterator = new MemoryIterator(new List<FileCabinetRecord>(this.records));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="iterator">An <see cref="IRecordIterator"/> specialized instance.</param>
        public FileCabinetServiceSnapshot(IRecordIterator iterator)
        {
            this.iterator = iterator;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> Records { get => new (this.records); }

        /// <inheritdoc/>
        public void LoadFromCsv(FileStream fileStream)
        {
            var reader = new FileCabinetRecordCsvReader(new StreamReader(fileStream));
            this.records = reader.ReadAll().ToArray();
        }

        /// <inheritdoc/>
        public void LoadFromXml(FileStream fileStream)
        {
            var reader = new FileCabinetRecordXmlReader(new StreamReader(fileStream));
            this.records = reader.ReadAll().ToArray();
        }

        /// <inheritdoc/>
        public void SaveToCsv(StreamWriter sw)
        {
            var writer = new FileCabinetRecordCsvWriter(sw);
            while (this.iterator.HasMore())
            {
                writer.Write(this.iterator.GetNext());
            }
        }

        /// <inheritdoc/>
        public void SaveToXml(StreamWriter sw)
        {
            using var writer = new FileCabinetRecordXmlWriter(sw);
            while (this.iterator.HasMore())
            {
                writer.Write(this.iterator.GetNext());
            }
        }
    }
}