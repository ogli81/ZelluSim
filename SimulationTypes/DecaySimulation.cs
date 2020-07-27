using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZelluSim.CellField;
using ZelluSim.Misc;

namespace ZelluSim.SimulationTypes
{
    /// <summary>
    /// A decay simulation with variable decay rate. If a cell has neighbors, the decay is reduced or even stopped.
    /// <list type="bullet">
    /// <item>
    /// Param1: how quickly will decay take place (value per second)
    /// </item>
    /// <item>
    /// Param2: each living neighbor reduces the decay rate by ...
    /// </item>
    /// </list>
    /// </summary>
    public class DecaySimulation : GenericCellSimulation<byte>
    {
        //state:

        //-


        //c'tors:

        public DecaySimulation(SimulationSettings settings) : base(settings)
        {
            Init();
        }


        //helper methods:

        protected override void CreateParams()
        {
            param1 = new SimulationParameter("rate of decay", "how quickly will decay take place (value per second)", 1, 255, 10);
            param2 = new SimulationParameter("stabilization", "each living neighbor reduces the decay rate by ...", 0, 255, 3);
        }

        protected override void DoCalculateNextGen()
        {
            //hint: the new generation was already allocated in AbstractCellSimulation
            IGenericCellField2D<byte> last = ring.Last;
            IGenericCellField2D<byte> prev = ring.Previous;

            byte decayRate = (byte)param1.Current;
            byte reduction = (byte)param2.Current;
            byte decayHere;
            int reduceBy;
            bool wrap = Settings.IsWrap;
            int xBound = prev.CellsX;
            int yBound = prev.CellsY;

            for(int x = 0; x < xBound; ++x)
                for (int y = 0; y < yBound; ++y)
                {
                    if (prev[x, y] == 0) //was dead
                        last[x, y] = 0; //stays dead
                    else
                    {
                        GetNeighbors(prev, x, y, out int neighbors, wrap);
                        decayHere = 0;
                        decayHere += decayRate;
                        reduceBy = neighbors * reduction;
                        decayHere -= (reduceBy >= decayHere) ? decayHere : (byte)reduceBy;
                        if (decayHere > 0)
                            last[x, y] = (decayHere >= prev[x, y]) ? (byte)0 : (byte)(prev[x, y] - decayHere);
                        else
                            last[x, y] = prev[x, y];
                    }
                }
        }

        protected int GetNeighbors(IGenericCellField2D<byte> cells, int x, int y, out int n, bool wrap)
        {
            return wrap ? GetNeighborsWithWrap(cells, x, y, out n) : GetNeighborsWithoutWrap(cells, x, y, out n);
        }

        protected int GetNeighborsWithWrap(IGenericCellField2D<byte> cells, int x, int y, out int n)
        {
            n = 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.N) > 0 ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.NE) > 0 ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.E) > 0 ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.SE) > 0 ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.S) > 0 ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.SW) > 0 ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.W) > 0 ? 1 : 0;
            n += cells.GetCellValueWithWrap(x, y, Direction.NW) > 0 ? 1 : 0;
            return n;
        }

        protected int GetNeighborsWithoutWrap(IGenericCellField2D<byte> cells, int x, int y, out int n)
        {
            n = 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.N, 0) > 0 ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.NE, 0) > 0 ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.E, 0) > 0 ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.SE, 0) > 0 ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.S, 0) > 0 ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.SW, 0) > 0 ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.W, 0) > 0 ? 1 : 0;
            n += cells.GetCellValueWithoutWrap(x, y, Direction.NW, 0) > 0 ? 1 : 0;
            return n;
        }

        //public methods:

        public override string Info => "A decay simulation with variable decay rate. If a cell has neighbors, the decay is reduced or even stopped.";

        public override decimal GetCellValue(int x, int y)
        {
            return ring.Last[x, y] / 255m;
        }

        public override void SetCellValue(int x, int y, decimal val)
        {
            val = val > 1m ? 1m : val;
            val = val < 0m ? 0m : val;
            ring.Last[x, y] = (byte)(255m * val);
        }
    }
}
