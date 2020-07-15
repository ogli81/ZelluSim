using System;
using System.Collections.Generic;
using System.Text;

namespace ZelluSim.CellField
{
    public abstract class AbstractCellField2D
    {
        //state:

        public int CellsX { get; }
        public int CellsY { get; }


        //c'tors:

        public AbstractCellField2D(int cellsX, int cellsY)
        {
            CellsX = cellsX;
            CellsY = cellsY;
        }


        //helper methods:

        protected void BoundsCheck(int x, int y)
        {
            //usually you want these checks only in DEBUG, not in RELEASE...
#if DEBUG
            if (x < 0) throw new ArgumentException("x can't be less than zero!");
            if (x >= CellsX) throw new ArgumentException($"x can't be >= {CellsX}");
            if (y < 0) throw new ArgumentException("y can't be less than zero!");
            if (y >= CellsY) throw new ArgumentException($"y can't be >= {CellsY}");
#endif
        }
    }
}
