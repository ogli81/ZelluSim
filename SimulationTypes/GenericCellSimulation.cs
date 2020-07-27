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
    /// Abstract base class for all "generic" cell simulations, meaning we have a type 'C' for the 
    /// content of a cell in our cell field.
    /// </summary>
    /// <typeparam name="C">the generic type of the value inside a cell</typeparam>
    public abstract class GenericCellSimulation<C> : AbstractCellSimulation<IGenericCellField2D<C>>
    {
        //constants:

        static readonly bool USE_MESHEDCELLS = false;


        //state:

        protected GenericRingBuffer3D<C> ring;


        //c'tors:

        public GenericCellSimulation(SimulationSettings settings) : base(settings)
        {
        }


        //helper methods:

        protected virtual IGenericCellField2D<C> CreateCellField()
        {
            if (USE_MESHEDCELLS)
                return new MeshedCellField2D<C>(Settings.SizeX, Settings.SizeY);
            else
                return new SimpleCellField2D<C>(Settings.SizeX, Settings.SizeY);
        }

        protected override void CreateRingBuffer()
        {
            ring = new GenericRingBuffer3D<C>(Settings.MemSlots, CreateCellField());
            aring = ring;
        }

        protected override void DoResizeRingBuffer(int mem, int x, int y)
        {
            ring = new GenericRingBuffer3D<C>(mem, CreateCellField(), ring);
            aring = ring;
        }


        //public methods:

        //-
    }
}
