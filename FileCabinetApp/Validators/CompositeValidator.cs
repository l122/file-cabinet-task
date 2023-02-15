using System;
using System.Collections.Generic;
using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// The Composite Validator Class.
    /// </summary>
    public class CompositeValidator : IRecordValidator
    {
        private readonly List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">A <see cref="IEnumerable{T}"/> of validator instances.</param>
        public CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = new List<IRecordValidator>(validators);
        }

        /// <inheritdoc/>
        public Tuple<bool, string> ValidateParameters(FileCabinetRecord record)
        {
            foreach (var validator in this.validators)
            {
                var validationResult = validator.ValidateParameters(record);
                if (!validationResult.Item1)
                {
                    return validationResult;
                }
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
