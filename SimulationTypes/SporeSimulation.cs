using System;
using System.Collections.Generic;
using System.Text;

namespace ZelluSim.SimulationTypes
{
    /// <summary>
    /// This sim is similar to the 'Fuzzy11' variant of the 'Classic' game 
    /// of life simulation. Growth is achieved by letting fly some "spores" 
    /// instead of having direct neighbors. The spores will be carried by
    /// the wind and will fall down in a circular region. Landed Spores will 
    /// never starve but might die due to overpopulation. Spores will grow
    /// by +1 for each simulation step.
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
    public class SporeSimulation
    {
    }
}
