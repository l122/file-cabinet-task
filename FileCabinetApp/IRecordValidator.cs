using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Strategy interface for choosing a validator object.
    /// </summary>
    public interface IRecordValidator
    {
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