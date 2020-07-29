using System;
using System.Collections.Generic;
using System.Text;

namespace ZelluSim.Misc
{
    public class DecimalMath
    {
        //TODO: better use #ifdef PREFER_SPEED_OVER_ACCURACY or an Enum with SPEED,COMPROMISE,PRECISION
        public static readonly string USE_THIS_SQRT_IMPLEMENTATION = "Sqrt2";

        /// <summary>
        /// Calculate the sqare root of a decimal value.
        /// </summary>
        /// <param name="x">a number, from which we need to calculate the square root</param>
        /// <returns>the square root of x</returns>
        public static decimal Sqrt(decimal x)
        {
            switch (USE_THIS_SQRT_IMPLEMENTATION)
            {
                case "Sqrt1": return Sqrt1(x);
                case "Sqrt2": return Sqrt2(x);
                case "Sqrt3": return Sqrt3(x);
                default: throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Simple (but inaccurate) way to calculate the sqare root of a decimal value.
        /// This function uses <see cref="Math.Sqrt(double)"/> and casting between decimal and double. 
        /// For some values, the output of this function will be inaccurate. 
        /// The execution of this function is (in theory) the fastest.
        /// </summary>
        /// <param name="x">a number, from which we need to calculate the square root</param>
        /// <returns>the square root of x</returns>
        public static decimal Sqrt1(decimal x)
        {
            return (decimal)Math.Sqrt((double)x);
        }

        /// <summary>
        /// The Sqrt-function for decimals as suggested by SLenik @ stackoverflow.com. 
        /// This function is usually more accurate than just using <see cref="Math.Sqrt(double)"/>.
        /// The output of this function is not always accurate, but the function is quite fast.
        /// https://stackoverflow.com/questions/4124189/performing-math-operations-on-decimal-datatype-in-c
        /// </summary>
        /// <param name="x">a number, from which we need to calculate the square root</param>
        /// <param name="epsilon">an accuracy of calculation of the root from our number.
        /// The result of the calculations will differ from an actual value
        /// of the root on less than epslion.</param>
        /// <returns>the square root of x</returns>
        public static decimal Sqrt2(decimal x, decimal epsilon = 0.0M)
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

        /// <summary>
        /// The Sqrt-function for decimals as suggested by Bobson @ stackoverflow.com. 
        /// This function is very accurate, but (in theory) also very slow.
        /// https://stackoverflow.com/questions/4124189/performing-math-operations-on-decimal-datatype-in-c
        /// </summary>
        /// <param name="x">a number, from which we need to calculate the square root</param>
        /// <param name="guess">this parameter is needed for the recursive call to 
        /// this method - just leave it being null</param>
        /// <returns>the square root of x</returns>
        public static decimal Sqrt3(decimal x, decimal? guess = null)
        {
            var ourGuess = guess.GetValueOrDefault(x / 2m);
            var result = x / ourGuess;
            var average = (ourGuess + result) / 2m;

            if (average == ourGuess) // This checks for the maximum precision possible with a decimal.
                return average;
            else
                return Sqrt3(x, average);
        }
    }
}
