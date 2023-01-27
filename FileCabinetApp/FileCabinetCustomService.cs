using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Helper class with default parameters.
    /// Inherits <see cref="FileCabinetService"/>.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>
        /// Creates an object of IRecordValidator with customv strategy.
        /// </summary>
        /// <returns><see cref="CustomValidator"/> instance.</returns>
        public override IRecordValidator CreateValidator()
        {
            return new CustomValidator();
        }
    }
}
