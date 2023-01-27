using System;
using static FileCabinetApp.FileCabinetService;

namespace FileCabinetApp
{
    /// <summary>
    /// Strategy interface for choosing a validator object.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validates input data and returns a new record instance.
        /// </summary>
        /// <param name="data">The <see cref="IFileCabinetService.RecordParameters"/> instance input data.</param>
        /// <returns>A new record instance of type <see cref="FileCabinetRecord"/> with id = 0.</returns>
        /// <exception cref="ArgumentException">If arguments are invalid.</exception>
        /// <exception cref="ArgumentNullException">If arguments have <c>null</c> values.</exception>
        public FileCabinetRecord ValidateParameters(IFileCabinetService.RecordParameters data);
    }
}
