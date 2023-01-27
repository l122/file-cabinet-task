using System;
using System.Globalization;
using static FileCabinetApp.FileCabinetService;

namespace FileCabinetApp
{
    /// <summary>
    /// Dafault Validator implementation.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        private static readonly DateTime MinDate = new (1950, 1, 1);

        /// <summary>
        /// Validates input data with default rules and returns a new record instance.
        /// </summary>
        /// <param name="data">The <see cref="RecordParameters"/> instance input data.</param>
        /// <returns>A new record instance of type <see cref="FileCabinetRecord"/>.</returns>
        /// <exception cref="ArgumentException">If arguments are invalid.</exception>
        /// <exception cref="ArgumentNullException">If arguments have <c>null</c> values.</exception>
        public FileCabinetRecord ValidateParameters(RecordParameters data)
        {
            if (string.IsNullOrWhiteSpace(data.FirstName))
            {
                throw new ArgumentException("First name cannot be empty or have only white spaces.", nameof(data));
            }

            data.FirstName = data.FirstName.Trim();
            if (data.FirstName.Length < 2 || data.FirstName.Length > 60)
            {
                throw new ArgumentException("First name has to have at least 2 and maximum 60 characters.", nameof(data));
            }

            if (string.IsNullOrWhiteSpace(data.LastName))
            {
                throw new ArgumentException("Last name cannot be empty or have only white spaces.", nameof(data));
            }

            data.LastName = data.LastName.Trim();
            if (data.LastName.Length < 2 || data.LastName.Length > 60)
            {
                throw new ArgumentException("Last name has to have at least 2 and maximum 60 characters.", nameof(data));
            }

            if (data.DateOfBirth == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!DateTime.TryParse(data.DateOfBirth, out DateTime dateOfBirth))
            {
                throw new ArgumentException("Date of birth is invalid.", nameof(data));
            }

            if (DateTime.Compare(dateOfBirth, MinDate) < 0
                || DateTime.Compare(dateOfBirth, DateTime.Today) > 0)
            {
                throw new ArgumentException("Date of birth should be within 01-Jan-1950 and today.", nameof(data));
            }

            if (data.WorkPlaceNumber == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!short.TryParse(data.WorkPlaceNumber, CultureInfo.InvariantCulture, out short workPlaceNumber))
            {
                throw new ArgumentException("WorkPlaceNumber is not a valid number.", nameof(data));
            }

            if (workPlaceNumber < 0 || DateTime.Compare(dateOfBirth.AddYears(workPlaceNumber), DateTime.Today) > 0)
            {
                throw new ArgumentException("WorkPlaceNumber cannot be less than zero or be passed today.", nameof(data));
            }

            if (data.Salary == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!decimal.TryParse(data.Salary, CultureInfo.InvariantCulture, out decimal salary))
            {
                throw new ArgumentException("salary is not a valid number.", nameof(data));
            }

            if (salary < 0)
            {
                throw new ArgumentException("Saving cannot be less than zero.", nameof(data));
            }

            if (data.Department == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Department.Length != 1)
            {
                throw new ArgumentException("Department should have one character", nameof(data));
            }

            char department = data.Department[0];
            if (!char.IsLetter(department))
            {
                throw new ArgumentException("Not an English letter.", nameof(data));
            }

            return new FileCabinetRecord
            {
                Id = 0,
                FirstName = data.FirstName,
                LastName = data.LastName,
                DateOfBirth = dateOfBirth,
                WorkPlaceNumber = workPlaceNumber,
                Salary = salary,
                Department = department,
            };
        }
    }
}
