using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Default Last Name Validator.
    /// </summary>
    public class DefaultLastNameValidator : IRecordValidator
    {
        private const int LastNameMinLength = 2;
        private const int LastNameMaxLength = 60;

        /// <inheritdoc/>
        public Tuple<bool, string> ValidateParameters(object parameters)
        {
            string value = (string)parameters;

            if (string.IsNullOrWhiteSpace(value))
            {
                return Tuple.Create(false, "Last Name is not provided.");
            }

            value = value.Trim();
            if (value.Length < LastNameMinLength || value.Length > LastNameMaxLength)
            {
                return Tuple.Create(false, $"Last name has to have at least {LastNameMinLength}  and maximum  {LastNameMaxLength} characters.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
