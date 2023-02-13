using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Default Last Name Validator.
    /// </summary>
    public class LastNameValidator : IRecordValidator
    {
        private readonly int lastNameMinLength;
        private readonly int lastNameMaxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        /// <param name="lastNameMinLength">A <see cref="string"/> instance of the Last Name minimum length.</param>
        /// <param name="lastNameMaxLength">A <see cref="string"/> instance of the Last Name maximum lenght.</param>
        public LastNameValidator(int lastNameMinLength, int lastNameMaxLength)
        {
            this.lastNameMinLength = lastNameMinLength;
            this.lastNameMaxLength = lastNameMaxLength;
        }

        /// <inheritdoc/>
        public Tuple<bool, string> ValidateParameters(object parameters)
        {
            string value = (string)parameters;

            if (string.IsNullOrWhiteSpace(value))
            {
                return Tuple.Create(false, "Last Name is not provided.");
            }

            value = value.Trim();
            if (value.Length < this.lastNameMinLength || value.Length > this.lastNameMaxLength)
            {
                return Tuple.Create(false, $"Last name has to have at least {this.lastNameMinLength}  and maximum  {this.lastNameMaxLength} characters.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
