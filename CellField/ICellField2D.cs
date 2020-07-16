using System;
using System.Collections.Generic;
using System.Text;

namespace ZelluSim.CellField
{
    /// <summary>
    /// A 2-dimensional field of cells (a "cell grid") that stores the state of each cell. 
    /// This is the base interface for both <see cref="IGenericCellField2D"/> and <see cref="IBinaryCellField2D"/>.
    /// </summary>
    public interface ICellField2D
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
        /// Will set the content of all cells to the default value (usually 
        /// that's null for generics and false for bool and zero for numbers).
        /// </summary>
        void ClearAllWithDefault();

        //remark:
        //turns out, generic and binary cell fields don't have that much in common
    }
}
