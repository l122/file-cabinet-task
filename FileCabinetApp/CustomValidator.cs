using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Custom Validator implementation.
    /// </summary>
    public class CustomValidator : CompositeValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomValidator"/> class.
        /// </summary>
        public CustomValidator()
            : base(new IRecordValidator[]
            {
                new FirstNameValidator(1, 30),
                new LastNameValidator(1, 30),
                new DateOfBirthValidator(new DateTime(1900, 1, 1), DateTime.Today),
                new WorkplaceValidator(1, short.MaxValue),
                new SalaryValidator(0, decimal.MaxValue),
                new DepartmentValidator('A', 'Z'),
            })
        {
        }
    }
}
