using System;
using System.Collections.Generic;
using System.Text;
using ZelluSim.Misc;

namespace ZelluSim.CellField
{
    public abstract class AbstractBinaryCellField2D : AbstractCellField2D
    {
        //state:

        //-


        //c'tors:

        public AbstractBinaryCellField2D(int cellsX, int cellsY) : base(cellsX, cellsY)
        {
        }


        //public methods:

        /// <inheritdoc/>
        public abstract void SetAllCells(bool value);

        /// <inheritdoc/>
        public abstract bool this[int x, int y] { get; set; }

        /// <inheritdoc/>
        public abstract bool GetCellValue(int x, int y);

        /// <inheritdoc/>
        public abstract void SetCellValue(int x, int y, bool value);

        /// <inheritdoc/>
        public void ClearAllWithDefault()
        {
            SetAllCells(false);
        }

        /// <inheritdoc/>
        public void CopyIntoOther(int fromThisX, int fromThisY, IBinaryCellField2D other, int toOtherX, int toOtherY)
        {
            other[toOtherX, toOtherY] = this[fromThisX, fromThisY];
        }

        /// <inheritdoc/>
        public void CopyFromOther(IBinaryCellField2D other, int fromOtherX, int fromOtherY, int toThisX, int toThisY)
        {
            this[toThisX, toThisY] = other[fromOtherX, fromOtherY];
        }

        /// <inheritdoc/>
        public bool GetCellValueWithWrap(int x, int y, Direction direction)
        {
            int ix = x, iy = y; //we could operate on x and y, but let's make new variables (cleaner code)
            switch (direction)
            {
                case Direction.N: if (y == 0) iy = CellsY - 1; break;
                case Direction.NE: if (x == CellsX - 1) ix = 0; if (y == 0) iy = CellsY - 1; break;
                case Direction.E: if (x == CellsX - 1) ix = 0; break;
                case Direction.SE: if (x == CellsX - 1) ix = 0; if (y == CellsY - 1) iy = 0; break;
                case Direction.S: if (y == CellsY - 1) iy = 0; break;
                case Direction.SW: if (x == 0) ix = CellsX - 1; if (y == CellsY - 1) iy = 0; break;
                case Direction.W: if (x == 0) ix = CellsX - 1; break;
                case Direction.NW: if (x == 0) ix = CellsX - 1; if (y == 0) iy = CellsY - 1; break;
            }
            return GetCellValue(ix, iy);
        }

        public bool GetCellValueWithoutWrap(int x, int y, Direction direction, bool outsideVal)
        {
            switch (direction)
            {
                case Direction.N: if (y == 0) return outsideVal; break;
                case Direction.NE: if (x == CellsX - 1) return outsideVal; if (y == 0) return outsideVal; break;
                case Direction.E: if (x == CellsX - 1) return outsideVal; break;
                case Direction.SE: if (x == CellsX - 1) return outsideVal; if (y == CellsY - 1) return outsideVal; break;
                case Direction.S: if (y == CellsY - 1) return outsideVal; break;
                case Direction.SW: if (x == 0) return outsideVal; if (y == CellsY - 1) return outsideVal; break;
                case Direction.W: if (x == 0) return outsideVal; break;
                case Direction.NW: if (x == 0) return outsideVal; if (y == 0) return outsideVal; break;
            }
            return GetCellValue(x, y);
        }
    }
}
