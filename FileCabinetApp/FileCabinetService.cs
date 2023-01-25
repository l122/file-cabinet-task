using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private static readonly DateTime MinDate = new DateTime(1950, 1, 1);
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(
            string? firstName,
            string? lastName,
            string? dateOfBirthString,
            string? ageString,
            string? savingsString,
            string? letterString)
        {
            var record = this.ValidateData(firstName, lastName, dateOfBirthString, ageString, savingsString, letterString);

            this.list.Add(record);

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        public int GetStat()
        {
            return this.list.Count;
        }

        public void EditRecord(
            int id,
            string? firstName,
            string? lastName,
            string? dateOfBirthString,
            string? ageString,
            string? savingsString,
            string? letterString)
        {
            if (id < 1 || id > this.list.Count)
            {
                throw new ArgumentException("id is not found.", nameof(id));
            }

            var record = this.ValidateData(firstName, lastName, dateOfBirthString, ageString, savingsString, letterString);

            this.list[id - 1].FirstName = record.FirstName;
            this.list[id - 1].LastName = record.LastName;
            this.list[id - 1].DateOfBirth = record.DateOfBirth;
            this.list[id - 1].Age = record.Age;
            this.list[id - 1].Savings = record.Savings;
            this.list[id - 1].Letter = record.Letter;
        }

        private FileCabinetRecord ValidateData(
            string? firstName,
            string? lastName,
            string? dateOfBirthString,
            string? ageString,
            string? savingsString,
            string? letterString)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("First name cannot be empty or have only white spaces.", nameof(firstName));
            }

            firstName = firstName.Trim();
            if (firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException("First name has to have at least 2 and maximum 60 characters.", nameof(firstName));
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("Last name cannot be empty or have only white spaces.", nameof(lastName));
            }

            lastName = lastName.Trim();
            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException("Last name has to have at least 2 and maximum 60 characters.", nameof(lastName));
            }

            if (dateOfBirthString == null)
            {
                throw new ArgumentNullException(nameof(dateOfBirthString));
            }

            DateTime dateOfBirth;
            if (!DateTime.TryParse(dateOfBirthString, out dateOfBirth))
            {
                throw new ArgumentException("Date of birth is invalid.", nameof(dateOfBirthString));
            }

            if (DateTime.Compare(dateOfBirth, MinDate) < 0
                || DateTime.Compare(dateOfBirth, DateTime.Today) > 0)
            {
                throw new ArgumentException("Date of birth should be within 01-Jan-1950 and today.", nameof(dateOfBirthString));
            }

            if (ageString == null)
            {
                throw new ArgumentNullException(nameof(ageString));
            }

            short age;
            if (!short.TryParse(ageString, CultureInfo.InvariantCulture, out age))
            {
                throw new ArgumentException("Age is not a valid number.", nameof(ageString));
            }

            if (age < 0 || DateTime.Compare(dateOfBirth.AddYears(age), DateTime.Today) > 0)
            {
                throw new ArgumentException("Age cannot be less than zero or be passed today.", nameof(ageString));
            }

            if (savingsString == null)
            {
                throw new ArgumentNullException(nameof(savingsString));
            }

            decimal savings;
            if (!decimal.TryParse(savingsString, CultureInfo.InvariantCulture, out savings))
            {
                throw new ArgumentException("Savings is not a valid number.", nameof(savingsString));
            }

            if (savings < 0)
            {
                throw new ArgumentException("Saving cannot be less than zero.", nameof(savingsString));
            }

            if (letterString == null)
            {
                throw new ArgumentNullException(nameof(letterString));
            }

            if (letterString.Length != 1)
            {
                throw new ArgumentException("Letter should have one character", nameof(letterString));
            }

            char letter = letterString[0];
            if (!char.IsLetter(letter))
            {
                throw new ArgumentException("Not an English letter.", nameof(letterString));
            }

            return new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Age = age,
                Savings = savings,
                Letter = letter,
            };
        }
    }
}
