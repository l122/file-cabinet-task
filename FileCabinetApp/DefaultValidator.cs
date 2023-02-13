using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Dafault Validator implementation.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public Tuple<bool, string> ValidateParameters(object parameters)
        {
            new FirstNameValidator(2, 60).ValidateParameters(parameters);
            new LastNameValidator(2, 60).ValidateParameters(parameters);
            new DateOfBirthValidator(new DateTime(1950, 1, 1), DateTime.Today).ValidateParameters(parameters);
            new WorkplaceValidator(1, short.MaxValue).ValidateParameters(parameters);
            new SalaryValidator(0, decimal.MaxValue).ValidateParameters(parameters);
            new DepartmentValidator('A', 'Z').ValidateParameters(parameters);
        }
    }
}
