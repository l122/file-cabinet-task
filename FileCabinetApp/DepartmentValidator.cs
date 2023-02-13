using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Default Department Validator.
    /// </summary>
    public class DepartmentValidator : IRecordValidator
    {
        private readonly char start;
        private readonly char end;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentValidator"/> class.
        /// </summary>
        /// <param name="start">A <see cref="char"/> instance of the starting letter.</param>
        /// <param name="end">A <see cref="char"/> instance of the ending letter.</param>
        public DepartmentValidator(char start, char end)
        {
            this.start = start;
            this.end = end;
        }

        /// <inheritdoc/>
        public Tuple<bool, string> ValidateParameters(object parameters)
        {
            char value = (char)parameters;

            if (!char.IsLetter(value))
            {
                return Tuple.Create(false, "Not an English letter.");
            }

            if ((int)value < (int)this.start && (int)value > (int)this.end)
            {
                return Tuple.Create(false, $"The letter should be within {this.start} and {this.end}.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
