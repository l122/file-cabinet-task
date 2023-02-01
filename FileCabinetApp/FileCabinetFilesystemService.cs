using System;
using System.Collections.ObjectModel;
using System.IO;

namespace FileCabinetApp
{
    internal class FileCabinetFilesystemService : IFileCabinetService
    {
        private readonly FileStream fileStream;

        public FileCabinetFilesystemService(FileStream fileStream)
        {
            this.fileStream = fileStream;
        }

        int IFileCabinetService.CreateRecord()
        {
            throw new NotImplementedException();
        }

        void IFileCabinetService.EditRecord(int id)
        {
            throw new NotImplementedException();
        }

        ReadOnlyCollection<FileCabinetRecord> IFileCabinetService.FindByDateOfBirth(string dateOfBirthString)
        {
            throw new NotImplementedException();
        }

        ReadOnlyCollection<FileCabinetRecord> IFileCabinetService.FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        ReadOnlyCollection<FileCabinetRecord> IFileCabinetService.FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        ReadOnlyCollection<FileCabinetRecord> IFileCabinetService.GetRecords()
        {
            throw new NotImplementedException();
        }

        int IFileCabinetService.GetStat()
        {
            throw new NotImplementedException();
        }

        IFileCabinetServiceSnapshot IFileCabinetService.MakeSnapshot()
        {
            throw new NotImplementedException();
        }
    }
}
