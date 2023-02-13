using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Custom Validator implementation.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public Tuple<bool, string> ValidateParameters(object parameters)
        {
            new FirstNameValidator(1, 30).ValidateParameters(parameters);
            new LastNameValidator(1, 30).ValidateParameters(parameters);
            new DateOfBirthValidator(new DateTime(1900, 1, 1), DateTime.Today).ValidateParameters(parameters);
            new WorkplaceValidator(1, short.MaxValue).ValidateParameters(parameters);
            new SalaryValidator(0, decimal.MaxValue).ValidateParameters(parameters);
            new DepartmentValidator('A', 'Z').ValidateParameters(parameters);
        }
    }
}
