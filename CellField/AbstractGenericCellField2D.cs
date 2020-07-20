using System;
using System.Collections.Generic;
using System.Text;
using ZelluSim.Misc;

namespace ZelluSim.CellField
{
    public abstract class AbstractGenericCellField2D<T> : AbstractCellField2D
    {
        //state: 

        protected CloningPolicy cloningPolicy;


        //c'tors:

        public AbstractGenericCellField2D(int cellsX, int cellsY, 
            CloningPolicy cloningPolicy = CloningPolicy.DO_NOT_TRY_DEEP_CLONE) : base(cellsX, cellsY)
        {
            this.cloningPolicy = cloningPolicy;
            CreateArray(cellsX, cellsY);
        }

        public AbstractGenericCellField2D(int cellsX, int cellsY,
            bool tryDeepCopy = false) : base(cellsX, cellsY)
        {
            cloningPolicy = tryDeepCopy ? CloningPolicy.TRY_DEEP_CLONE : CloningPolicy.DO_NOT_TRY_DEEP_CLONE;
            CreateArray(cellsX, cellsY);
        }


        //helper methods:

        protected abstract bool DetectCloneables();

        protected abstract void CreateArray(int cellsX, int cellsY);


        //public methods:

        /// <inheritdoc/>
        public abstract void SetAllCells(T value, bool tryDeepCopy = false);

        /// <inheritdoc/>
        public T this[int x, int y]
        {
            get => GetCellValue(x, y);
            set => SetCellValue(x, y, value);
        }

        /// <inheritdoc/>
        public abstract T GetCellValueWithWrap(int x, int y, Direction direction);

        /// <inheritdoc/>
        public T GetCellValueWithoutWrap(int x, int y, Direction direction, T outsideVal)
        {
            //BoundsCheck(x, y); //the array will do its own bounds check
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
            return GetCellValueWithWrap(x, y, direction); //wrap situation (border) can't happen anymore
        }

        /// <inheritdoc/>
        public abstract T GetCellValue(int x, int y);

        /// <inheritdoc/>
        public abstract void SetCellValue(int x, int y, T value);

        /// <inheritdoc/>
        public void ClearAllWithDefault()
        {
            SetAllCells(default);
        }

        /// <inheritdoc/>
        public CloningPolicy CloningPolicy
        {
            get => cloningPolicy;
            set
            {
                switch (value)
                {
                    case CloningPolicy.TRY_DEEP_CLONE:
                        cloningPolicy = CloningPolicy.TRY_DEEP_CLONE;
                        return;
                    case CloningPolicy.DO_NOT_TRY_DEEP_CLONE:
                        cloningPolicy = CloningPolicy.DO_NOT_TRY_DEEP_CLONE;
                        return;
                    case CloningPolicy.USE_DEFAULT:
                        cloningPolicy = CloningPolicy.USE_DEFAULT;
                        return;
                    case CloningPolicy.AUTO_DETECT:
                        if (DetectCloneables())
                            cloningPolicy = CloningPolicy.TRY_DEEP_CLONE;
                        else
                            cloningPolicy = CloningPolicy.DO_NOT_TRY_DEEP_CLONE;
                        return;
                }
            }
        }

        /// <inheritdoc/>
        public void CloneIntoOther(int fromThisX, int fromThisY, IGenericCellField2D<T> other, int toOtherX, int toOtherY)
        {
            switch (CloningPolicy)
            {
                case CloningPolicy.TRY_DEEP_CLONE:
                    T element = this[fromThisX, fromThisY];
                    other[toOtherX, toOtherY] = element is ICloneable cloneable ? (T)cloneable.Clone() : element;
                    return;
                case CloningPolicy.DO_NOT_TRY_DEEP_CLONE:
                case CloningPolicy.USE_DEFAULT:
                    other[toOtherX, toOtherY] = this[fromThisX, fromThisY];
                    return;
                case CloningPolicy.AUTO_DETECT:
                    DetectCloneables();
                    CloneIntoOther(fromThisX, fromThisY, other, toOtherX, toOtherY);
                    return;
            }
        }

        /// <inheritdoc/>
        public void CloneFromOther(IGenericCellField2D<T> other, int fromOtherX, int fromOtherY, int toThisX, int toThisY)
        {
            switch (CloningPolicy)
            {
                case CloningPolicy.TRY_DEEP_CLONE:
                    T element = other[fromOtherX, fromOtherY];
                    //TODO: which of the following three lines is the fastest? (do performance comparison)
                    //this[toThisX, toThisY] = element is ICloneable ? (T)(element as ICloneable).Clone() : element;
                    //this[toThisX, toThisY] = element is ICloneable ? (T)((ICloneable)element).Clone() : element;
                    this[toThisX, toThisY] = element is ICloneable cloneable ? (T)cloneable.Clone() : element;
                    return;
                case CloningPolicy.DO_NOT_TRY_DEEP_CLONE:
                case CloningPolicy.USE_DEFAULT:
                    this[toThisX, toThisY] = other[fromOtherX, fromOtherY];
                    return;
                case CloningPolicy.AUTO_DETECT:
                    DetectCloneables();
                    CloneFromOther(other, fromOtherX, fromOtherY, toThisX, toThisY);
                    return;
            }
        }
    }
}
