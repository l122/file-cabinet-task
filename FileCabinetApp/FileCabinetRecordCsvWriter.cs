using System.IO;
using System.Text;

namespace FileCabinetApp
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
                "id," +
                "First Name," +
                "Last Name," +
                "Date of Birth," +
                "Work Place Number," +
                "Salary," +
                "Department");

            this.textWriter.WriteLine(line);
        }

        /// <summary>
        /// Wirtes a <see cref="FileCabinetRecord"/> object to a CSV file.
        /// </summary>
        /// <param name="record">The <see cref="FileCabinetRecord"/> instance.</param>
        public void Write(FileCabinetRecord record)
        {
            this.textWriter.Write(record.ToString().Replace(", ", ",", System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
