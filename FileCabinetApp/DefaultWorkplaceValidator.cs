using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Default Work Place Validator.
    /// </summary>
    public class DefaultWorkplaceValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public Tuple<bool, string> ValidateParameters(object parameters)
        {
            short value = (short)parameters;

            if (value < 0)
            {
                return Tuple.Create(false, "WorkPlaceNumber cannot be less than zero");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
