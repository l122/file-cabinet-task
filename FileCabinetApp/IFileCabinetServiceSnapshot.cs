using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// File Cabinet Service Snapshot interface.
    /// </summary>
    public interface IFileCabinetServiceSnapshot
    {
        /// <summary>
        /// Saves a snapshot to a csv file.
        /// </summary>
        /// <param name="sw">A <see cref="StreamWriter"/> instance.</param>
        public void SaveToCsv(StreamWriter sw);
    }
}