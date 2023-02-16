using System;
using System.Globalization;

namespace FileCabinetApp.StaticClasses
{
    /// <summary>
    /// Helper static class with static conversion functions.
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Converts a <see cref="string"/> argument into a <see cref="char"/>.
        /// </summary>
        /// <param name="arg">The nullable <see cref="char"/> argument.</param>
        /// <returns>The <see cref="Tuple{T1, T2, T3}"/> value.</returns>
        public static Tuple<bool, string, char> CharConverter(string? arg)
        {
            if (!char.TryParse(arg, out char result))
            {
                return Tuple.Create(false, $"{arg} should be a letter.", default(char));
            }

            return Tuple.Create(true, string.Empty, result);
        }

        /// <summary>
        /// Converts a <see cref="string"/> argument into a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="arg">The nullable <see cref="DateTime"/> argument.</param>
        /// <returns>The <see cref="Tuple{T1, T2, T3}"/> value.</returns>
        public static Tuple<bool, string, DateTime> DateConverter(string? arg)
        {
            if (!DateTime.TryParse(arg, out DateTime dateOfBirth))
            {
                return Tuple.Create(false, $"{arg} is an invalid DateTime.", dateOfBirth);
            }

            return Tuple.Create(true, string.Empty, dateOfBirth);
        }

        /// <summary>
        /// Converts a <see cref="string"/> argument into a <see cref="decimal"/>.
        /// </summary>
        /// <param name="arg">The nullable <see cref="decimal"/> argument.</param>
        /// <returns>The <see cref="Tuple{T1, T2, T3}"/> value.</returns>
        public static Tuple<bool, string, decimal> DecimalConverter(string? arg)
        {
            if (!decimal.TryParse(arg, CultureInfo.InvariantCulture, out decimal result))
            {
                return Tuple.Create(false, $"{arg} is not a valid decimal type.", result);
            }

            return Tuple.Create(true, string.Empty, result);
        }

        /// <summary>
        /// Converts a <see cref="string"/> argument into a <see cref="short"/>.
        /// </summary>
        /// <param name="arg">The nullable <see cref="short"/> argument.</param>
        /// <returns>The <see cref="Tuple{T1, T2, T3}"/> value.</returns>
        public static Tuple<bool, string, short> ShortConverter(string? arg)
        {
            if (!short.TryParse(arg, CultureInfo.InvariantCulture, out short result))
            {
                return Tuple.Create(false, $"{arg} is not a valid short number.", result);
            }

            return Tuple.Create(true, string.Empty, result);
        }

        /// <summary>
        /// Converts a <see cref="string"/> argument into a <see cref="string"/>.
        /// </summary>
        /// <param name="arg">The nullable <see cref="string"/> argument.</param>
        /// <returns>The <see cref="Tuple{T1, T2, T3}"/> value.</returns>
        public static Tuple<bool, string, string> StringConverter(string? arg)
        {
            if (arg == null)
            {
                return Tuple.Create(false, "String cannot be empty.", string.Empty);
            }

            return Tuple.Create(true, string.Empty, arg);
        }
    }
}
