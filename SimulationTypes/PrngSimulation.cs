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
    /// </summary>
    public class PrngSimulation // : ISimulation
    {
    }
}
