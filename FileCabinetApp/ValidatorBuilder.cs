using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// The Validator Builder Class.
    /// </summary>
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> validators = new ();

        /// <summary>
        /// Validates First Name.
        /// </summary>
        /// <param name="min">An <see cref="int"/> instance of minimum length.</param>
        /// <param name="max">An <see cref="int"/> instance of maximum length.</param>
        /// <returns>The <see cref="ValidatorBuilder"/> instance.</returns>
        public ValidatorBuilder ValidateFirstName(int min, int max)
        {
            this.validators.Add(new FirstNameValidator(2, 60));
            return this;
        }

        /// <summary>
        /// Validate Last Name.
        /// </summary>
        /// <param name="min">An <see cref="int"/> instance of minimum length.</param>
        /// <param name="max">An <see cref="int"/> instance of maximum length.</param>
        /// <returns>The <see cref="ValidatorBuilder"/> instance.</returns>
        public ValidatorBuilder ValidateLastName(int min, int max)
        {
            this.validators.Add(new LastNameValidator(2, 60));
            return this;
        }

        /// <summary>
        /// Validate Date of birth.
        /// </summary>
        /// <param name="from">A <see cref="DateTime"/> instance of starting date.</param>
        /// <param name="to">A <see cref="DateTime"/> instance of ending date.</param>
        /// <returns>The <see cref="ValidatorBuilder"/> instance.</returns>
        public ValidatorBuilder ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
            return this;
        }

        /// <summary>
        /// Validates Workplace.
        /// </summary>
        /// <param name="from">A <see cref="short"/> instance of minimum number.</param>
        /// <param name="to">A <see cref="short"/> instance of maximum number.</param>
        /// <returns>The <see cref="ValidatorBuilder"/> instance.</returns>
        public ValidatorBuilder ValidateWorkplace(short from, short to)
        {
            this.validators.Add(new WorkplaceValidator(from, to));
            return this;
        }

        /// <summary>
        /// Validates Salary.
        /// </summary>
        /// <param name="min">A <see cref="decimal"/> instance of minimal salary.</param>
        /// <param name="max">A <see cref="decimal"/> instance of maximal salary.</param>
        /// <returns>The <see cref="ValidatorBuilder"/> instance.</returns>
        public ValidatorBuilder ValidateSalary(decimal min, decimal max)
        {
            this.validators.Add(new SalaryValidator(min, max));
            return this;
        }

        /// <summary>
        /// Validates Department.
        /// </summary>
        /// <param name="from">A <see cref="char"/> instance of starting letter.</param>
        /// <param name="to">A <see cref="char"/> instance of ending letter.</param>
        /// <returns>The <see cref="ValidatorBuilder"/> instance.</returns>
        public ValidatorBuilder ValidateDepartment(char from, char to)
        {
            this.validators.Add(new DepartmentValidator(from, to));
            return this;
        }

        /// <summary>
        /// Creates a composite validator.
        /// </summary>
        /// <returns>A <see cref="CompositeValidator"/> instance.</returns>
        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
