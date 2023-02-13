using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Strategy interface for choosing a validator object.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validates input value.
        /// </summary>
        /// <param name="value">A <see cref="string"/> isntance.</param>
        /// <returns>A <see cref="Tuple{T1, T2}"/>.</returns>
        public Tuple<bool, string> ValidateParameters(object value);
    }
}