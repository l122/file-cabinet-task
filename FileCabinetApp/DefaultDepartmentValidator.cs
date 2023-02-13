using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Default Department Validator.
    /// </summary>
    public class DefaultDepartmentValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public Tuple<bool, string> ValidateParameters(object parameters)
        {
            char value = (char)parameters;

            if (!char.IsLetter(value))
            {
                return Tuple.Create(false, "Not an English letter.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
