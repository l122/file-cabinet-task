using System;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// Class that represents a record.
    /// </summary>
    [XmlType("record")]
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets record id.
        /// </summary>
        /// /// <value>Employee's id.</value>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets employee's first name.
        /// </summary>
        /// <value>Employee's first name.</value>
        [XmlElement("firstName")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets employee's last name.
        /// </summary>
        /// <value>Employee's last name.</value>
        [XmlElement("lastName")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets employee's date of birth.
        /// </summary>
        /// <value>Employee's date of birth.</value>
        [XmlElement(
            ElementName = "dateOfBirth",
            DataType = "date")]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets employee's work place number.
        /// </summary>
        /// <value>Employee's work place number.</value>
        [XmlElement(ElementName = "workPlace")]
        public short WorkPlaceNumber { get; set; }

        /// <summary>
        /// Gets or sets employee's salary.
        /// </summary>
        /// <value>Employee's salary.</value>
        [XmlElement("salary")]
        public decimal Salary { get; set; }

        /// <summary>
        /// Gets or sets employee's department letter.
        /// </summary>
        /// <value>Employee's department letter.</value>
        [XmlElement(ElementName = "department")]
        public char Department { get; set; }

        /// <summary>
        /// Overrides object's method <see cref="object.ToString"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> representation of the <see cref="FileCabinetRecord"/> instance.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(CultureInfo.InvariantCulture, $"{this.Id}, ");
            builder.Append(CultureInfo.InvariantCulture, $"{this.FirstName}, ");
            builder.Append(CultureInfo.InvariantCulture, $"{this.LastName}, ");
            builder.Append(CultureInfo.InvariantCulture, $"{this.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, ");
            builder.Append(CultureInfo.InvariantCulture, $"{this.WorkPlaceNumber}, ");
            builder.Append(CultureInfo.InvariantCulture, $"{this.Salary}, ");
            builder.Append(CultureInfo.InvariantCulture, $"{this.Department}");
            return builder.ToString();
        }
    }
}
