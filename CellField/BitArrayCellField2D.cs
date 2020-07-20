using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ZelluSim.Misc;

namespace ZelluSim.CellField
{
    public class BitArrayCellField2D : AbstractBinaryCellField2D, IBinaryCellField2D, ICloneable
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

        public override bool GetCellValue(int x, int y)
        {
            BoundsCheck(x, y); //the BitArray can't do its own bounds checks for 2d coordinates
            return Cells.Get(y * CellsX + x);
        }

        public override void SetCellValue(int x, int y, bool value)
        {
            BoundsCheck(x, y); //the BitArray can't do its own bounds checks for 2d coordinates
            Cells.Set(y * CellsX + x, value);
        }

        public override void SetAllCells(bool value)
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
