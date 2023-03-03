using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FileCabinetApp.FileCabinetService;
using FileCabinetApp.StaticClasses;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles the SelectRecords Command Request.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private const string DateMask = "dd-MMM-yyyy";
        private const string Trigger = "select";
        private const string Id = "id";
        private const string FirstName = "firstname";
        private const string LastName = "lastname";
        private const string DateOfBirth = "dateofbirth";
        private const string Workplace = "workplace";
        private const string Salary = "salary";
        private const string Department = "department";

        private readonly Dictionary<string, int> columnsWidth = new ()
        {
            { Id, 0 },
            { FirstName, 0 },
            { LastName, 0 },
            { DateOfBirth, 0 },
            { Workplace, 0 },
            { Salary, 0 },
            { Department, 0 },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">A <see cref="IFileCabinetService"/> specialized instance.</param>
        public SelectCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (CanHandle(Trigger, appCommandRequest.Command))
            {
                this.Select(appCommandRequest.Parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Select(string expression)
        {
            expression = expression.ToLowerInvariant();

            // Select records according to the where expression.
            var records = this.service.SelectRecords(expression);
            if (!records.Any())
            {
                Console.WriteLine("No records found.");
                Console.WriteLine();
                return;
            }

            // Parse fields.
            var end = expression.IndexOf("where ", StringComparison.InvariantCultureIgnoreCase);
            var fields = end == -1
                ? expression.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                : expression[0..end].Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (fields.Length == 0)
            {
                fields = new string[] { Id, FirstName, LastName, DateOfBirth, Workplace, Salary, Department };
            }

            this.SetColumnsWidth(records, fields);

            // Pring headers.
            Console.WriteLine();
            Console.WriteLine(this.GetLine(fields));
            Console.WriteLine(this.AppendHeaders(fields));
            Console.WriteLine(this.GetLine(fields));

            // Pring records.
            foreach (var record in records)
            {
                Console.WriteLine(this.AppendRecord(fields, record));
            }

            // Printer total.
            Console.WriteLine(this.GetLine(fields));
            var count = records.Count();
            if (count == 1)
            {
                Console.WriteLine("Displayed {0} record.", count);
            }
            else
            {
                Console.WriteLine("Displayed {0} records.", count);
            }

            Console.WriteLine();
        }

        private void SetColumnsWidth(IEnumerable<FileCabinetRecord> records, string[] fields)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            if (fields == null)
            {
                throw new ArgumentNullException(nameof(fields));
            }

            foreach (var field in fields)
            {
                if (!this.columnsWidth.TryGetValue(field, out _))
                {
                    continue;
                }

                this.columnsWidth[field] = field switch
                {
                    Id => Math.Max(records.Max(p => p.Id.ToString(CultureInfo.InvariantCulture).Length), field.Length) + 2,
                    FirstName => Math.Max(records.Max(p => p.FirstName.Length), field.Length) + 2,
                    LastName => Math.Max(records.Max(p => p.LastName.Length), field.Length) + 2,
                    DateOfBirth => Math.Max(records.Max(p => p.DateOfBirth.ToString(DateMask, CultureInfo.InvariantCulture).Length), DateMask.Length) + 2,
                    Workplace => Math.Max(records.Max(p => p.Workplace.ToString(CultureInfo.InvariantCulture).Length), field.Length) + 2,
                    Salary => Math.Max(records.Max(p => p.Salary.ToString(CultureInfo.InvariantCulture).Length), field.Length) + 2,
                    Department => Math.Max(records.Max(p => p.Department.ToString().Length), Department.Length) + 2,
                    _ => 0,
                };
            }
        }

        private string GetLine(string[] fields)
        {
            const char cross = '+';
            const char dash = '-';

            if (fields == null)
            {
                throw new ArgumentNullException(nameof(fields));
            }

            var sb = new StringBuilder();
            foreach (var field in fields)
            {
                if (this.columnsWidth.TryGetValue(field, out var width))
                {
                    sb.Append(cross);
                    sb.Append(dash, width);
                }
            }

            sb.Append(cross);

            return sb.ToString();
        }

        private string AppendHeaders(string[] fields)
        {
            if (fields == null)
            {
                throw new ArgumentNullException(nameof(fields));
            }

            var result = new StringBuilder();
            var sep = "|";

            foreach (var field in fields)
            {
                if (!this.columnsWidth.TryGetValue(field, out var padding))
                {
                    continue;
                }

                result.Append(sep);
                var value = field switch
                {
                    Id => $"{nameof(FileCabinetRecord.Id)} ".PadLeft(padding),
                    FirstName => $" {nameof(FileCabinetRecord.FirstName)}".PadRight(padding),
                    LastName => $" {nameof(FileCabinetRecord.LastName)}".PadRight(padding),
                    DateOfBirth => $" {nameof(FileCabinetRecord.DateOfBirth)}".PadRight(padding),
                    Workplace => $"{nameof(FileCabinetRecord.Workplace)} ".PadLeft(padding),
                    Salary => $"{nameof(FileCabinetRecord.Salary)} ".PadLeft(padding),
                    Department => $" {nameof(FileCabinetRecord.Department)}".PadRight(padding),
                    _ => string.Empty,
                };

                result.Append(value);
            }

            result.Append(sep);

            return result.ToString();
        }

        private string AppendRecord(string[] fields, FileCabinetRecord record)
        {
            if (fields == null)
            {
                throw new ArgumentNullException(nameof(fields));
            }

            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            var result = new StringBuilder();
            var sep = "|";

            foreach (var field in fields)
            {
                if (!this.columnsWidth.TryGetValue(field, out var padding))
                {
                    continue;
                }

                result.Append(sep);
                var value = field switch
                {
                    Id => $"{record.Id} ".PadLeft(padding),
                    FirstName => $" {record.FirstName}".PadRight(padding),
                    LastName => $" {record.LastName}".PadRight(padding),
                    DateOfBirth => $" {record.DateOfBirth.ToString(DateMask, CultureInfo.InvariantCulture)}".PadRight(padding),
                    Workplace => $"{record.Workplace} ".PadLeft(padding),
                    Salary => $"{record.Salary} ".PadLeft(padding),
                    Department => $" {record.Department}".PadRight(padding),
                    _ => string.Empty,
                };

                result.Append(value);
            }

            result.Append(sep);

            return result.ToString();
        }
    }
}
