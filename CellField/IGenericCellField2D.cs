using System;
using System.Collections.Generic;
using System.Text;
using ZelluSim.Misc;

namespace ZelluSim.CellField
{
    /// <summary>
    /// A 2-dimensional field of cells (a "cell grid") that stores generic values. 
    /// The generic type could be anything, but there is a more efficient version for binary (true/false) 
    /// values <see cref="IBinaryCellField2D"/>.
    /// </summary>
    /// <typeparam name="T">Any type you want, but consider bool with <see cref="IBinaryCellField2D"/></typeparam>
    public interface IGenericCellField2D<T> : ICellField2D, ICloneable
    {
        /// <summary>
        /// Try to get the content of the neighboring cell in that specific direction. 
        /// This behavior uses the "wrap-around at world boundaries" semantics.
        /// The attempt may fail when x or y is out of bounds (an exception will be thrown).
        /// </summary>
        /// <param name="x">the x coordinate of the cell you are interested in</param>
        /// <param name="y">thy y coordinate of the cell you are interested in</param>
        /// <param name="direction">the direction where we find the neighbor of our cell</param>
        /// <returns>the content of the neighbor cell</returns>
        T GetCellValueWithWrap(int x, int y, Direction direction);

        /// <summary>
        /// Try to get the content of the neighboring cell in that specific direction. 
        /// When there is no neighbor due to missing "wrap around" semantics, a default value is 
        /// needed, which you must provide.
        /// The attempt may fail when x or y is out of bounds (an exception will be thrown).
        /// </summary>
        /// <param name="x">the x coordinate of the cell you are interested in</param>
        /// <param name="y">thy y coordinate of the cell you are interested in</param>
        /// <param name="direction">the direction where we find the neighbor of our cell</param>
        /// <param name="outsideVal">the default value in case our world ends here and so there is no neighboring cell</param>
        /// <returns>the content of the neighbor cell (or the default value 'outsideVal')</returns>
        T GetCellValueWithoutWrap(int x, int y, Direction direction, T outsideVal);

        /// <summary>
        /// Try to get the content of the cell.
        /// The attempt may fail when x or y is out of bounds (an exception will be thrown).
        /// </summary>
        /// <param name="x">the x coordinate of the cell you are interested in</param>
        /// <param name="y">thy y coordinate of the cell you are interested in</param>
        /// <returns>the content of the cell</returns>
        T GetCellValue(int x, int y);

        /// <summary>
        /// Try to set the content of the cell.
        /// The attempt may fail when x or y is out of bounds (an exception will be thrown).
        /// </summary>
        /// <param name="x">the x coordinate of the cell you are interested in</param>
        /// <param name="y">thy y coordinate of the cell you are interested in</param>
        /// <param name="value">the new value for the cell</param>
        void SetCellValue(int x, int y, T value);

        /// <summary>
        /// Set the content of all cells to a specific value.
        /// </summary>
        /// <param name="value">the value that you want to set for all cells</param>
        /// <param name="tryDeepCopy">if your value implements ICloneable, you may want to set this parameter to true</param>
        void SetAllCells(T value, bool tryDeepCopy = false);

        //
        void SetAllNulls(T value, bool tryDeepCopy = false);

        /// <summary>
        /// Try to get or set the cell value via a (2-dimensional) indexer. 
        /// The attempt may fail when x or y is out of bounds (an exception will be thrown).
        /// </summary>
        /// <param name="x">the x coordinate of the cell you are interested in</param>
        /// <param name="y">thy y coordinate of the cell you are interested in</param>
        /// <returns>the current value of the cell</returns>
        T this[int x, int y] { get; set; }

        /// <summary>
        /// How should we handle cloning of the contained elements - this policy defines it.
        /// </summary>
        /// <param name="policy">the desired cloning policy</param>
        CloningPolicy CloningPolicy { get; set; }

        /// <summary>
        /// Copy/clone the contained elements from a specific source to a specific destination.
        /// The source is in this instance and the destination is in an 'other' instance (but could be this too).
        /// <br></br>NOTE: The x/y boundaries won't be checked (violating the bounds may lead to unexpected behavior).
        /// <seealso cref="CloningPolicy"/>
        /// <seealso cref="CloneInSelf(int, int, int, int)"/>
        /// <seealso cref="CloneFromOther(IGenericCellField2D{T}, int, int, int, int)"/>
        /// </summary>
        /// <param name="fromThisX">from where (x-coordinate) should we copy/clone</param>
        /// <param name="fromThisY">from where (y-coordinate) should we copy/clone</param>
        /// <param name="other">to which other instance should we transfer the copy/clone of the element</param>
        /// <param name="toOtherX">to which x-coordinate should we copy/clone</param>
        /// <param name="toOtherY">to which y-coordinate should we copy/clone</param>
        void CloneIntoOther(int fromThisX, int fromThisY, IGenericCellField2D<T> other, int toOtherX, int toOtherY);

        /// <summary>
        /// Copy/clone the contained elements from a specific source to a specific destination.
        /// The source is in an 'other' instance (but could be this too). The destination is in this instance.
        /// <br></br>NOTE: The x/y boundaries won't be checked (violating the bounds may lead to unexpected behavior).
        /// <seealso cref="CloningPolicy"/>
        /// <seealso cref="CloneInSelf(int, int, int, int)"/>
        /// <seealso cref="CloneIntoOther(int, int, IGenericCellField2D{T}, int, int)"/>
        /// </summary>
        /// <param name="other">from which other instance should we transfer the copy/clone of the element</param>
        /// <param name="fromOtherX">from which x-coordinate should we copy/clone</param>
        /// <param name="fromOtherY">from which y-coordinate should we copy/clone</param>
        /// <param name="toThisX">to where (x-coordinate) should we copy/clone</param>
        /// <param name="toThisY">to where (y-coordinate) should we copy/clone</param>
        void CloneFromOther(IGenericCellField2D<T> other, int fromOtherX, int fromOtherY, int toThisX, int toThisY);

        //TODO
        void CloneFromRegion(IGenericCellField2D<T> source, (int x, int y) srcUpperLeft, (int width, int height) dimension, (int x, int y) dstUpperLeft);
    }
}
