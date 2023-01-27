using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Dafault Validator implementation.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        private static readonly DateTime MinDate = new (1950, 1, 1);

        /// <summary>
        /// Validates First Name.
        /// </summary>
        /// <param name="value">The <see cref="string"/> instance value.</param>
        /// <returns>True, if valudation is successful, false otherwise, and a validation result.</returns>
        public Tuple<bool, string> FirstNameValidator(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Tuple.Create(false, "First Name is not provided.");
            }

            value = value.Trim();
            if (value.Length < 2 || value.Length > 60)
            {
                return Tuple.Create(false, "First name has to have at least 2 and maximum 60 characters.");
            }

            return Tuple.Create(true, string.Empty);
        }

        /// <summary>
        /// Validates Last Name.
        /// </summary>
        /// <param name="value">The <see cref="string"/> instance value.</param>
        /// <returns>True, if valudation is successful, false otherwise, and a validation result.</returns>
        public Tuple<bool, string> LastNameValidator(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Tuple.Create(false, "Last Name is not provided.");
            }

            value = value.Trim();
            if (value.Length < 2 || value.Length > 60)
            {
                return Tuple.Create(false, "Last name has to have at least 2 and maximum 60 characters.");
            }

            return Tuple.Create(true, string.Empty);
        }

        /// <summary>
        /// Validates date of birth.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> instance value.</param>
        /// <returns>True, if valudation is successful, false otherwise, and a validation result.</returns>
        public Tuple<bool, string> DateOfBirthValidator(DateTime value)
        {
            if (DateTime.Compare(value, MinDate) < 0
                || DateTime.Compare(value, DateTime.Today) > 0)
            {
                return Tuple.Create(false, "Date of birth should be within 01-Jan-1950 and today.");
            }

            return Tuple.Create(true, string.Empty);
        }

        /// <summary>
        /// Validates place of work.
        /// </summary>
        /// <param name="value">The <see cref="short"/> instance value.</param>
        /// <returns>True, if valudation is successful, false otherwise, and a validation result.</returns>
        public Tuple<bool, string> WorkPlaceValidator(short value)
        {
            if (value < 0)
            {
                return Tuple.Create(false, "WorkPlaceNumber cannot be less than zero");
            }

            return Tuple.Create(true, string.Empty);
        }

        /// <summary>
        /// Validates salary.
        /// </summary>
        /// <param name="value">The <see cref="decimal"/> instance value.</param>
        /// <returns>True, if valudation is successful, false otherwise, and a validation result.</returns>
        public Tuple<bool, string> SalaryValidator(decimal value)
        {
            if (value < 0)
            {
                return Tuple.Create(false, "Salary cannot be less than zero.");
            }

            return Tuple.Create(true, string.Empty);
        }

        /// <summary>
        /// Validates department letter.
        /// </summary>
        /// <param name="value">The <see cref="char"/> instance value.</param>
        /// <returns>True, if valudation is successful, false otherwise, and a validation result.</returns>
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
