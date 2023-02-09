using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Strategy interface for choosing a validator object.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Gets the minimum length of the First Name.
        /// </summary>
        /// /// <value>
        /// The minimum length of the First Name.
        /// </value>
        public int FirstNameMinLength { get; }

        /// <summary>
        /// Gets the maximum length of the First Name.
        /// </summary>
        /// /// <value>
        /// The maximum length of the First Name.
        /// </value>
        public int FirstNameMaxLength { get; }

        /// <summary>
        /// Gets the minimul length of the Last Name.
        /// </summary>
        /// /// <value>
        /// The minimul length of the Last Name.
        /// </value>
        public int LastNameMinLength { get; }

        /// <summary>
        /// Gets the maximum length of the Last Name.
        /// </summary>
        /// <value>
        /// The maximum length of the Last Name.
        /// </value>
        public int LastNameMaxLength { get; }

        /// <summary>
        /// Gets the earliest date of birth.
        /// </summary>
        /// <value>
        /// The earliest date of birth.
        /// </value>
        public DateTime MinDate { get; }

        /// <summary>
        /// Validates First Name.
        /// </summary>
        /// <param name="value">The <see cref="string"/> instance value.</param>
        /// <returns>True, if valudation is successful, false otherwise, and a validation result.</returns>
        public Tuple<bool, string> FirstNameValidator(string value);

        /// <summary>
        /// Validates Last Name.
        /// </summary>
        /// <param name="value">The <see cref="string"/> instance value.</param>
        /// <returns>True, if valudation is successful, false otherwise, and a validation result.</returns>
        public Tuple<bool, string> LastNameValidator(string value);

        /// <summary>
        /// Validates date of birth.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> instance value.</param>
        /// <returns>True, if valudation is successful, false otherwise, and a validation result.</returns>
        public Tuple<bool, string> DateOfBirthValidator(DateTime value);

        /// <summary>
        /// Validates place of work.
        /// </summary>
        /// <param name="value">The <see cref="short"/> instance value.</param>
        /// <returns>True, if valudation is successful, false otherwise, and a validation result.</returns>
        public Tuple<bool, string> WorkPlaceValidator(short value);

        /// <summary>
        /// Validates salary.
        /// </summary>
        /// <param name="value">The <see cref="decimal"/> instance value.</param>
        /// <returns>True, if valudation is successful, false otherwise, and a validation result.</returns>
        public Tuple<bool, string> SalaryValidator(decimal value);

        /// <summary>
        /// Validates department letter.
        /// </summary>
        /// <param name="value">The <see cref="char"/> instance value.</param>
        /// <returns>True, if valudation is successful, false otherwise, and a validation result.</returns>
        public Tuple<bool, string> DepartmentValidator(char value);
    }
}