using System;
using System.Globalization;
using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Default Date Of Birth Validator.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        private const string DateMask = "dd.MMM.yyyy";
        private readonly DateTime fromDate;
        private readonly DateTime toDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="fromDate">A <see cref="DateTime"/> instance of the starting Date of Birth.</param>
        /// <param name="toDate">A <see cref="DateTime"/> instance of the ending Date of Birth.</param>
        public DateOfBirthValidator(DateTime fromDate, DateTime toDate)
        {
            this.fromDate = fromDate;
            this.toDate = toDate;
        }

        /// <inheritdoc/>
        public Tuple<bool, string> ValidateParameters(FileCabinetRecord record)
        {
            DateTime value = record.DateOfBirth;

            if (DateTime.Compare(value, this.fromDate) < 01
                || DateTime.Compare(value, this.toDate) > 0)
            {
                return Tuple.Create(
                    false,
                    new string($"Date of birth should be within {this.fromDate.ToString(DateMask, CultureInfo.InvariantCulture)}"
                            + $" and {this.toDate.ToString(DateMask, CultureInfo.InvariantCulture)}."));
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
