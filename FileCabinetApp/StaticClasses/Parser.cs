using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.StaticClasses
{
    /// <summary>
    /// Helper static class with parsing static functions.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Parses input arguments input a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="args">The <see cref="string"/> array instance.</param>
        /// <returns>The <see cref="Dictionary{TKey, TValue}"/> object.</returns>
        public static Dictionary<string, string> ParseArgs(string[] args)
        {
            Dictionary<string, string> result = new ();
            int i = 0;
            while (i < args.Length)
            {
                args[i] = args[i].ToLower(CultureInfo.InvariantCulture);
                if (args[i].StartsWith("--", StringComparison.InvariantCulture))
                {
                    var arg = args[i].Split("=");
                    if (arg.Length >= 2)
                    {
                        result.Add(arg[0], arg[1]);
                    }
                    else
                    {
                        result.Add(arg[0], string.Empty);
                    }
                }
                else if (args[i].StartsWith("-", StringComparison.InvariantCulture))
                {
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-", StringComparison.InvariantCulture))
                    {
                        result.Add(args[i], args[i + 1]);
                        i++;
                    }
                    else
                    {
                        result.Add(args[i], string.Empty);
                    }
                }

                i++;
            }

            return result;
        }

        /// <summary>
        /// Parses a record from array of bytes.
        /// </summary>
        /// <param name="buffer">A <see cref="byte"/> type input array.</param>
        /// <returns>A <see cref="FileCabinetRecord"/> instance.</returns>
        public static FileCabinetRecord ParseRecord(byte[] buffer)
        {
            const int StringBufferSize = 120;

            FileCabinetRecord record = new ();

            // Skip status bytes
            int offset = sizeof(short);

            // Parse Id
            record.Id = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);

            // Parse First Name
            record.FirstName = Encoding.UTF8.GetString(buffer, offset, StringBufferSize).Trim();
            offset += StringBufferSize;

            // Parse Last Name
            record.LastName = Encoding.UTF8.GetString(buffer, offset, StringBufferSize).Trim();
            offset += StringBufferSize;

            // Parse Birth Year
            var year = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);

            // Parse Birth Month
            var month = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);

            // Parse Birth Day
            var day = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);

            // Parse Date Of Birth
            record.DateOfBirth = new DateTime(year, month, day);

            // Parse Work Place Number
            record.WorkPlaceNumber = BitConverter.ToInt16(buffer, offset);
            offset += sizeof(short);

            // Parse SalaryType
            int[] parts = new int[4];
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = BitConverter.ToInt32(buffer, offset);
                offset += sizeof(int);
            }

            bool sign = (parts[3] & 0x80000000) != 0;

            byte scale = (byte)(parts[3] >> 16 & 0x7F);
            record.Salary = new decimal(parts[0], parts[1], parts[2], sign, scale);

            // Parse DepartmentType
            record.Department = BitConverter.ToChar(buffer, offset);

            return record;
        }
    }
}
