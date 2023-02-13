using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Default Salary Validator.
    /// </summary>
    public class SalaryValidator : IRecordValidator
    {
        private readonly decimal min;
        private readonly decimal max;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryValidator"/> class.
        /// </summary>
        /// <param name="minSalary">A <see cref="decimal"/> instance of the minimal value.</param>
        /// <param name="maxSalary">A <see cref="decimal"/> instance of the maximal value.</param>
        public SalaryValidator(decimal minSalary, decimal maxSalary)
        {
            this.min = minSalary;
            this.max = maxSalary;
        }

        /// <inheritdoc/>
        public Tuple<bool, string> ValidateParameters(FileCabinetRecord record)
        {
            decimal value = record.Salary;

            if (value < this.min || value > this.max)
            {
                return Tuple.Create(false, $"Salary cannot be less than {this.min} and greater than {this.max}.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
