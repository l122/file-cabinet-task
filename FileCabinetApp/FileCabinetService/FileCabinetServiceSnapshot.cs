using System;
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
        private FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        public FileCabinetServiceSnapshot()
        {
            this.records = Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="param">The <see cref="FileCabinetRecord"/> array instance.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] param)
        {
            this.records = param;
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
            foreach (var record in this.records)
            {
                writer.Write(record);
            }
        }

        /// <inheritdoc/>
        public void SaveToXml(StreamWriter sw)
        {
            using var writer = new FileCabinetRecordXmlWriter(sw);
            foreach (var record in this.records)
            {
                writer.Write(record);
            }
        }
    }
}