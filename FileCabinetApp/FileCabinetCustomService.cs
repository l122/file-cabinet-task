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
        /// Initializes a new instance of the <see cref="FileCabinetCustomService"/> class.
        /// </summary>
        public FileCabinetCustomService()
            : base(new CustomValidator())
        {
        }
    }
}
