using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.FileCabinetService
{
    public class FilesystemEnumerator : IEnumerator<FileCabinetRecord>
    {
        private readonly FileStream fileStream;

        public FilesystemEnumerator(FileStream fileStream)
        {
            this.fileStream = fileStream;
        }

        public FileCabinetRecord Current => throw new NotImplementedException();

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
