using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Default Salary Validator.
    /// </summary>
    public class DefaultSalaryValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public Tuple<bool, string> ValidateParameters(object parameters)
        {
            decimal value = (decimal)parameters;

            if (value < 0)
            {
                return Tuple.Create(false, "Salary cannot be less than zero.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
