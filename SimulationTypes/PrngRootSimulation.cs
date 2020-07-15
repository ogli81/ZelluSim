using System;
using System.Collections.Generic;
using System.Text;

namespace ZelluSim.SimulationTypes
{
    /// <summary>
    /// This sim is similar to the 'Classic' game of life simulation and 
    /// also has random numbers like those from the 'PRNG' simulation. Growth 
    /// is achieved along a "root" instead of having direct neighbors. The 
    /// root will grow if two or more neighbors form a "stick". <br></br>
    /// The simulation will only look at one of the 8 neighbors and detect
    /// a "stick" of at least two cells in one of the 8 directions. So, 
    /// basically it looks at one randomly chosen neighbor and will then 
    /// check one randomly chosen direction from that neighbor to one of its 
    /// 8 neighboring cells.
    /// <br></br>
    /// Similar simulations: <seealso cref="RootSimulation"/> and 
    /// <seealso cref="PrngSimulation"/>
    /// <br></br>
    /// <list type="number">
    /// <item>
    /// Param1: min number of neighbors to survive, default: 1
    /// </item>
    /// <item>
    /// Param2: min number of neighbors for overpopulation, default: 5
    /// </item>
    /// </list>
    /// </summary>
    class PrngRootSimulation
    {
    }
}
