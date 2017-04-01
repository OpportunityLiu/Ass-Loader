using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssLoader
{
    static class ThrowHelper
    {
        public static bool IsLessThanZeroOrOutOfRange(int max, int value)
        {
            unchecked
            {
                return ((uint)value) >= ((uint)max);
            }
        }

        public static bool IsInvalidDouble(double value) 
            => double.IsNaN(value) || double.IsInfinity(value);
    }
}
