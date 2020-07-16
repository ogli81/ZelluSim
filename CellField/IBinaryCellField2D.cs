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
    public interface IBinaryCellField2D : ICellField2D, ICloneable
    {
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

        /// <summary>
        /// Copy the binary value from a specific source to a specific destination. 
        /// In this kind of cell field it's just a bit that we copy.
        /// The source is in this instance and the destination is in an 'other' instance (but could be this too). 
        /// <br></br>NOTE: The x/y boundaries won't be checked (violating the bounds may lead to unexpected behavior).
        /// </summary>
        /// <param name="fromThisX">from where (x-coordinate) should we copy the bit</param>
        /// <param name="fromThisY">from where (y-coordinate) should we copy the bit</param>
        /// <param name="other">to which other instance should we transfer the bit</param>
        /// <param name="toOtherX">to which x-coordinate should we copy</param>
        /// <param name="toOtherY">to which y-coordinate should we copy</param>
        void CopyIntoOther(int fromThisX, int fromThisY, IBinaryCellField2D other, int toOtherX, int toOtherY);

        /// <summary>
        /// Copy the binary value from a specific source to a specific destination. 
        /// In this kind of cell field it's just a bit that we copy.
        /// The source is in an 'other' instance (but could be this too). The destination is in this instance.
        /// <br></br>NOTE: The x/y boundaries won't be checked (violating the bounds may lead to unexpected behavior).
        /// </summary>
        /// <param name="other">from which other instance should we transfer the bit</param>
        /// <param name="fromOtherX">from which x-coordinate should we copy the bit</param>
        /// <param name="fromOtherY">from which y-coordinate should we copy the bit</param>
        /// <param name="toThisX">to where (x-coordinate) should we copy the bit</param>
        /// <param name="toThisY">to where (y-coordinate) should we copy the bit</param>
        void CopyFromOther(IBinaryCellField2D other, int fromOtherX, int fromOtherY, int toThisX, int toThisY);
    }
}
