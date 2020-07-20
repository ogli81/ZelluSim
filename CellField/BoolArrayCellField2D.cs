using System;
using System.Collections.Generic;
using System.Text;
using ZelluSim.Misc;

namespace ZelluSim.CellField
{
    public class BoolArrayCellField2D : AbstractBinaryCellField2D, IBinaryCellField2D, ICloneable
    {
        //state

        public readonly bool[,] Cells;


        //c'tors

        public BoolArrayCellField2D(int cellsX, int cellsY) : base(cellsX, cellsY)
        {
            Cells = new bool[cellsX, cellsY];
        }

        public BoolArrayCellField2D(BoolArrayCellField2D other) : base(other.CellsX, other.CellsY)
        {
            Cells = (bool[,])other.Cells.Clone();
        }

        public BoolArrayCellField2D(int cellsX, int cellsY, BoolArrayCellField2D other) : this(cellsX, cellsY)
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
            //BoundsCheck(x, y); //the array will do its own bounds checks
            return Cells[x, y];
        }

        public override void SetCellValue(int x, int y, bool value)
        {
            //BoundsCheck(x, y); //the array will do its own bounds checks
            Cells[x,y] = value;
        }

        public override void SetAllCells(bool value)
        {
            //Array.Fill<T>(Cells, value, 0, Cells.Length); //<--- not available in my version of .Net :-(
            for (int i = 0; i < CellsX; i++)
                for (int j = 0; j < CellsY; j++)
                    Cells[i, j] = value;
        }

        public BoolArrayCellField2D Clone()
        {
            return new BoolArrayCellField2D(this);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
