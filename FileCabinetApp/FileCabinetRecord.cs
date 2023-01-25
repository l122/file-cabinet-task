using System;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetRecord
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public short Age { get; set; }

        public decimal Savings { get; set; }

        public char Letter { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(CultureInfo.InvariantCulture, $"{this.Id}, ");
            builder.Append(CultureInfo.InvariantCulture, $"{this.FirstName}, ");
            builder.Append(CultureInfo.InvariantCulture, $"{this.LastName}, ");
            builder.Append(CultureInfo.InvariantCulture, $"{this.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, ");
            builder.Append(CultureInfo.InvariantCulture, $"{this.Age}, ");
            builder.Append(CultureInfo.InvariantCulture, $"{this.Savings}, ");
            builder.Append(CultureInfo.InvariantCulture, $"{this.Letter}");
            return builder.ToString();
        }
    }
}
