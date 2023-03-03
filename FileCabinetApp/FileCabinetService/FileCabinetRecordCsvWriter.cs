using System.IO;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// CSV Writer Class.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private readonly TextWriter textWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="tw">The <see cref="TextWriter"/> instance.</param>
        public FileCabinetRecordCsvWriter(TextWriter tw)
        {
            this.textWriter = tw;
            string line = new (
                $"{nameof(FileCabinetRecord.Id)}," +
                $"{nameof(FileCabinetRecord.FirstName)}," +
                $"{nameof(FileCabinetRecord.LastName)}," +
                $"{nameof(FileCabinetRecord.DateOfBirth)}," +
                $"{nameof(FileCabinetRecord.Workplace)}," +
                $"{nameof(FileCabinetRecord.Salary)}," +
                $"{nameof(FileCabinetRecord.Department)}");

            this.textWriter.WriteLine(line);
        }

        /// <summary>
        /// Wirtes a <see cref="FileCabinetRecord"/> object to a CSV file.
        /// </summary>
        /// <param name="record">The <see cref="FileCabinetRecord"/> instance.</param>
        public void Write(FileCabinetRecord record)
        {
            this.textWriter.WriteLine(record.ToString().Replace(", ", ",", System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
