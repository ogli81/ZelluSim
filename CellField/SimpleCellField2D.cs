using System;
using System.Collections.Generic;
using System.Text;
using ZelluSim.Misc;

namespace ZelluSim.CellField
{
    public class SimpleCellField2D<T> : AbstractGenericCellField2D<T>, IGenericCellField2D<T>, ICloneable
    {
        //state

        protected readonly T[,] Cells;


        //c'tors

        public SimpleCellField2D(int cellsX, int cellsY) : base(cellsX, cellsY)
        {
            Cells = new T[cellsX, cellsY];
        }

        public SimpleCellField2D(SimpleCellField2D<T> other, bool tryDeepCopy = false) : base(other.CellsX, other.CellsY)
        {
            if (tryDeepCopy)
            {
                cloningPolicy = CloningPolicy.TRY_DEEP_CLONE;
                Cells = new T[other.CellsX, other.CellsY];
                T element;
                for (int i = 0; i < CellsX; i++)
                    for (int j = 0; j < CellsY; j++)
                    {
                        element = other.Cells[i, j];
                        //TODO: which of the following three lines is the fastest? (do performance comparison)
                        //cells[i, j] = element is ICloneable ? (T)(element as ICloneable).Clone() : element;
                        //cells[i, j] = element is ICloneable ? (T)((ICloneable)element).Clone() : element;
                        Cells[i, j] = element is ICloneable cloneable ? (T)cloneable.Clone() : element;
                    }
            }
            else
            {
                cloningPolicy = CloningPolicy.DO_NOT_TRY_DEEP_CLONE;
                Cells = (T[,])other.Cells.Clone(); //should be faster than the following two lines (TODO: performance comparison)
                //cells = new T[other.cellsX,other.cellsY];
                //Array.Copy(other.cells, cells, cells.Length);
            }
        }

        public SimpleCellField2D(int cellsX, int cellsY, SimpleCellField2D<T> other, bool tryDeepCopy = false) : this(cellsX, cellsY)
        {
            //we copy as much as we can from the other
            //use top-left corner (0,0), work into x and y direction as far as we can
            //if this new cell field is bigger than the other: the default value of Type T is used (when array is created)

            int x = CellsX;
            int y = CellsY;
            int ox = other.CellsX;
            int oy = other.CellsY;
            T[,] arr = Cells;
            T[,] oarr = other.Cells;
            T element;
            for (int a = 0; a < x && a < ox; a++)
                for (int b = 0; b < y && b < oy; b++)
                {
                    element = oarr[a, b];
                    if (tryDeepCopy)
                        arr[a, b] = element is ICloneable cloneable ? (T)cloneable.Clone() : element;
                    else
                        arr[a, b] = element;
                }
        }


        //helper methods

        protected override bool DetectCloneables()
        {
            foreach (T element in Cells)
                if (element is ICloneable)
                    return true;
            return false;
        }


        //public methods

        public T GetCellValueWithWrap(int x, int y, Direction direction)
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

        public T GetCellValueWithoutWrap(int x, int y, Direction direction, T outsideVal)
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

        public T GetCellValue(int x, int y)
        {
            BoundsCheck(x, y);
            return Cells[x, y];
        }

        public void SetCellValue(int x, int y, T value)
        {
            BoundsCheck(x, y);
            Cells[x, y] = value;
        }

        public override T this[int x, int y]
        {
            //get => GetCellValue(x, y);
            //set => SetCellValue(x, y, value);
            get => Cells[x, y];
            set => Cells[x, y] = value;
        }

        public override void SetAllCells(T value, bool tryDeepCopy = false)
        {
            if (tryDeepCopy && value is ICloneable)
            {
                ICloneable cloneable = (ICloneable)value;
                for (int i = 0; i < CellsX; i++)
                    for (int j = 0; j < CellsY; j++)
                        Cells[i, j] = (T)cloneable.Clone();
            }
            else
            {
                //Array.Fill<T>(Cells, value, 0, Cells.Length); //<--- not available in my version of .Net :-(
                for (int i = 0; i < CellsX; i++)
                    for (int j = 0; j < CellsY; j++)
                        Cells[i, j] = value;
            }
        }

        public SimpleCellField2D<T> Clone()
        {
            if (CloningPolicy == CloningPolicy.TRY_DEEP_CLONE)
                return new SimpleCellField2D<T>(this, true);
            else if (CloningPolicy == CloningPolicy.DO_NOT_TRY_DEEP_CLONE)
                return new SimpleCellField2D<T>(this, false);
            else if (CloningPolicy == CloningPolicy.USE_DEFAULT)
                return new SimpleCellField2D<T>(this);
            throw new ArgumentException($"unknown cloning policy: {CloningPolicy}");
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
