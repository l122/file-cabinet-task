using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth)
        {
            // TODO: добавьте реализацию метода
            return 0;
        }

        public FileCabinetRecord[] GetRecords()
        {
            // TODO: добавьте реализацию метода
            return Array.Empty<FileCabinetRecord>();
        }

        public int GetStat()
        {
            return this.list.Count;
        }

    }
}
