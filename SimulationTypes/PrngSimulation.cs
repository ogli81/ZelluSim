using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZelluSim.SimulationTypes
{
    /// <summary>
    /// This sim is similar to the 'Classic' game of life simulation and
    /// also has random numbers, provided by a PRNG (Pseudo Random Numbers 
    /// Generator). The current internal state of the PRNG will be saved 
    /// when this simulation is saved and it will be loaded when this 
    /// simulation is loaded. This is done to always achieve the same 
    /// sequence of generations - so it's a deterministic algorithm and 
    /// the randomness is only pseudo-randomness that stems from the
    /// same 'seed' value for the PRNG.
    /// <br></br>
    /// <list type="number">
    /// <item>
    /// Param1: min number of neighbors to survive, default: 2
    /// </item>
    /// <item>
    /// Param2: min number of neighbors for overpopulation, default: 4
    /// </item>
    /// </list>
    /// HINT:
    /// How is the randomness applied to th GoL-simulation? 
    /// We could examine 6 random cells instead of 8 fixed cells. 
    /// We could also vary the parameters (e.g. -1, +0 or +1) for each cell.
    /// We could also vary the number of 3 cells needed for being born (e.g. 
    /// 10% for "1 needed", 20% for "2 needed", 40% for "3 needed", 
    /// 20% for "4 needed" and 10% for "1 needed").
    /// </summary>
    public class PrngSimulation // : ISimulation
    {
    }
}
