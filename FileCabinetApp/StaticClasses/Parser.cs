using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.StaticClasses
{
    /// <summary>
    /// Helper static class with parsing static functions.
    /// </summary>
    public static class Parser
    {
        private const string WhereStr = "where";
        private const string AndStr = "and";
        private const string OrStr = "or";
        private const string NotStr = "not";
        private const string EqualsStr = "=";
        private const string NotEqualsStr = "!=";

        private static readonly string[] LogicalOperators = { WhereStr, AndStr, OrStr };

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

        /// <summary>
        /// Parses a string with the 'where' expression into an enumerable object.
        /// </summary>
        /// <param name="enumerable">An <see cref="IEnumerable{T}"/> specialized instance of source data.</param>
        /// <param name="expression">A <see cref="string"/> instance expression.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> specialized instance of parsed expression.</returns>
        public static IEnumerable<FileCabinetRecord> ParseWhereExpression(IEnumerable<FileCabinetRecord> enumerable, string expression)
        {
            const int minimumParameters = 4;
            char[] splittingChars = new char[] { ' ', ',', '"' };

            // Insert spaces around '='.
            expression = expression.Replace("=", " = ", StringComparison.OrdinalIgnoreCase);
            expression = expression.Replace("! =", " != ", StringComparison.OrdinalIgnoreCase);

            var expr = expression.Split(splittingChars, StringSplitOptions.RemoveEmptyEntries);

            var result =
                    from record in enumerable
                    where record.Id == -1
                    select record;

            // Return an empty result query if the number of parameters is less than minimum.
            if (expr.Length < minimumParameters)
            {
                return result;
            }

            for (int i = 0; i < expr.Length;)
            {
                // Read logical operator.
                string logicalOperator = expr[i];

                // If the operator is not valid, then return empty expression.
                if (Array.FindIndex(LogicalOperators, 0, LogicalOperators.Length, p => p.Equals(expr[i], StringComparison.OrdinalIgnoreCase)) == -1)
                {
                    result =
                        from record in enumerable
                        where record.Id == -1
                        select record;

                    return result;
                }

                // Check if the next logical operator is 'not'.
                i++;
                bool negation = false;
                if (i < expr.Length && expr[i].Equals(NotStr, StringComparison.OrdinalIgnoreCase))
                {
                    negation = true;
                    i++;
                }

                // Check if enough parameters are left in the expression.
                if (i + 2 > expr.Length)
                {
                    break;
                }

                // Read field.
                var field = expr[i].ToLower(CultureInfo.InvariantCulture);

                // Read equality operator.
                i++;
                string equalityOperator = expr[i];

                // Read value.
                i++;
                var value = expr[i].Trim('\'');

                // Make expression.
                if ((i < minimumParameters && logicalOperator.Equals(WhereStr, StringComparison.OrdinalIgnoreCase))
                    || logicalOperator.Equals(OrStr, StringComparison.OrdinalIgnoreCase))
                {
                    result = result.Concat(CreateQuery(enumerable, equalityOperator, field, value, negation));
                }
                else if (logicalOperator.Equals(AndStr, StringComparison.Ordinal))
                {
                    result = CreateQuery(result, equalityOperator, field, value, negation);
                }

                i++;
            }

            return result;
        }

        /// <summary>
        /// Creates a query expression.
        /// </summary>
        /// <param name="inputQuery">An <see cref="IEnumerable{T}"/> specialized instance of an input query.</param>
        /// <param name="op">A <see cref="string"/> instance of the comparison operator.</param>
        /// <param name="field">A <see cref="string"/> name of the record's field.</param>
        /// <param name="value">A <see cref="string"/> value of the record's field.</param>
        /// <param name="negation">A <see cref="bool"/> value that indicates if 'Not' operator encountered.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> specialized instance of a query expression.</returns>
        /// <exception cref="ArgumentException">if operator is not supported.</exception>
        private static IEnumerable<FileCabinetRecord> CreateQuery(IEnumerable<FileCabinetRecord> inputQuery, string op, string field, string value, bool negation)
        {
            const string Id = "id";
            const string FirstName = "firstname";
            const string LastName = "lastname";
            const string DateOfBirth = "dateofbirth";
            const string Workplace = "workplace";
            const string Salary = "salary";
            const string Department = "department";

            if (op != EqualsStr && op != NotEqualsStr)
            {
                throw new ArgumentException("Unsupported comparison operator encountered: {0}", nameof(op));
            }

            var result = inputQuery;

            if ((op.Equals(EqualsStr, StringComparison.OrdinalIgnoreCase) && !negation)
                || (op.Equals(NotEqualsStr, StringComparison.OrdinalIgnoreCase) && negation))
            {
                switch (field)
                {
                    case Id:
                        var id = Converter.IntConverter(value);
                        if (id.Item1)
                        {
                            result =
                                from record in result
                                where record.Id == id.Item3
                                select record;
                        }

                        break;
                    case FirstName:
                        result =
                            from record in result
                            where record.FirstName.Equals(value, StringComparison.OrdinalIgnoreCase)
                            select record;
                        break;
                    case LastName:
                        result =
                            from record in result
                            where record.LastName.Equals(value, StringComparison.OrdinalIgnoreCase)
                            select record;
                        break;
                    case DateOfBirth:
                        var dateofbirth = Converter.DateConverter(value);
                        if (dateofbirth.Item1)
                        {
                            result =
                                from record in result
                                where record.DateOfBirth == dateofbirth.Item3
                                select record;
                        }

                        break;
                    case Workplace:
                        var workplace = Converter.ShortConverter(value);
                        if (workplace.Item1)
                        {
                            result =
                                from record in result
                                where record.WorkPlaceNumber == workplace.Item3
                                select record;
                        }

                        break;
                    case Salary:
                        var salary = Converter.DecimalConverter(value);
                        if (salary.Item1)
                        {
                            result =
                                from record in result
                                where record.Salary == salary.Item3
                                select record;
                        }

                        break;
                    case Department:
                        var depatment = Converter.CharConverter(value.ToUpper(CultureInfo.InvariantCulture));
                        if (depatment.Item1)
                        {
                            result =
                                from record in result
                                where record.Department.Equals(depatment.Item3)
                                select record;
                        }

                        break;
                    default:
                        result =
                            from record in result
                            where record.Id == -1
                            select record;
                        break;
                }
            }
            else
            {
                switch (field)
                {
                    case Id:
                        var id = Converter.IntConverter(value);
                        if (id.Item1)
                        {
                            result =
                                from record in result
                                where record.Id != id.Item3
                                select record;
                        }

                        break;
                    case FirstName:
                        result =
                            from record in result
                            where !record.FirstName.Equals(value, StringComparison.OrdinalIgnoreCase)
                            select record;
                        break;
                    case LastName:
                        result =
                            from record in result
                            where !record.LastName.Equals(value, StringComparison.OrdinalIgnoreCase)
                            select record;
                        break;
                    case DateOfBirth:
                        var dateofbirth = Converter.DateConverter(value);
                        if (dateofbirth.Item1)
                        {
                            result =
                                from record in result
                                where !record.DateOfBirth.Equals(dateofbirth.Item3)
                                select record;
                        }

                        break;
                    case Workplace:
                        var workplace = Converter.ShortConverter(value);
                        if (workplace.Item1)
                        {
                            result =
                                from record in result
                                where record.WorkPlaceNumber != workplace.Item3
                                select record;
                        }

                        break;
                    case Salary:
                        var salary = Converter.DecimalConverter(value);
                        if (salary.Item1)
                        {
                            result =
                                from record in result
                                where record.Salary != salary.Item3
                                select record;
                        }

                        break;
                    case Department:
                        var depatment = Converter.CharConverter(value);
                        if (depatment.Item1)
                        {
                            result =
                                from record in result
                                where record.Department != depatment.Item3
                                select record;
                        }

                        break;
                    default:
                        result =
                            from record in result
                            where record.Id == -1
                            select record;
                        break;
                }
            }

            return result;
        }
    }
}
