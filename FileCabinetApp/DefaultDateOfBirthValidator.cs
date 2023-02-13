using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Default Date Of Birth Validator.
    /// </summary>
    public class DefaultDateOfBirthValidator : IRecordValidator
    {
        private readonly DateTime minDate = new (1950, 1, 1);

        /// <inheritdoc/>
        public Tuple<bool, string> ValidateParameters(object parameters)
        {
            DateTime value = (DateTime)parameters;

            if (DateTime.Compare(value, this.minDate) < 01
                || DateTime.Compare(value, DateTime.Today) > 0)
            {
                return Tuple.Create(false, $"Date of birth should be within {this.minDate.ToString("dd.MMM.yyyy", CultureInfo.InvariantCulture)} and today.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
