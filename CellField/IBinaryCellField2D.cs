using System;
using System.Collections.Generic;
using System.Text;
using ZelluSim.Misc;

namespace ZelluSim.CellField
{
    /// <summary>
    /// A 2-dimensional field of cells (a "cell grid") that stores binary (boolean) values. 
    /// There is also a version that stores generic values <see cref="IGenericCellField2D"/>.
    /// </summary>
    public interface IBinaryCellField2D : ICloneable
    {
        /// <summary>
        /// The number of cells in x (horizontal) direction.
        /// </summary>
        int CellsX { get; }

        /// <summary>
        /// The number of cells in y (vertical) direction.
        /// </summary>
        int CellsY { get; }

        /// <summary>
        /// Try to get the content of the neighboring cell in that specific direction. 
        /// This behavior uses the "wrap-around at world boundaries" semantics.
        /// The attempt may fail when x or y is out of bounds (an exception will be thrown).
        /// </summary>
        /// <param name="arr">the cell field</param>
        /// <param name="x">the x coordinate of the cell you are interested in</param>
        /// <param name="y">thy y coordinate of the cell you are interested in</param>
        /// <param name="direction">the direction where we find the neighbor of our cell</param>
        /// <returns>the content of the neighbor cell</returns>
        bool GetCellValueWithWrap(int x, int y, Direction direction);

        /// <summary>
        /// Try to get the content of the neighboring cell in that specific direction. 
        /// When there is no neighbor due to missing "wrap around" semantics, a default value is 
        /// needed, which you must provide.
        /// The attempt may fail when x or y is out of bounds (an exception will be thrown).
        /// </summary>
        /// <param name="arr">the cell field</param>
        /// <param name="x">the x coordinate of the cell you are interested in</param>
        /// <param name="y">thy y coordinate of the cell you are interested in</param>
        /// <param name="direction">the direction where we find the neighbor of our cell</param>
        /// <param name="outsideVal">the default value in case our world ends here and so there is no neighboring cell</param>
        /// <returns>the content of the neighbor cell (or the default value 'outsideVal')</returns>
        bool GetCellValueWithoutWrap(int x, int y, Direction direction, bool outsideVal);

        /// <summary>
        /// Try to get the content of the cell.
        /// The attempt may fail when x or y is out of bounds (an exception will be thrown).
        /// </summary>
        /// <param name="x">the x coordinate of the cell you are interested in</param>
        /// <param name="y">thy y coordinate of the cell you are interested in</param>
        /// <returns>the content of the cell</returns>
        bool GetCellValue(int x, int y);

        /// <summary>
        /// Try to set the content of the cell.
        /// The attempt may fail when x or y is out of bounds (an exception will be thrown).
        /// </summary>
        /// <param name="x">the x coordinate of the cell you are interested in</param>
        /// <param name="y">thy y coordinate of the cell you are interested in</param>
        /// <param name="value">the new value for the cell</param>
        void SetCellValue(int x, int y, bool value);

        /// <summary>
        /// Set the content of all cells to a specific value.
        /// </summary>
        /// <param name="value">the value that you want to set for all cells</param>
        void SetAllCells(bool value);

        /// <summary>
        /// Try to get or set the cell value via a (2-dimensional) indexer.
        /// The attempt may fail when x or y is out of bounds (an exception will be thrown).
        /// </summary>
        /// <param name="x">the x coordinate of the cell you are interested in</param>
        /// <param name="y">thy y coordinate of the cell you are interested in</param>
        /// <returns>the current value of the cell</returns>
        bool this[int x, int y] { get; set; }
    }
}
