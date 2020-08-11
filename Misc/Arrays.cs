using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ZelluSim.Misc
{
    /// <summary>
    /// Static utility class that helps with arrays.
    /// </summary>
    public static class Arrays
    {
        /// <summary>
        /// Check, if an array is sorted in ascending order. Examples:<br></br>
        /// <code>
        /// int[] numbers = { 1, 2, 3 }; <br></br>
        /// string[] words = { "I", "like", "ZelluSim" };
        /// </code>
        /// </summary>
        /// <param name="anArray">the array that you want to check</param>
        /// <returns>true if sorted in ascending order</returns>
        public static bool IsSortedAscending(Array anArray)
        {
            Array clone = (Array)anArray.Clone();
            Array.Sort(clone);
            return ((IStructuralEquatable)anArray).Equals(clone, StructuralComparisons.StructuralEqualityComparer);
            //these didn't work
            //(Enumerable.SequenceEqual((IEnumerable)anArray, (IEnumerable)clone));
            //(Array.Equals(anArray, clone));
        }

        /// <summary>
        /// Check, if an array is sorted in descending order. Examples:<br></br>
        /// <code>
        /// int[] numbers = { 3, 2, 1 }; <br></br>
        /// string[] words = { "ZelluSim", "is", "great" };
        /// </code>
        /// </summary>
        /// <param name="anArray">the array that you want to check</param>
        /// <returns>true if sorted in descending order</returns>
        public static bool IsSortedDescending(Array anArray)
        {
            Array clone = (Array)anArray.Clone();
            Array.Sort(clone);
            Array.Reverse(clone);
            return ((IStructuralEquatable)anArray).Equals(clone, StructuralComparisons.StructuralEqualityComparer);
        }

        /// <summary>
        /// Write a proper string representation of the array.
        /// </summary>
        /// <param name="anArray">the array that you want to write as a string</param>
        /// <returns>a proper string for the array</returns>
        public static string ToString(Array anArray)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("type:").Append(anArray.GetType()).AppendLine();
            sb.Append("length:").Append(anArray.Length).AppendLine();
            sb.Append("values:").AppendLine();
            for (int i = 0; i < anArray.Length; ++i)
                sb.Append("[").Append(i).Append("]").Append("=").Append(anArray.GetValue(i)).Append(",").AppendLine();
            return sb.ToString();
        }
    }
}
