using System;
using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Strategy interface for choosing a validator object.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validates a record.
        /// </summary>
        /// <param name="record">A <see cref="FileCabinetRecord"/> instance.</param>
        /// <returns>(true, message) if record is valid and (false, message) otherwise.</returns>
        public Tuple<bool, string> ValidateParameters(FileCabinetRecord record);
    }
}