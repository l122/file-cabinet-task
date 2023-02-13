using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// The Composite Validator Class.
    /// </summary>
    public abstract class CompositeValidator : IRecordValidator
    {
        private readonly List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">A <see cref="IEnumerable{T}"/> of validator instances.</param>
        protected CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = new List<IRecordValidator>(validators);
        }

        /// <inheritdoc/>
        public Tuple<bool, string> ValidateParameters(object parameters)
        {
            foreach (var validator in this.validators)
            {
                validator.ValidateParameters(parameters);
            }
        }
    }
}
