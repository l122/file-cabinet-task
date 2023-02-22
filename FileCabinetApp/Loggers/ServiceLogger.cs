using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.Loggers
{
    /// <summary>
    /// Logs FileCabinetService methods calls in a text file.
    /// </summary>
    public class ServiceLogger : IFileCabinetService
    {
        private const string FileName = "log.txt";

        private readonly IFileCabinetService service;
        private readonly StreamWriter sw;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="service">An <see cref="IFileCabinetService"/> specialized instance.</param>
        public ServiceLogger(IFileCabinetService service)
        {
            this.service = service;
            this.sw = File.CreateText(FileName);
        }

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetRecord record)
        {
            this.sw.Write("{0} - Calling Create() with ", GetCurrentTime());
            this.sw.Write("FirstName = '{0}', ", record.FirstName);
            this.sw.Write("LastName = '{0}', ", record.LastName);
            this.sw.Write("DateOfBirth = '{0}', ", record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
            this.sw.Write("Workplace = '{0}', ", record.WorkPlaceNumber);
            this.sw.Write("Salary = '{0}', ", record.Salary);
            this.sw.WriteLine("Department = '{0}'.", record.Department);
            this.sw.Flush();

            var result = this.service.CreateRecord(record);

            this.sw.WriteLine("{0} - Create() returned '{1}'", GetCurrentTime(), result);
            this.sw.Flush();

            return result;
        }

        /// <inheritdoc/>
        public bool EditRecord(FileCabinetRecord record)
        {
            this.sw.Write("{0} - Calling Edit() with ", GetCurrentTime());
            this.sw.Write("FirstName = '{0}', ", record.FirstName);
            this.sw.Write("LastName = '{0}', ", record.LastName);
            this.sw.Write("DateOfBirth = '{0}', ", record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
            this.sw.Write("Workplace = '{0}', ", record.WorkPlaceNumber);
            this.sw.Write("Salary = '{0}', ", record.Salary);
            this.sw.WriteLine("Department = '{0}'.", record.Department);
            this.sw.Flush();

            var result = this.service.EditRecord(record);

            this.sw.WriteLine("{0} - Edit() returned '{1}'", GetCurrentTime(), result.ToString());
            this.sw.Flush();

            return result;
        }

        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.sw.Write("{0} - Calling FindByFirstName() with ", GetCurrentTime());
            this.sw.WriteLine("FirstName = '{0}'.", firstName);
            this.sw.Flush();

            var result = this.service.FindByFirstName(firstName);

            this.sw.WriteLine("{0} - FindByFirstName() returned an iterator.", GetCurrentTime());
            this.sw.Flush();

            return result;
        }

        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.sw.Write("{0} - Calling FindByLastName() with ", GetCurrentTime());
            this.sw.WriteLine("LastName = '{0}'.", lastName);
            this.sw.Flush();

            var result = this.service.FindByLastName(lastName);

            this.sw.WriteLine("{0} - FindByLastName() returned an iterator.", GetCurrentTime());
            this.sw.Flush();

            return result;
        }

        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString)
        {
            this.sw.Write("{0} - Calling FindByDateOfBirth() with ", GetCurrentTime());
            this.sw.WriteLine("DateOfBirth = '{0}'.", dateOfBirthString);
            this.sw.Flush();

            var result = this.service.FindByDateOfBirth(dateOfBirthString);

            this.sw.WriteLine("{0} - FindByDateOfBirth() returned an iterator.", GetCurrentTime());
            this.sw.Flush();

            return result;
        }

        /// <inheritdoc/>
        public FileCabinetRecord? FindById(int id)
        {
            // No need to log it, because this method is always called by the other logged methods.
            return this.service.FindById(id);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            this.sw.WriteLine("{0} - Calling GetRecords().", GetCurrentTime());
            this.sw.Flush();

            var result = this.service.GetRecords();

            this.sw.WriteLine("{0} - GetRecords() returned an iterator.", GetCurrentTime());
            this.sw.Flush();

            return result;
        }

        /// <inheritdoc/>
        public (int, int) GetStat()
        {
            this.sw.WriteLine("{0} - Calling GetStat().", GetCurrentTime());
            this.sw.Flush();

            var result = this.service.GetStat();

            this.sw.WriteLine("{0} - GetStat() returned {1} total record(s), {2} deleted record(s).", GetCurrentTime(), result.Item1, result.Item2);
            this.sw.Flush();

            return result;
        }

        /// <inheritdoc/>
        public IFileCabinetServiceSnapshot MakeSnapshot()
        {
            // No need to log it, because this method is always called by the other logged methods.
            return this.service.MakeSnapshot();
        }

        /// <inheritdoc/>
        public (int, int) Purge()
        {
            this.sw.WriteLine("{0} - Calling Purge().", GetCurrentTime());
            this.sw.Flush();

            var result = this.service.Purge();

            this.sw.WriteLine("{0} - Purge() returned '{1}' of '{2}' records were purged.", GetCurrentTime(), result.Item1, result.Item2);
            this.sw.Flush();

            return result;
        }

        /// <inheritdoc/>
        public bool RemoveRecord(int id)
        {
            this.sw.Write("{0} - Calling RemoveRecord() with ", GetCurrentTime());
            this.sw.WriteLine("id = '{0}'.", id);
            this.sw.Flush();

            var result = this.service.RemoveRecord(id);

            this.sw.WriteLine("{0} - RemoveRecord() returned '{1}'.", GetCurrentTime(), result.ToString());
            this.sw.Flush();

            return result;
        }

        /// <inheritdoc/>
        public int Restore(IFileCabinetServiceSnapshot snapshot)
        {
            this.sw.WriteLine("{0} - Calling Restore().", GetCurrentTime());
            this.sw.Flush();

            var result = this.service.Restore(snapshot);

            this.sw.WriteLine("{0} - Restore() restored '{1}' records.", GetCurrentTime(), result);
            this.sw.Flush();

            return result;
        }

        /// <summary>
        /// Return a string of the current date and time.
        /// </summary>
        /// <returns>A <see cref="string"/> instance of the current date and time.</returns>
        private static string GetCurrentTime()
        {
            return DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
        }
    }
}
