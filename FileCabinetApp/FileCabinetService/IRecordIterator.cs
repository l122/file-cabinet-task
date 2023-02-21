namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// Iterator for FileCabinetService.
    /// </summary>
    public interface IRecordIterator
    {
        /// <summary>
        /// Gets next element.
        /// </summary>
        /// <returns>A nullable <see cref="FileCabinetRecord"/> instance.</returns>
        public FileCabinetRecord GetNext();

        /// <summary>
        /// Checks if next element is available.
        /// </summary>
        /// <returns>true if next element is available, false otherwise.</returns>
        public bool HasMore();

        /// <summary>
        /// Return record's position.
        /// </summary>
        /// <returns>A <see cref="long"/> instance.</returns>
        public long GetPosition();
    }
}
