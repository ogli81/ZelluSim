using System;
using System.Collections.Generic;
using System.Text;
using ZelluSim.Misc;

namespace ZelluSim.CellField
{
    public class SimpleCellField2D<T> : AbstractGenericCellField2D<T>, IGenericCellField2D<T>, ICloneable
    {
        //state

        //protected readonly T[,] Cells; //older software architecture made this possible
        protected T[,] Cells;


        //c'tors

        public SimpleCellField2D(int cellsX, int cellsY) : base(cellsX, cellsY, false)
        {
            
        }

        public SimpleCellField2D(SimpleCellField2D<T> other, bool tryDeepCopy = false) : base(other.CellsX, other.CellsY, tryDeepCopy)
        {
            if (tryDeepCopy)
            {
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
                Cells = (T[,])other.Cells.Clone(); //should be faster than the following two lines (TODO: performance comparison)
                //cells = new T[other.cellsX,other.cellsY];
                //Array.Copy(other.cells, cells, cells.Length);
            }
        }

        public SimpleCellField2D(int cellsX, int cellsY, SimpleCellField2D<T> other, bool tryDeepCopy = false) : base(cellsX, cellsY, tryDeepCopy)
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

        protected override void CreateArray(int cellsX, int cellsY)
        {
            Cells = new T[cellsX, cellsY];
        }

        protected override bool DetectCloneables()
        {
            foreach (T element in Cells)
                if (element is ICloneable)
                    return true;
            return false;
        }


        //public methods

        public override T GetCellValueWithWrap(int x, int y, Direction direction)
        {
            //BoundsCheck(x, y); //the array will do its own bounds check
            switch (direction)
            {
                case Direction.N: return GetCellValue(x, (y == 0) ? CellsY - 1 : y - 1);
                case Direction.NE: return GetCellValue((x == CellsX - 1) ? 0 : x + 1, (y == 0) ? CellsY - 1 : y - 1);
                case Direction.E: return GetCellValue((x == CellsX - 1) ? 0 : x + 1, y);
                case Direction.SE: return GetCellValue((x == CellsX - 1) ? 0 : x + 1, (y == CellsY - 1) ? 0 : y + 1);
                case Direction.S: return GetCellValue(x, (y == CellsY - 1) ? 0 : y + 1);
                case Direction.SW: return GetCellValue((x == 0) ? CellsX - 1 : x - 1, (y == CellsY - 1) ? 0 : y + 1);
                case Direction.W: return GetCellValue((x == 0) ? CellsX - 1 : x - 1, y);
                case Direction.NW: return GetCellValue((x == 0) ? CellsX - 1 : x - 1, (y == 0) ? CellsY - 1 : y - 1);
            }
            //throw new ArgumentException("Unknown direction: " + direction);
            return default(T); //probably faster?
        }

        public override T GetCellValue(int x, int y)
        {
            //BoundsCheck(x, y); //the array will do its own bounds check
            return Cells[x, y];
        }

        public override void SetCellValue(int x, int y, T value)
        {
            BoundsCheck(x, y);
            Cells[x, y] = value;
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

        public override void SetAllNulls(T value, bool tryDeepCopy = false)
        {
            if (tryDeepCopy && value is ICloneable)
            {
                ICloneable cloneable = (ICloneable)value;
                for (int i = 0; i < CellsX; i++)
                    for (int j = 0; j < CellsY; j++)
                        if(Cells[i, j] == null)
                            Cells[i, j] = (T)cloneable.Clone();
            }
            else
            {
                for (int i = 0; i < CellsX; i++)
                    for (int j = 0; j < CellsY; j++)
                        if(Cells[i, j] == null)
                            Cells[i, j] = value;
            }
        }

        public IGenericCellField2D<T> ResizedClone(int cellsX, int cellsY)
        {
            if (CloningPolicy == CloningPolicy.TRY_DEEP_CLONE)
                return new SimpleCellField2D<T>(cellsX, cellsY, this, true);
            else if (CloningPolicy == CloningPolicy.DO_NOT_TRY_DEEP_CLONE)
                return new SimpleCellField2D<T>(cellsX, cellsY, this, false);
            else if (CloningPolicy == CloningPolicy.USE_DEFAULT)
                return new SimpleCellField2D<T>(cellsX, cellsY, this);
            throw new ArgumentException($"unknown cloning policy: {CloningPolicy}");
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
