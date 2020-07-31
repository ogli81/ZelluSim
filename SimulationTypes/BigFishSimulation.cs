using System;
using System.Collections.Generic;
using System.Text;

namespace ZelluSim.SimulationTypes
{
    /// <summary>
    /// This sim initially contains many fishes. Over time, the bigger fishes will consume the smaller ones 
    /// and the bigger fish will grow in size after its meal. Near the end of the simulation only a few very 
    /// big fishes will remain. 
    /// <br></br>
    /// The "life value" of a single fish will be the square root of its actual size value. So, a fish with
    /// a life value of 0.01 will produce a cell value of 0.1 and a fish with a life value of 0.25 will produce
    /// a cell value of 0.5. The smallest fishes will have a value of around 0.01 and the biggest one of 1.0.
    /// <list type="number">
    /// <item>
    /// Param1: relative swimming speed(1.0 = "slow movement" / 4.0 = "fast movement"), small fishes will swim faster
    /// </item>
    /// <item>
    /// Param2: travel pattern:
    /// <list type="number">
    /// <item>straight line (main direction depends on the cell where the fish started the simulation)</item>
    /// <item>zig-zag/sine-wave-like pattern (main direction changes regularly)</item>
    /// <item>spirals (that regularly start anew)</item>
    /// <item>circles (with regular changes of center, center is now where the fish currently is)</item>
    /// <item>repeating pattern of prime numbers for both direction and steps</item>
    /// <item>individuals chose their own strategy and keep their strategies</item>
    /// <item>individuals chose their own strategy and change it from time to time</item>
    /// </list>
    /// </item>
    /// </list>
    /// </summary>
    public class BigFishSimulation //:
    {
    }
}
