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
        /// Initializes a new instance of the <see cref="FileCabinetDefaultService"/> class.
        /// </summary>
        public FileCabinetDefaultService()
            : base(new DefaultValidator())
        {
        }
    }
}
