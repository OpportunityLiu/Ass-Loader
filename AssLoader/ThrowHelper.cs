using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace AssLoader
{
    internal static class ThrowHelper
    {
        public static void ThrowIfLessThanZeroOrOutOfRange(int max, int value, string paramName)
        {
            unchecked
            {
                if((uint)value >= (uint)max)
                    throw new ArgumentOutOfRangeException(paramName, paramName + " must greater than 0 and less than " + max);
            }
        }

        public static void ThrowIfOutOfRange(double min, double max, double value, string paramName)
        {
            if(value < min || value >= max)
                throw new ArgumentOutOfRangeException(paramName, paramName + " must greater than " + min + " and less than " + max);
        }

        public static void ThrowIfOutOfRange(int min, int max, int value, string paramName)
        {
            if(value < min || value >= max)
                throw new ArgumentOutOfRangeException(paramName, paramName + " must greater than " + min + " and less than " + max);
        }

        public static void ThrowIfLessThanZero(int value, string paramName)
        {
            if(value < 0)
                throw new ArgumentOutOfRangeException(paramName, paramName + " must greater than 0.");
        }

        public static void ThrowIfLessThanZero(double value, string paramName)
        {
            if(double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException($"{value} is not a valid value.", paramName);
            if(value < 0)
                throw new ArgumentOutOfRangeException(paramName, paramName + " must greater than 0.");
        }

        public static void ThrowIfNull<T>(T value, string paramName) where T : class
        {
            if(value == null)
                throw new ArgumentNullException(paramName);
        }

        public static void ThrowIfNullOrEmpty(string value, string paramName)
        {
            if(string.IsNullOrEmpty(value))
                throw new ArgumentNullException(paramName);
        }

        public static void ThrowIfNullOrWhiteSpace(string value, string paramName)
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(paramName);
        }
    }
}
