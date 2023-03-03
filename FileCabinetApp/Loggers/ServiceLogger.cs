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
            this.sw.Write("Workplace = '{0}', ", record.Workplace);
            this.sw.Write("Salary = '{0}', ", record.Salary);
            this.sw.WriteLine("Department = '{0}'.", record.Department);
            this.sw.Flush();

            var result = this.service.CreateRecord(record);

            this.sw.WriteLine("{0} - Create() returned '{1}'", GetCurrentTime(), result);
            this.sw.Flush();

            return result;
        }

        /// <inheritdoc/>
        public bool Insert(FileCabinetRecord record)
        {
            this.sw.Write("{0} - Calling Insert() with ", GetCurrentTime());
            this.sw.Write("FirstName = '{0}', ", record.FirstName);
            this.sw.Write("LastName = '{0}', ", record.LastName);
            this.sw.Write("DateOfBirth = '{0}', ", record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
            this.sw.Write("Workplace = '{0}', ", record.Workplace);
            this.sw.Write("Salary = '{0}', ", record.Salary);
            this.sw.WriteLine("Department = '{0}'.", record.Department);
            this.sw.Flush();

            var result = this.service.Insert(record);

            this.sw.WriteLine("{0} - Insert() returned '{1}'", GetCurrentTime(), result.ToString());
            this.sw.Flush();

            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindById(int id)
        {
            // No need to log it, because this method is always called by the other logged methods.
            return this.service.FindById(id);
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
        public int Restore(IFileCabinetServiceSnapshot snapshot)
        {
            this.sw.WriteLine("{0} - Calling Restore().", GetCurrentTime());
            this.sw.Flush();

            var result = this.service.Restore(snapshot);

            this.sw.WriteLine("{0} - Restore() restored '{1}' records.", GetCurrentTime(), result);
            this.sw.Flush();

            return result;
        }

        /// <inheritdoc/>
        public string Delete(string expression)
        {
            this.sw.WriteLine("{0} - Calling Delete().", GetCurrentTime());
            this.sw.Flush();

            var result = this.service.Delete(expression);

            this.sw.WriteLine("{0} - Delete() return the following message: {1}", GetCurrentTime(), result);
            this.sw.Flush();

            return result;
        }

        /// <inheritdoc/>
        public string Update(string expression)
        {
            this.sw.WriteLine("{0} - Calling Update().", GetCurrentTime());
            this.sw.Flush();

            var result = this.service.Update(expression);

            this.sw.WriteLine("{0} - Update() return the following message: {1}", GetCurrentTime(), result);
            this.sw.Flush();

            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> SelectRecords(string expression)
        {
            this.sw.WriteLine("{0} - Calling SelectRecords().", GetCurrentTime());
            this.sw.Flush();

            var result = this.service.SelectRecords(expression);

            this.sw.WriteLine("{0} - SelectRecords() returned an iterator.", GetCurrentTime());
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
