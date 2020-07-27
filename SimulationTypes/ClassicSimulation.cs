using System;
using ZelluSim.CellField;

namespace ZelluSim.SimulationTypes
{
    /// <summary>
    /// This is the classic "Game of Life" simulation by John Horton Conway.
    /// The 'Classic' simulation uses a binary state for each cell in the 
    /// cell field which means that cells are either "fully alive" (true) or 
    /// "fully dead" (false). The cell field (or "world") is 2-dimensional 
    /// and usually "wraps around" at its borders (if you are at the east
    /// border and go one step easterly, you end in the same row but in the 
    /// west). <br></br>
    /// You can find the original rules for "Conway's Game of Life" at 
    /// several places (for example at Wikipedia). Our simulation allows 
    /// you to change the rules (see the two parameters below).
    /// <list type="number">
    /// <item>
    /// Param1: min number of neighbors to survive, default: 2
    /// </item>
    /// <item>
    /// Param2: min number of neighbors for overpopulation, default: 4
    /// </item>
    /// </list>
    /// </summary>
    public class ClassicSimulation : BinaryCellSimulation
    {
        //state:

        //-


        //c'tors:

        public ClassicSimulation(SimulationSettings settings) : base(settings)
        {
            Init();
        }


        //helper methods:

        protected override void CreateParams()
        {
            param1 = new SimulationParameter("minimum survival", "min number of neighbors to survive, default: 2", 0, 8, 2);
            param2 = new SimulationParameter("overpopulation", "min number of neighbors for overpopulation, default: 4", 1, 9, 4);
        }

        protected override void DoCalculateNextGen()
        {
            //hint: the new generation was already allocated in AbstractCellSimulation
            IBinaryCellField2D last = ring.Last;
            IBinaryCellField2D prev = ring.Previous;

            int minSurv = (int)param1.Current;
            int overPop = (int)param2.Current;
            int neighbors;
            bool wrap = Settings.IsWrap;

            for (int x = 0; x < prev.CellsX; ++x)
                for (int y = 0; y < prev.CellsY; ++y)
                {
                    GetNeighbors(prev, x, y, out neighbors, wrap);
                    if (prev[x, y]) //is alive
                    {
                        if (neighbors < minSurv)
                            last[x, y] = false; //die
                        else
                        if (neighbors >= overPop)
                            last[x, y] = false; //die
                        else
                            last[x, y] = true; //stay alife
                    }
                    else //is dead
                    {
                        if (neighbors == 3)
                            last[x, y] = true; //get born
                        else
                            last[x, y] = false; //stay dead
                    }
                }
        }

        protected override void Param1Changed(object sender, EventArgs e)
        {
            if (param1.Current >= param2.Current)
                param2.Current = param1.Current + 1;
        }

        protected override void Param2Changed(object sender, EventArgs e)
        {
            if (param2.Current <= param1.Current)
                param1.Current = param2.Current - 1;
        }


        //public methods:

        public override string Info => "The classic game of life simulation by John Horton Conway - simulates birth and death of a population of micro organisms.";
    }
}
