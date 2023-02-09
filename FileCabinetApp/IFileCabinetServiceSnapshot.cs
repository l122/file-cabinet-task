using System.Collections.ObjectModel;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// File Cabinet Service Snapshot interface.
    /// </summary>
    public interface IFileCabinetServiceSnapshot
    {
        /// <summary>
        /// Gets records.
        /// </summary>
        /// <value>Records.</value>
        public ReadOnlyCollection<FileCabinetRecord> Records { get; }

        /// <summary>
        /// Saves a snapshot to a csv file.
        /// </summary>
        /// <param name="sw">A <see cref="StreamWriter"/> instance.</param>
        public void SaveToCsv(StreamWriter sw);

        /// <summary>
        /// Saves a snapshot to an xml file.
        /// </summary>
        /// <param name="sw">A <see cref="StreamWriter"/> instance.</param>
        public void SaveToXml(StreamWriter sw);

        /// <summary>
        /// Loads data from csv.
        /// </summary>
        /// <param name="fileStream">A <see cref="FileStream"/> instance.</param>
        public void LoadFromCsv(FileStream fileStream);

        /// <summary>
        /// Loads data from csv.
        /// </summary>
        /// <param name="fileStream">A <see cref="FileStream"/> instance.</param>
        public void LoadFromXml(FileStream fileStream);
    }
}