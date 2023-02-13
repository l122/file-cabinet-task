using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Dafault Validator implementation.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public Tuple<bool, string> ValidateParameters(object value)
        {
            new DefaultFirstNameValidator().ValidateParameters(value);
            new DefaultLastNameValidator().ValidateParameters(value);
            new DefaultDateOfBirthValidator().ValidateParameters(value);
            new DefaultWorkplaceValidator().ValidateParameters(value);
            new DefaultSalaryValidator().ValidateParameters(value);
            new DefaultDepartmentValidator().ValidateParameters(value);
        }
    }
}
