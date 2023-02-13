using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Dafault Validator implementation.
    /// </summary>
    public class DefaultValidator : CompositeValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValidator"/> class.
        /// </summary>
        public DefaultValidator()
            : base(new IRecordValidator[]
            {
                new FirstNameValidator(2, 60),
                new LastNameValidator(2, 60),
                new DateOfBirthValidator(new DateTime(1950, 1, 1), DateTime.Today),
                new WorkplaceValidator(1, short.MaxValue),
                new SalaryValidator(0, decimal.MaxValue),
                new DepartmentValidator('A', 'Z'),
            })
        {
        }
    }
}
