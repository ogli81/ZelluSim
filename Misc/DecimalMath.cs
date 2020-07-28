using System;
using System.Collections.Generic;
using System.Text;

namespace ZelluSim.Misc
{
    public class DecimalMath
    {
        /// <summary>
        /// The Sqrt-function for decimals as suggested by SLenik @ stackoverflow.com.
        /// https://stackoverflow.com/questions/4124189/performing-math-operations-on-decimal-datatype-in-c
        /// </summary>
        /// <param name="x">a number, from which we need to calculate the square root</param>
        /// <param name="epsilon">an accuracy of calculation of the root from our number.
        /// The result of the calculations will differ from an actual value
        /// of the root on less than epslion.</param>
        /// <returns></returns>
        public static decimal Sqrt(decimal x, decimal epsilon = 0.0M)
        {
            if (x < 0) throw new OverflowException("Cannot calculate square root from a negative number");

            decimal current = (decimal)Math.Sqrt((double)x), previous;
            do
            {
                previous = current;
                if (previous == 0.0M) return 0;
                current = (previous + x / previous) / 2;
            }
            while (Math.Abs(previous - current) > epsilon);
            return current;
        }
    }
}
