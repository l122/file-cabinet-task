using System;

namespace FileCabinetApp
{
    /// <summary>
    /// The class with extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Extension method for <see cref="ValidatorBuilder"/> that creates a default validator object.
        /// </summary>
        /// <param name="validator">A <see cref="ValidatorBuilder"/> instance object.</param>
        /// <returns>An <see cref="IRecordValidator"/> instance.</returns>
        public static IRecordValidator CreateDefaultValidator(this ValidatorBuilder validator)
        {
            validator = new ValidatorBuilder()
                .ValidateFirstName(1, 30)
                .ValidateLastName(1, 30)
                .ValidateDateOfBirth(new DateTime(1900, 1, 1), DateTime.Today)
                .ValidateWorkplace(1, short.MaxValue)
                .ValidateSalary(0, decimal.MaxValue)
                .ValidateDepartment('A', 'Z');

            return validator.Create();
        }

        /// <summary>
        /// Extension method for <see cref="ValidatorBuilder"/> that creates a custom validator object.
        /// </summary>
        /// <param name="validator">A <see cref="ValidatorBuilder"/> instance object.</param>
        /// <returns>An <see cref="IRecordValidator"/> instance.</returns>
        public static IRecordValidator CreateCustomValidator(this ValidatorBuilder validator)
        {
            validator = new ValidatorBuilder()
                .ValidateFirstName(2, 60)
                .ValidateLastName(2, 60)
                .ValidateDateOfBirth(new DateTime(1950, 1, 1), DateTime.Today)
                .ValidateWorkplace(1, short.MaxValue)
                .ValidateSalary(0, decimal.MaxValue)
                .ValidateDepartment('A', 'Z');

            return validator.Create();
        }
    }
}
