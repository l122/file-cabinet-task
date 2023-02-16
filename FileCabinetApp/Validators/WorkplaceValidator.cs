using System;
using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Default Work Place Validator.
    /// </summary>
    public class WorkplaceValidator : IRecordValidator
    {
        private readonly short fromNumber;
        private readonly short toNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkplaceValidator"/> class.
        /// </summary>
        /// <param name="fromNumber">A <see cref="short"/> instance of the from number parameter.</param>
        /// <param name="toNumber">A <see cref="short"/> instance of the to number parameter.</param>
        public WorkplaceValidator(short fromNumber, short toNumber)
        {
            this.fromNumber = fromNumber;
            this.toNumber = toNumber;
        }

        /// <inheritdoc/>
        public Tuple<bool, string> ValidateParameters(FileCabinetRecord record)
        {
            short value = record.WorkPlaceNumber;

            if (value < this.fromNumber || value > this.toNumber)
            {
                return Tuple.Create(false, $"WorkPlaceNumber cannot be less than {this.fromNumber} and greater than {this.toNumber}.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
