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
        public bool this[int x, int y]
        {
            get => GetCellValue(x, y);
            set => SetCellValue(x, y, value);
        }

        /// <inheritdoc/>
        public virtual bool GetCellValueWithWrap(int x, int y, Direction direction)
        {
            switch (direction)
            {
                case Direction.N: return GetCellValue(x, (y == 0) ? CellsY - 1 : y - 1);
                case Direction.NE: return GetCellValue((x == CellsX - 1) ? 0 : x + 1, (y == 0) ? CellsY - 1 : y - 1);
                case Direction.E: return GetCellValue((x == CellsX - 1) ? 0 : x + 1, y);
                case Direction.SE: return GetCellValue((x == CellsX - 1) ? 0 : x + 1, (y == CellsY - 1) ? 0 : y + 1);
                case Direction.S: return GetCellValue(x,(y == CellsY - 1) ? 0 : y + 1);
                case Direction.SW: return GetCellValue((x == 0) ? CellsX - 1 : x - 1, (y == CellsY - 1) ? 0 : y + 1);
                case Direction.W: return GetCellValue((x == 0) ? CellsX - 1 : x - 1, y);
                case Direction.NW: return GetCellValue((x == 0) ? CellsX - 1 : x - 1, (y == 0) ? CellsY - 1 : y - 1);
            }
            //throw new ArgumentException("Unknown direction: " + direction);
            return false; //probably faster?
        }

        /// <inheritdoc/>
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
            return GetCellValueNoCheck(x, y, direction); //wrap-around situation at the borders can't happen anymore
        }

        //TODO: use this for faster execution times for the "inner parts" of the cell field, e.g. in the "ClassicSimulation"
        public virtual bool GetCellValueNoCheck(int x, int y, Direction direction)
        {
            switch (direction)
            {
                case Direction.N: return GetCellValue(x, y - 1);
                case Direction.NE: return GetCellValue(x + 1, y - 1);
                case Direction.E: return GetCellValue(x + 1, y);
                case Direction.SE: return GetCellValue(x + 1, y + 1);
                case Direction.S: return GetCellValue(x, y + 1);
                case Direction.SW: return GetCellValue(x - 1, y + 1);
                case Direction.W: return GetCellValue(x - 1, y);
                case Direction.NW: return GetCellValue(x - 1, y - 1);
            }
            //throw new ArgumentException("Unknown direction: " + direction);
            return false; //probably faster?
        }

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
        public void CopyFromRegion(IBinaryCellField2D source, (int width, int height) dimension,
                    (int x, int y) srcUpperLeft, (int x, int y) dstUpperLeft, IBinaryCellField2D selfCopyBuffer = null)
        {
            if (source == this && Geom.OverlapsIn2D(srcUpperLeft, dstUpperLeft, dimension))
            {
                if (selfCopyBuffer == null || selfCopyBuffer.CellsX < dimension.width || selfCopyBuffer.CellsY < dimension.height)
                    selfCopyBuffer = new BoolArrayCellField2D(dimension.width, dimension.height);
                    //we could also tell the user that the copy buffer is not wide/high enough via ArgumentException
                (int x, int y) newUpperLeft = (0, 0);
                selfCopyBuffer.CopyFromRegion(source, dimension, srcUpperLeft, newUpperLeft);
                source = selfCopyBuffer;
                srcUpperLeft = newUpperLeft;
            }
            for (int x = 0; x < dimension.width; x++)
                for (int y = 0; y < dimension.height; y++)
                    CopyFromOther(source, srcUpperLeft.x + x, srcUpperLeft.y + y, dstUpperLeft.x + x, dstUpperLeft.y + y);
        }

        /// <inheritdoc/>
        public void CopyFromRegion(IBinaryCellField2D source, (int width, int height) dimension)
        {
            if (this == source) //this would be nonesense and we ignore it silently (or would an ArgumentException be better?)
                return;

            CopyFromRegion(source, dimension, (0, 0), (0, 0), null);
        }
    }
}
