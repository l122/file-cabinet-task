using System;
using System.Collections.Generic;
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
        public int CreateRecord(FileCabinetRecord record)
        {
            this.stopwatch.Restart();
            var result = this.service.CreateRecord(record);
            this.stopwatch.Stop();
            Log(this.stopwatch.ElapsedTicks, "Create");

            return result;
        }

        /// <inheritdoc/>
        public bool Insert(FileCabinetRecord record)
        {
            this.stopwatch.Restart();
            var result = this.service.Insert(record);
            this.stopwatch.Stop();
            Log(this.stopwatch.ElapsedTicks, "Insert");
            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> SelectRecords(string expression)
        {
            this.stopwatch.Restart();
            var result = this.service.SelectRecords(expression);
            this.stopwatch.Stop();
            Log(this.stopwatch.ElapsedTicks, "SelectRecords");

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
        public (int, int) Purge()
        {
            this.stopwatch.Restart();
            var result = this.service.Purge();
            this.stopwatch.Stop();
            Log(this.stopwatch.ElapsedTicks, "Purge");
            return result;
        }

        /// <inheritdoc/>
        public int Restore(IFileCabinetServiceSnapshot snapshot)
        {
            this.stopwatch.Restart();
            var result = this.service.Restore(snapshot);
            this.stopwatch.Stop();
            Log(this.stopwatch.ElapsedTicks, "Restore");
            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindById(int id)
        {
            return this.service.FindById(id);
        }

        /// <inheritdoc/>
        public string Delete(string expression)
        {
            this.stopwatch.Restart();
            var result = this.service.Delete(expression);
            this.stopwatch.Stop();
            Log(this.stopwatch.ElapsedTicks, "Delete");
            return result;
        }

        /// <inheritdoc/>
        public string Update(string expression)
        {
            this.stopwatch.Restart();
            var result = this.service.Update(expression);
            this.stopwatch.Stop();
            Log(this.stopwatch.ElapsedTicks, "Update");
            return result;
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
