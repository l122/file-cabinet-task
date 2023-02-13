using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Default First Name Validator.
    /// </summary>
    public class DefaultFirstNameValidator : IRecordValidator
    {
        private const int FirstNameMinLength = 2;
        private const int FirstNameMaxLength = 60;

        /// <inheritdoc/>
        public Tuple<bool, string> ValidateParameters(object parameters)
        {
            string value = (string)parameters;

            if (string.IsNullOrWhiteSpace(value))
            {
                return Tuple.Create(false, "First Name is not provided.");
            }

            value = value.Trim();
            if (value.Length < FirstNameMinLength || value.Length > FirstNameMaxLength)
            {
                return Tuple.Create(false, $"First name has to have at least {FirstNameMinLength} and maximum {FirstNameMaxLength} characters.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
