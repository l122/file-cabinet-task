using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// An interface for the Record Printer Classes.
    /// </summary>
    public interface IRecordPrinter
    {
        /// <summary>
        /// Prints records.
        /// </summary>
        /// <param name="records">A collection of records of type <see cref="FileCabinetRecord"/>.</param>
        public void Print(IEnumerable<FileCabinetRecord> records);
    }
}