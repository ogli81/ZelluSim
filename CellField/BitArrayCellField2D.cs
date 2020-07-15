using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ZelluSim.Misc;

namespace ZelluSim.CellField
{
    public class BitArrayCellField2D : AbstractCellField2D, IBinaryCellField2D, ICloneable
    {
        //state

        public readonly BitArray Cells;


        //c'tors

        public BitArrayCellField2D(int cellsX, int cellsY) : base(cellsX, cellsY)
        {
            Cells = new BitArray(cellsX * cellsY);
        }

        public BitArrayCellField2D(BitArrayCellField2D other) : base(other.CellsX, other.CellsY)
        {
            Cells = new BitArray(other.Cells);
        }

        public BitArrayCellField2D(int cellsX, int cellsY, BitArrayCellField2D other) : this(cellsX, cellsY)
        {
            //we copy as much as we can from the other
            //use top-left corner (0,0), work into x and y direction as far as we can
            //if this new cell field is bigger than the other: the default value of Type T is used (when array is created)

            int x = CellsX;
            int y = CellsY;
            int ox = other.CellsX;
            int oy = other.CellsY;
            for (int a = 0; a < x && a < ox; a++)
                for (int b = 0; b < y && b < oy; b++)
                    SetCellValue(a, b, other.GetCellValue(a, b));
        }


        //helper methods

        //-


        //public methods

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

        public bool GetCellValue(int x, int y)
        {
            BoundsCheck(x, y);
            return Cells.Get(y * CellsX + x);
        }

        public void SetCellValue(int x, int y, bool value)
        {
            BoundsCheck(x, y);
            Cells.Set(y * CellsX + x, value);
        }

        public bool this[int x, int y]
        {
            get => GetCellValue(x, y);
            set => SetCellValue(x, y, value);
        }

        public void SetAllCells(bool value)
        {
            Cells.SetAll(value);
        }

        public BitArrayCellField2D Clone()
        {
            return new BitArrayCellField2D(this);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
