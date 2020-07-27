using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZelluSim.CellField;
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

        protected virtual IBinaryCellField2D CreateCellField()
        {
            if (USE_BITARRAY)
                return new BitArrayCellField2D(Settings.SizeX, Settings.SizeY);
            else
                return new BoolArrayCellField2D(Settings.SizeX, Settings.SizeY);
        }

        protected override void CreateRingBuffer()
        {
            ring = new BinaryRingBuffer3D(Settings.MemSlots, CreateCellField());
            aring = ring;
        }

        protected override void DoResizeRingBuffer(int mem, int x, int y)
        {
            ring = new BinaryRingBuffer3D(mem, CreateCellField(), ring);
            aring = ring;
        }


        //public methods:

        public override decimal GetCellValue(int x, int y) => ring.Last[x, y] ? 1.0m : 0.0m;

        public override void SetCellValue(int x, int y, decimal val) { ring.Last[x,y] = val > 0.0m ? true : false; }
    }
}
