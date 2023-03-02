using System;
using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Id Validator.
    /// </summary>
    public class IdentificationValidator : IRecordValidator
    {
        private readonly int identificationMin;
        private readonly int identificationMax;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentificationValidator"/> class.
        /// </summary>
        /// <param name="identificationMin">A <see cref="string"/> instance of minimum id value.</param>
        /// <param name="identificationMax">A <see cref="string"/> instance of maximum id value.</param>
        public IdentificationValidator(int identificationMin, int identificationMax)
        {
            this.identificationMin = identificationMin;
            this.identificationMax = identificationMax;
        }

        /// <inheritdoc/>
        public Tuple<bool, string> ValidateParameters(FileCabinetRecord record)
        {
            var value = record.Id;

            if (value < this.identificationMin || value > this.identificationMax)
            {
                return Tuple.Create(false, $"Record id has to be a number between {this.identificationMin} and {this.identificationMax}.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}