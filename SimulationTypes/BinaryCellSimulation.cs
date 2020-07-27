using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZelluSim.CellField;
using ZelluSim.Misc;
using ZelluSim.RingBuffer;

namespace ZelluSim.SimulationTypes
{
    /// <summary>
    /// A 'BinaryCellSimulation' is one with only two possible states for each cell: 
    /// true and false (or 1 and 0) meaning true=alife and false=dead.
    /// </summary>
    public abstract class BinaryCellSimulation : AbstractCellSimulation<IBinaryCellField2D>
    {
        //constants:

        static readonly bool USE_BITARRAY = true;


        //state:

        protected BinaryRingBuffer3D ring;


        //c'tors:

        public BinaryCellSimulation(SimulationSettings settings) : base(settings)
        {
        }


        //helper methods:

        protected virtual IBinaryCellField2D CreateCellField(int sizeX, int sizeY)
        {
            if (USE_BITARRAY)
                return new BitArrayCellField2D(sizeX, sizeY);
            else
                return new BoolArrayCellField2D(sizeX, sizeY);
        }

        protected override void CreateRingBuffer()
        {
            ring = new BinaryRingBuffer3D(Settings.MemSlots, CreateCellField(Settings.SizeX, Settings.SizeY));
            aring = ring;
        }

        protected override void DoResizeRingBuffer(int mem, int x, int y)
        {
            if (ring.CellsX == x && ring.CellsY == y)
            {
                ring = new BinaryRingBuffer3D(mem, ring);
            }
            else
            {
                ring = new BinaryRingBuffer3D(mem, CreateCellField(x, y), ring);
            }
            aring = ring;
        }

        protected int GetNeighbors(IBinaryCellField2D cells, int x, int y, out int n, bool wrap)
        {
            return wrap ? GetNeighborsWithWrap(cells, x, y, out n) : GetNeighborsWithoutWrap(cells, x, y, out n);
        }

        protected int GetNeighborsWithWrap(IBinaryCellField2D cells, int x, int y, out int n)
        {
            n = 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.N) ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.NE) ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.E) ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.SE) ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.S) ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.SW) ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.W) ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.NW) ? 1 : 0;
            return n;
        }

        protected int GetNeighborsWithoutWrap(IBinaryCellField2D cells, int x, int y, out int n)
        {
            n = 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.N, false) ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.NE, false) ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.E, false) ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.SE, false) ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.S, false) ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.SW, false) ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.W, false) ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.NW, false) ? 1 : 0;
            return n;
        }


        //public methods:

        public override decimal GetCellValue(int x, int y) => ring.Last[x, y] ? 1.0m : 0.0m;

        public override void SetCellValue(int x, int y, decimal val) { ring.Last[x,y] = val > 0.0m ? true : false; }
    }
}
