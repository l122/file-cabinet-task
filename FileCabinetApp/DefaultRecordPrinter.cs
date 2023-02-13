using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// A default class for printing records.
    /// </summary>
    public class DefaultRecordPrinter : IRecordPrinter
    {
        /// <inheritdoc/>
        public void Print(IEnumerable<FileCabinetRecord> records)
        {
            if (records == null)
            {
                return;
            }

            foreach (var record in records)
            {
                Console.WriteLine(record.ToString());
            }
        }
    }
}
