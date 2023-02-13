using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Custom Validator implementation.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        private const int FirstNameMinLength = 1;
        private const int FirstNameMaxLength = 30;
        private const int LastNameMinLength = 1;
        private const int LastNameMaxLength = 30;
        private readonly DateTime minDate = new (1900, 1, 1);

        /// <inheritdoc/>
        public Tuple<bool, string> ValidateParameters(object value)
        {
            this.ValidateFirstName((string)value);
            this.ValidateLastName((string)value);
            this.ValidateDateOfBirth((DateTime)value);
            this.ValidateWorkPlace((short)value);
            this.ValidateSalary((decimal)value);
            this.ValidateDepartment((char)value);
        }

        private Tuple<bool, string> ValidateFirstName(string value)
        {
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

        private Tuple<bool, string> ValidateLastName(string value)
        {
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

        private Tuple<bool, string> ValidateDateOfBirth(DateTime value)
        {
            if (DateTime.Compare(value, this.minDate) < 0
                || DateTime.Compare(value, DateTime.Today) > 0)
            {
                return Tuple.Create(false, $"Date of birth should be within {this.minDate.ToString("dd.MMM.yyyy", CultureInfo.InvariantCulture)} and today.");
            }

            return Tuple.Create(true, string.Empty);
        }

        private Tuple<bool, string> ValidateWorkPlace(short value)
        {
            if (value < 0)
            {
                return Tuple.Create(false, "WorkPlaceNumber cannot be less than zero");
            }

            return Tuple.Create(true, string.Empty);
        }

        private Tuple<bool, string> ValidateSalary(decimal value)
        {
            if (value < 0)
            {
                return Tuple.Create(false, "Salary cannot be less than zero.");
            }

            return Tuple.Create(true, string.Empty);
        }

        private Tuple<bool, string> ValidateDepartment(char value)
        {
            if (!char.IsLetter(value))
            {
                return Tuple.Create(false, "Not an English letter.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
