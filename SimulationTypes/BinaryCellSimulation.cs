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

        protected override decimal GetCellValue(IBinaryCellField2D cellfield2d, int x, int y) => cellfield2d[x, y] ? 1.0m : 0.0m;

        protected override void SetCellValue(IBinaryCellField2D cellfield2d, int x, int y, decimal val) { cellfield2d[x, y] = val > 0.0m ? true : false; }


        //public methods:

        //-
    }
}
