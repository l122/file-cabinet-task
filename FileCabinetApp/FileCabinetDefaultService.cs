using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Helper class with default parameters.
    /// Inherits <see cref="FileCabinetService"/>.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        /// <summary>
        /// Creates an object of IRecordValidator with default strategy.
        /// </summary>
        /// <returns><see cref="DefaultValidator"/> instance.</returns>
        public override IRecordValidator CreateValidator()
        {
            return new DefaultValidator();
        }
    }
}
