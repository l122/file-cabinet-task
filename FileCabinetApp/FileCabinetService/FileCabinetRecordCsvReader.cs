using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// Helper class for reading data from csv.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private readonly StreamReader reader;
        private readonly Dictionary<string, int> headers;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="streamReader">A <see cref="StreamReader"/> instance.</param>
        public FileCabinetRecordCsvReader(StreamReader streamReader)
        {
            this.reader = streamReader;
            this.headers = new ();
        }

        /// <summary>
        /// Reads csv data into a list.
        /// </summary>
        /// <returns>The <see cref="IList{T}"/> instance of <see cref="FileCabinetRecord"/> items.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> records = new ();

            if (!this.ReadAndValidateHeaders())
            {
                return records;
            }

            while (!this.reader.EndOfStream)
            {
                string? line;
                try
                {
                    line = this.reader.ReadLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while reading a line from stream reader: {0}.", e.ToString());
                    continue;
                }

                string[] values = Array.Empty<string>();
                if (line != null)
                {
                    values = line.Split(',');
                }

                if (values.Length == this.headers.Count)
                {
                    var record = this.ParseStringArrayToRecord(values);

                    if (record != null)
                    {
                        records.Add(record);
                    }
                }
            }

            return records;
        }

        /// <summary>
        /// Parses an array of strings into a record.
        /// </summary>
        /// <param name="values">An array of <see cref="string"/>.</param>
        /// <returns>A <see cref="FileCabinetRecord"/> instance.</returns>
        private FileCabinetRecord? ParseStringArrayToRecord(string[] values)
        {
            FileCabinetRecord? record = null;
            if (int.TryParse(values[this.headers[nameof(record.Id)]], out var id)
                && DateTime.TryParse(values[this.headers[nameof(record.DateOfBirth)]], out var dateOfBirth)
                && short.TryParse(values[this.headers[nameof(record.Workplace)]], out var workPlaceNumber)
                && decimal.TryParse(values[this.headers[nameof(record.Salary)]], out var salary)
                && char.TryParse(values[this.headers[nameof(record.Department)]], out var department))
            {
                record = new FileCabinetRecord()
                {
                    Id = id,
                    FirstName = values[this.headers[nameof(record.FirstName)]],
                    LastName = values[this.headers[nameof(record.LastName)]],
                    DateOfBirth = dateOfBirth,
                    Workplace = workPlaceNumber,
                    Salary = salary,
                    Department = department.ToString().ToUpper(CultureInfo.InvariantCulture)[0],
                };
            }

            return record;
        }

        private bool ReadAndValidateHeaders()
        {
            var record = new FileCabinetRecord();
            var fileCabinetRecordFields = new string[]
            {
                nameof(record.Id),
                nameof(record.FirstName),
                nameof(record.LastName),
                nameof(record.DateOfBirth),
                nameof(record.Workplace),
                nameof(record.Salary),
                nameof(record.Department),
            };

            string? line;
            try
            {
                line = this.reader.ReadLine();
            }
            catch (Exception)
            {
                return false;
            }

            string[] values = Array.Empty<string>();
            if (line != null)
            {
                values = line.Split(',');
            }

            for (int i = 0; i < values.Length; i++)
            {
                var index = Array.FindIndex(fileCabinetRecordFields, 0, fileCabinetRecordFields.Length, p => p.Equals(values[i], StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    this.headers[fileCabinetRecordFields[index]] = i;
                }
            }

            return true;
        }
    }
}
