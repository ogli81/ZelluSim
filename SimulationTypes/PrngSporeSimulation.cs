using System;
using System.Collections.Generic;
using System.Text;

namespace ZelluSim.SimulationTypes
{
    /// <summary>
    /// This sim is similar to the 'Growth' simulation. Growth is achieved by 
    /// letting fly some "spores" instead of having direct neighbors. The 
    /// spores will be carried by the wind and will fall down in a circular 
    /// region. Landed Spores will never starve but might die due to 
    /// overpopulation. Spores will grow like in the 'Growth' simulation. 
    /// <br></br>
    /// Randomness No.1: The wind from the outside will change direction over
    /// time (more rapid changes by higher wind speeds). 
    /// <br></br>
    /// Randomness No.2: The spreading pattern of the spoors will not always 
    /// be a perfect circle (it's more like a spray can in this simulation).
    /// <br></br>
    /// <list type="number">
    /// <item>
    /// Param1: wind direction ([0°..360°], clockwise rotation), default: 0°
    /// </item>
    /// <item>
    /// Param2: wind speed ([0m/s..100m/s]), default: 10m/s
    /// <list type="bullet">
    /// <item>more speed = spores land in a more distant position</item>
    /// <item>more speed = circular region's diameter is bigger</item>
    /// <item>big circular region = spores are widely scattered</item>
    /// <item>small circular region = spores are close to each other</item>
    /// </list>
    /// </item>
    /// </list>
    /// </summary>
    class PrngSporeSimulation
    {
    }
}
