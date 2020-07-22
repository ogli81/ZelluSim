using System;
using System.Collections.Generic;
using System.Text;
using ZelluSim.Misc;

namespace ZelluSim.CellField
{
    public class MeshedCellField2D<T> : AbstractGenericCellField2D<T>, IGenericCellField2D<T>, ICloneable
    {
        //state

        //protected readonly MeshNode<T>[,] Cells; //older software architecture made this possible
        protected MeshNode<T>[,] Cells;


        //c'tors

        public MeshedCellField2D(int cellsX, int cellsY) : base(cellsX, cellsY, false)
        {

        }

        public MeshedCellField2D(MeshedCellField2D<T> other, bool tryDeepCopy = false) : base(other.CellsX, other.CellsY, tryDeepCopy)
        {
            if (tryDeepCopy)
            {
                T element;
                for (int i = 0; i < CellsX; i++)
                    for (int j = 0; j < CellsY; j++)
                    {
                        element = other.Cells[i, j].Value;
                        //TODO: which of the following three lines is the fastest? (do performance comparison)
                        //cells[i, j] = element is ICloneable ? (T)(element as ICloneable).Clone() : element;
                        //cells[i, j] = element is ICloneable ? (T)((ICloneable)element).Clone() : element;
                        Cells[i, j].Value = element is ICloneable cloneable ? (T)cloneable.Clone() : element;
                    }
            }
            else
            {
                for (int i = 0; i < CellsX; i++)
                    for (int j = 0; j < CellsY; j++)
                        SetCellValue(i, j, other.GetCellValue(i, j));
            }
        }

        public MeshedCellField2D(int cellsX, int cellsY, MeshedCellField2D<T> other, bool tryDeepCopy = false) : base(cellsX, cellsY, tryDeepCopy)
        {
            //we copy as much as we can from the other
            //use top-left corner (0,0), work into x and y direction as far as we can
            //if this new cell field is bigger than the other: the default value of Type T is used (when array is created)

            int x = CellsX;
            int y = CellsY;
            int ox = other.CellsX;
            int oy = other.CellsY;
            MeshNode<T>[,] arr = Cells;
            MeshNode<T>[,] oarr = other.Cells;
            T element;
            for (int a = 0; a < x && a < ox; a++)
                for (int b = 0; b < y && b < oy; b++)
                {
                    element = oarr[a, b].Value;
                    if (tryDeepCopy)
                        arr[a, b].Value = element is ICloneable cloneable ? (T)cloneable.Clone() : element;
                    else
                        arr[a, b].Value = element;
                }
        }

        //helper methods

        protected override void CreateArray(int cellsX, int cellsY)
        {
            Cells = new MeshNode<T>[cellsX, cellsY];
            ConnectMesh();
        }

        private void ConnectMesh()
        {
            for(int i = 0; i < CellsX; i++)
                for(int j = 0; j < CellsY; j++)
                {
                    Cells[i,j].SetNeighbor(Direction.N, GetNeighborWithWrap(i, j, Direction.N));
                    Cells[i, j].SetNeighbor(Direction.NE, GetNeighborWithWrap(i, j, Direction.NE));
                    Cells[i, j].SetNeighbor(Direction.E, GetNeighborWithWrap(i, j, Direction.E));
                    Cells[i, j].SetNeighbor(Direction.SE, GetNeighborWithWrap(i, j, Direction.SE));
                    Cells[i, j].SetNeighbor(Direction.S, GetNeighborWithWrap(i, j, Direction.S));
                    Cells[i, j].SetNeighbor(Direction.SW, GetNeighborWithWrap(i, j, Direction.SW));
                    Cells[i, j].SetNeighbor(Direction.W, GetNeighborWithWrap(i, j, Direction.W));
                    Cells[i, j].SetNeighbor(Direction.NW, GetNeighborWithWrap(i, j, Direction.NW));
                }
        }

        protected MeshNode<T> GetNeighborWithWrap(int x, int y, Direction direction)
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
            return Cells[ix, iy];
        }

        protected override bool DetectCloneables()
        {
            foreach (MeshNode<T> node in Cells)
                if (node.Value is ICloneable)
                    return true;
            return false;
        }


        //public methods

        public override T GetCellValueWithWrap(int x, int y, Direction direction)
        {
            //BoundsCheck(x, y); //the array will do its own bounds check
            return Cells[x, y].GetNeighbor(direction).Value;
        }

        public override T GetCellValue(int x, int y)
        {
            //BoundsCheck(x, y); //the array will do its own bounds check
            return Cells[x, y].Value;
        }

        public override void SetCellValue(int x, int y, T value)
        {
            BoundsCheck(x, y);
            Cells[x, y].Value = value;
        }

        public override void SetAllCells(T value, bool tryDeepCopy = false)
        {
            if (tryDeepCopy && value is ICloneable)
            {
                ICloneable cloneable = (ICloneable)value;
                for (int i = 0; i < CellsX; i++)
                    for (int j = 0; j < CellsY; j++)
                        Cells[i, j].Value = (T)cloneable.Clone();
            }
            else
            {
                //Array.Fill<T>(Cells, value, 0, Cells.Length); //<--- not available in my version of .Net :-(
                for (int i = 0; i < CellsX; i++)
                    for (int j = 0; j < CellsY; j++)
                        Cells[i, j].Value = value;
            }
        }

        public override void SetAllNulls(T value, bool tryDeepCopy = false)
        {
            if (tryDeepCopy && value is ICloneable)
            {
                ICloneable cloneable = (ICloneable)value;
                for (int i = 0; i < CellsX; i++)
                    for (int j = 0; j < CellsY; j++)
                        if(Cells[i, j].Value == null)
                            Cells[i, j].Value = (T)cloneable.Clone();
            }
            else
            {
                for (int i = 0; i < CellsX; i++)
                    for (int j = 0; j < CellsY; j++)
                        if(Cells[i, j].Value == null)
                            Cells[i, j].Value = value;
            }
        }

        public MeshedCellField2D<T> Clone()
        {
            if (cloningPolicy == CloningPolicy.TRY_DEEP_CLONE)
                return new MeshedCellField2D<T>(this, true);
            else if (cloningPolicy == CloningPolicy.DO_NOT_TRY_DEEP_CLONE)
                return new MeshedCellField2D<T>(this, false);
            else if (cloningPolicy == CloningPolicy.USE_DEFAULT)
                return new MeshedCellField2D<T>(this);
            throw new ArgumentException($"unknown cloning policy: {cloningPolicy}");
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
