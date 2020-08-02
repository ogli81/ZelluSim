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
    /// <list type="bullet">
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

        //// used this to debug a nasty bug:
        //protected void ShowInfo(IBinaryCellField2D what)
        //{
        //    Console.WriteLine("" + what[0, 0] + what[0, 1] + what[0, 2]);
        //    Console.WriteLine("" + what[1, 0] + what[1, 1] + what[1, 2]);
        //    Console.WriteLine("" + what[2, 0] + what[2, 1] + what[2, 2]);
        //    Console.WriteLine();
        //    Console.WriteLine("N:" + what.GetCellValueWithWrap(1, 1, Misc.Direction.N));
        //    Console.WriteLine("NE:" + what.GetCellValueWithWrap(1, 1, Misc.Direction.NE));
        //    Console.WriteLine("E:" + what.GetCellValueWithWrap(1, 1, Misc.Direction.E));
        //    Console.WriteLine("SE:" + what.GetCellValueWithWrap(1, 1, Misc.Direction.SE));
        //    Console.WriteLine("S:" + what.GetCellValueWithWrap(1, 1, Misc.Direction.S));
        //    Console.WriteLine("SW:" + what.GetCellValueWithWrap(1, 1, Misc.Direction.SW));
        //    Console.WriteLine("W:" + what.GetCellValueWithWrap(1, 1, Misc.Direction.W));
        //    Console.WriteLine("NW:" + what.GetCellValueWithWrap(1, 1, Misc.Direction.NW));
        //    int exp = 0;
        //    exp += what[0, 0] ? 1 : 0;
        //    exp += what[1, 0] ? 1 : 0;
        //    exp += what[2, 0] ? 1 : 0;
        //    exp += what[0, 1] ? 1 : 0;
        //    exp += what[1, 1] ? 1 : 0;
        //    exp += what[2, 1] ? 1 : 0;
        //    exp += what[0, 2] ? 1 : 0;
        //    exp += what[1, 2] ? 1 : 0;
        //    exp += what[2, 2] ? 1 : 0;
        //    Console.WriteLine("expected: " + exp);
        //    Console.WriteLine("but we get:");
        //    Console.WriteLine(CellFields.GetNumNeighbors(what, 1, 1, true));
        //    Console.WriteLine(CellFields.GetNumNeighbors(what, 1, 1, false));
        //    Console.ReadKey();
        //}

        protected override void DoCalculateNextGen()
        {
            //hint: the new generation was already allocated in AbstractCellSimulation
            IBinaryCellField2D last = ring.Last;
            IBinaryCellField2D prev = ring.Previous;

            //ShowInfo(prev);

            int minSurv = (int)param1.Current;
            int overPop = (int)param2.Current;
            int neighbors;
            bool wrap = Settings.IsWrap;

            for (int x = 0; x < prev.CellsX; ++x)
                for (int y = 0; y < prev.CellsY; ++y)
                {
                    neighbors = CellFields.GetNumNeighbors(prev, x, y, wrap);
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

            //Console.WriteLine();
            //ShowInfo(last);
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
