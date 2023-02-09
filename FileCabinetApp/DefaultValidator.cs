using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Dafault Validator implementation.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public int FirstNameMinLength { get; } = 2;

        /// <inheritdoc/>
        public int FirstNameMaxLength { get; } = 60;

        /// <inheritdoc/>
        public int LastNameMinLength { get; } = 2;

        /// <inheritdoc/>
        public int LastNameMaxLength { get; } = 60;

        /// <inheritdoc/>
        public DateTime MinDate { get; } = new (1950, 1, 1);

        /// <inheritdoc/>
        public Tuple<bool, string> FirstNameValidator(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Tuple.Create(false, "First Name is not provided.");
            }

            value = value.Trim();
            if (value.Length < this.FirstNameMinLength || value.Length > this.FirstNameMaxLength)
            {
                return Tuple.Create(false, $"First name has to have at least {this.FirstNameMinLength} and maximum {this.FirstNameMaxLength} characters.");
            }

            return Tuple.Create(true, string.Empty);
        }

        /// <inheritdoc/>
        public Tuple<bool, string> LastNameValidator(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Tuple.Create(false, "Last Name is not provided.");
            }

            value = value.Trim();
            if (value.Length < this.LastNameMinLength || value.Length > this.LastNameMaxLength)
            {
                return Tuple.Create(false, $"Last name has to have at least {this.LastNameMinLength} and maximum {this.LastNameMaxLength} characters.");
            }

            return Tuple.Create(true, string.Empty);
        }

        /// <inheritdoc/>
        public Tuple<bool, string> DateOfBirthValidator(DateTime value)
        {
            if (DateTime.Compare(value, this.MinDate) < 0
                || DateTime.Compare(value, DateTime.Today) > 0)
            {
                return Tuple.Create(false, $"Date of birth should be within {this.MinDate.ToString("dd.MMM.yyyy", CultureInfo.InvariantCulture)} and today.");
            }

            return Tuple.Create(true, string.Empty);
        }

        /// <inheritdoc/>
        public Tuple<bool, string> WorkPlaceValidator(short value)
        {
            if (value < 0)
            {
                return Tuple.Create(false, "WorkPlaceNumber cannot be less than zero");
            }

            return Tuple.Create(true, string.Empty);
        }

        /// <inheritdoc/>
        public Tuple<bool, string> SalaryValidator(decimal value)
        {
            if (value < 0)
            {
                return Tuple.Create(false, "Salary cannot be less than zero.");
            }

            return Tuple.Create(true, string.Empty);
        }

        /// <inheritdoc/>
        public Tuple<bool, string> DepartmentValidator(char value)
        {
            if (!char.IsLetter(value))
            {
                return Tuple.Create(false, "Not an English letter.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
