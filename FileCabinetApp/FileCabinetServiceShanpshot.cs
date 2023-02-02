﻿using System.IO;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// File Cabinet Service Snapshot class.
    /// </summary>
    public class FileCabinetServiceShanpshot : IFileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceShanpshot"/> class.
        /// </summary>
        /// <param name="param">The <see cref="FileCabinetRecord"/> array instance.</param>
        public FileCabinetServiceShanpshot(FileCabinetRecord[] param)
        {
            this.records = param;
        }

        /// <summary>
        /// Saves a snapshot to a csv file.
        /// </summary>
        /// <param name="sw">A <see cref="StreamWriter"/> instance.</param>
        public void SaveToCsv(StreamWriter sw)
        {
            var writer = new FileCabinetRecordCsvWriter(sw);
            foreach (var record in this.records)
            {
                writer.Write(record);
            }
        }

        /// <summary>
        /// Saves a snapshot to an xml file.
        /// </summary>
        /// <param name="sw">A <see cref="StreamWriter"/> instance.</param>
        public void SaveToXml(StreamWriter sw)
        {
            using (var writer = new FileCabinetRecordXmlWriter(sw))
            {
                foreach (var record in this.records)
                {
                    writer.Write(record);
                }
            }
        }
    }
}