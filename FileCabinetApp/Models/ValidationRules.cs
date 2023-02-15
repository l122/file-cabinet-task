using System;
using Newtonsoft.Json;

namespace FileCabinetApp.Models
{
    /// <summary>
    /// A model class for validation rules.
    /// </summary>
    [JsonObject("ValidationRules")]
    public class ValidationRules
    {
        /// <summary>
        /// Gets or sets and sets FirstName.
        /// </summary>
        /// <value>And sets FirstName.</value>
        [JsonProperty("firstName")]
        public Name FirstName { get; set; }

        /// <summary>
        /// Gets or sets and sets LastName.
        /// </summary>
        /// <value>And sets LastName.</value>
        [JsonProperty("lastName")]
        public Name LastName { get; set; }

        /// <summary>
        /// Gets or sets and sets DateOfBirthType.
        /// </summary>
        /// <value>And sets DateOfBirthType.</value>
        [JsonProperty("dateOfBirth")]
        public DateOfBirthType DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets and sets WorkplaceType.
        /// </summary>
        /// <value>And sets WorkplaceType.</value>
        [JsonProperty("workplace")]
        public WorkplaceType Workplace { get; set; }

        /// <summary>
        /// Gets or sets and sets SalaryType.
        /// </summary>
        /// <value>And sets SalaryType.</value>
        [JsonProperty("salary")]
        public SalaryType Salary { get; set; }

        /// <summary>
        /// Gets or sets and sets DepartmentType.
        /// </summary>
        /// <value>And sets DepartmentType.</value>
        [JsonProperty("department")]
        public DepartmentType Department { get; set; }

        /// <summary>
        /// Name Type.
        /// </summary>
        public struct Name
        {
            /// <summary>
            /// Gets or sets and sets Min.
            /// </summary>
            /// <value>And sets Min.</value>
            [JsonProperty("min")]
            public int Min { get; set; }

            /// <summary>
            /// Gets or sets and sets Max.
            /// </summary>
            /// <value>And sets Max.</value>
            [JsonProperty("max")]
            public int Max { get; set; }
        }

        /// <summary>
        /// Date type.
        /// </summary>
        public struct DateOfBirthType
        {
            /// <summary>
            /// Gets or sets and sets From.
            /// </summary>
            /// <value>And sets From.</value>
            [JsonProperty("from")]
            public DateTime From { get; set; }

            /// <summary>
            /// Gets or sets and sets To.
            /// </summary>
            /// <value>And sets To.</value>
            [JsonProperty("to")]
            public DateTime To { get; set; }
        }

        /// <summary>
        /// Workplace type.
        /// </summary>
        public struct WorkplaceType
        {
            /// <summary>
            /// Gets or sets and sets Min.
            /// </summary>
            /// <value>And sets Min.</value>
            [JsonProperty("min")]
            public short Min { get; set; }

            /// <summary>
            /// Gets or sets and sets Max.
            /// </summary>
            /// <value>And sets Max.</value>
            [JsonProperty("max")]
            public short Max { get; set; }
        }

        /// <summary>
        /// Salary type.
        /// </summary>
        public struct SalaryType
        {
            /// <summary>
            /// Gets or sets and sets Min.
            /// </summary>
            /// <value>And sets Min.</value>
            [JsonProperty("min")]
            public decimal Min { get; set; }

            /// <summary>
            /// Gets or sets and sets Max.
            /// </summary>
            /// <value>And sets Max.</value>
            [JsonProperty("max")]
            public decimal Max { get; set; }
        }

        /// <summary>
        /// Departmen Type.
        /// </summary>
        public struct DepartmentType
        {
            /// <summary>
            /// Gets or sets and sets Start.
            /// </summary>
            /// <value>And sets Start.</value>
            [JsonProperty("start")]
            public char Start { get; set; }

            /// <summary>
            /// Gets or sets and sets End.
            /// </summary>
            /// <value>And sets End.</value>
            [JsonProperty("end")]
            public char End { get; set; }
        }
    }
}
