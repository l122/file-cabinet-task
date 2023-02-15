using System;
using FileCabinetApp.Models;
using FileCabinetApp.Validators;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp.Extensions
{
    /// <summary>
    /// The class with extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Extension method for <see cref="ValidatorBuilder"/> that creates a default validator object.
        /// </summary>
        /// <param name="validator">A <see cref="ValidatorBuilder"/> instance object.</param>
        /// <returns>An <see cref="IRecordValidator"/> instance.</returns>
        public static IRecordValidator CreateDefaultValidator(this ValidatorBuilder validator)
        {
            const string defaultSection = "default";

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("validation-rules.json", true, true)
                .Build();

            var rules = configuration.GetSection(defaultSection).Get<ValidationRules>();

            if (rules != null)
            {
                validator = new ValidatorBuilder()
                    .ValidateFirstName(rules.FirstName.Min, rules.FirstName.Max)
                    .ValidateLastName(rules.LastName.Min, rules.LastName.Max)
                    .ValidateDateOfBirth(rules.DateOfBirth.From, rules.DateOfBirth.To)
                    .ValidateWorkplace(rules.Workplace.Min, rules.Workplace.Max)
                    .ValidateSalary(rules.Salary.Min, rules.Salary.Max)
                    .ValidateDepartment(rules.Department.Start, rules.Department.End);
            }

            return validator.Create();
        }

        /// <summary>
        /// Extension method for <see cref="ValidatorBuilder"/> that creates a custom validator object.
        /// </summary>
        /// <param name="validator">A <see cref="ValidatorBuilder"/> instance object.</param>
        /// <returns>An <see cref="IRecordValidator"/> instance.</returns>
        public static IRecordValidator CreateCustomValidator(this ValidatorBuilder validator)
        {
            const string customSection = "custom";

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("validation-rules.json", true, true)
                .Build();

            var rules = configuration.GetSection(customSection).Get<ValidationRules>();

            if (rules != null)
            {
                validator = new ValidatorBuilder()
                    .ValidateFirstName(rules.FirstName.Min, rules.FirstName.Max)
                    .ValidateLastName(rules.LastName.Min, rules.LastName.Max)
                    .ValidateDateOfBirth(rules.DateOfBirth.From, rules.DateOfBirth.To)
                    .ValidateWorkplace(rules.Workplace.Min, rules.Workplace.Max)
                    .ValidateSalary(rules.Salary.Min, rules.Salary.Max)
                    .ValidateDepartment(rules.Department.Start, rules.Department.End);
            }

            return validator.Create();
        }
    }
}
