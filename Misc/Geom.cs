using System;
using System.Collections.Generic;
using System.Text;

namespace ZelluSim.Misc
{
    /// <summary>
    /// This is a utility class for geometrical operations.
    /// </summary>
    public class Geom
    {

        /// <summary>
        /// Find out if two rectangular regions overlap. 
        /// Borders (edges) don't matter - they don't count as part of the rectangle. 
        /// Coordinates: "x axis goes from left to right" and "y axis goes from top to bottom".
        /// </summary>
        /// <param name="dimension">the width and height of the rectangular region (same for both)</param>
        /// <param name="firstUpperLeft">the upper left corner of the first rectangle</param>
        /// <param name="secondUpperLeft">the upper left corner of the second rectangle</param>
        /// <returns>true, if the rectangular regions overlap</returns>
        public static bool OverlapsIn2D((int width, int height) dimension,
            (int x, int y) firstUpperLeft, (int x, int y) secondUpperLeft)
        {
            if (dimension.width < 0 || dimension.height < 0)
                throw new ArgumentException("dimension can't be negative");

            if (firstUpperLeft.x > secondUpperLeft.x)
            {
                if ((secondUpperLeft.x + dimension.width) <= firstUpperLeft.x)
                    return false;
            }
            else
            {
                if ((firstUpperLeft.x + dimension.width) <= secondUpperLeft.x)
                    return false;
            }

            if (firstUpperLeft.y > secondUpperLeft.y)
            {
                if ((secondUpperLeft.y + dimension.height) <= firstUpperLeft.y)
                    return false;
            }
            else
            {
                if ((firstUpperLeft.y + dimension.height) <= secondUpperLeft.y)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Find out if two rectangular regions overlap. 
        /// Borders (edges) don't matter - they don't count as part of the rectangle. 
        /// Coordinates: "x axis goes from left to right" and "y axis goes from top to bottom".
        /// </summary>
        /// <param name="first">(x and y = "upper left corner", width and height = "dimension")</param>
        /// <param name="second">(x and y = "upper left corner", width and height = "dimension")</param>
        /// <returns>true, if the rectangular regions overlap</returns>
        public static bool OverlapsIn2D((int x, int y, int width, int height) first,
            (int x, int y, int width, int height) second)
        {
            if (first.width < 0 || first.height < 0 || second.width < 0 || second.height < 0)
                throw new ArgumentException("dimension can't be negative");

            if (first.x > second.x)
            {
                if ((second.x + second.width) <= first.x)
                    return false;
            }
            else
            {
                if ((first.x + first.width) <= second.x)
                    return false;
            }

            if (first.y > second.y)
            {
                if ((second.y + second.height) <= first.y)
                    return false;
            }
            else
            {
                if ((first.y + first.height) <= second.y)
                    return false;
            }

            return true;
        }
    }
}
