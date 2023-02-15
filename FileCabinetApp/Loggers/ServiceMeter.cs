using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.Loggers
{
    /// <summary>
    /// Measures performance.
    /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService service;
        private readonly Stopwatch stopwatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="service">An <see cref="IFileCabinetService"/> specialized instance.</param>
        public ServiceMeter(IFileCabinetService service)
        {
            this.service = service;
            this.stopwatch = new ();
        }

        /// <inheritdoc/>
        public int CreateRecord()
        {
            this.stopwatch.Restart();
            var result = this.service.CreateRecord();
            this.stopwatch.Stop();
            Log(this.stopwatch.ElapsedTicks, "Create");

            return result;
        }

        /// <inheritdoc/>
        public void EditRecord(int id)
        {
            this.stopwatch.Restart();
            this.service.EditRecord(id);
            this.stopwatch.Stop();
            Log(this.stopwatch.ElapsedTicks, "Edit");
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirthString)
        {
            this.stopwatch.Restart();
            var result = this.service.FindByDateOfBirth(dateOfBirthString);
            this.stopwatch.Stop();
            Log(this.stopwatch.ElapsedTicks, "FindByDateOfBirth");

            return result;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.stopwatch.Restart();
            var result = this.service.FindByFirstName(firstName);
            this.stopwatch.Stop();
            Log(this.stopwatch.ElapsedTicks, "FindByFirstName");

            return result;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.stopwatch.Restart();
            var result = this.service.FindByLastName(lastName);
            this.stopwatch.Stop();
            Log(this.stopwatch.ElapsedTicks, "FindByLastName");

            return result;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.stopwatch.Restart();
            var result = this.service.GetRecords();
            this.stopwatch.Stop();
            Log(this.stopwatch.ElapsedTicks, "GetRecords");

            return result;
        }

        /// <inheritdoc/>
        public (int, int) GetStat()
        {
            this.stopwatch.Restart();
            var result = this.service.GetStat();
            this.stopwatch.Stop();
            Log(this.stopwatch.ElapsedTicks, "GetStat");

            return result;
        }

        /// <inheritdoc/>
        public IFileCabinetServiceSnapshot MakeSnapshot()
        {
            this.stopwatch.Restart();
            var result = this.service.MakeSnapshot();
            this.stopwatch.Stop();
            Log(this.stopwatch.ElapsedTicks, "MakeSnapshot");

            return result;
        }

        /// <inheritdoc/>
        public void Purge()
        {
            this.stopwatch.Restart();
            this.service.Purge();
            this.stopwatch.Stop();
            Log(this.stopwatch.ElapsedTicks, "Purge");
        }

        /// <inheritdoc/>
        public void RemoveRecord(int id)
        {
            this.stopwatch.Restart();
            this.service.RemoveRecord(id);
            this.stopwatch.Stop();
            Log(this.stopwatch.ElapsedTicks, "RemoveRecord");
        }

        /// <inheritdoc/>
        public void Restore(IFileCabinetServiceSnapshot snapshot)
        {
            this.stopwatch.Restart();
            this.service.Restore(snapshot);
            this.stopwatch.Stop();
            Log(this.stopwatch.ElapsedTicks, "Restore");
        }

        /// <summary>
        /// Loging elapsed ticks console.
        /// </summary>
        /// <param name="elapsed">A <see cref="long"/> instance.</param>
        /// <param name="methodName">A <see cref="string"/> instance.</param>
        private static void Log(long elapsed, string methodName)
        {
            Console.WriteLine("{0} method execution duration is {1} ticks.", methodName, elapsed);
        }
    }
}
