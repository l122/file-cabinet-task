﻿using System;

namespace FileCabinetApp
{
    /// <summary>
    /// First Name Validator.
    /// </summary>
    public class FirstNameValidator : IRecordValidator
    {
        private readonly int firstNameMinLength;
        private readonly int firstNameMaxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="firstNameMinLength">A <see cref="string"/> instance of First Name minimum length.</param>
        /// <param name="firstNameMaxLength">A <see cref="string"/> instance of First Name maximum length.</param>
        public FirstNameValidator(int firstNameMinLength, int firstNameMaxLength)
        {
            this.firstNameMinLength = firstNameMinLength;
            this.firstNameMaxLength = firstNameMaxLength;
        }

        /// <inheritdoc/>
        public Tuple<bool, string> ValidateParameters(object parameters)
        {
            string value = (string)parameters;

            if (string.IsNullOrWhiteSpace(value))
            {
                return Tuple.Create(false, "First Name is not provided.");
            }

            value = value.Trim();
            if (value.Length < this.firstNameMinLength || value.Length > this.firstNameMaxLength)
            {
                return Tuple.Create(false, $"First name has to have at least {this.firstNameMinLength} and maximum {this.firstNameMaxLength} characters.");
            }

            return Tuple.Create(true, string.Empty);
        }
    }
}
