using System;

namespace FileCabinetApp
{
    public class FileCabinetRecord
    {
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public short Age { get; set; }

        public decimal Savings { get; set; }

        public char Letter { get; set; }
    }
}
