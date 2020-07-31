using System;
using System.Collections.Generic;
using System.Text;

namespace ZelluSim.SimulationTypes
{
    /// <summary>
    /// This simulation is about a population of hunters and their prey and 
    /// the food that they desire. Plants grow, prey eats plants, hunters 
    /// eat prey. Those that aren't hungry, will spawn babies. Those that are 
    /// too hungry lose health. Those that lose too much health die.
    /// <br></br>
    /// The "life" value of a cell will have following meaning:
    /// <list type="bullet">
    /// <item>a value between 0 and 0.25 (inclusive) means that plants grow (0 = seeds in the ground, 0.25 = fully grown)</item>
    /// <item>a value > 0.25 and <= 0.5 means that prey lives here (0.5 = 100% health, 0.25 = 0% health)</item>
    /// <item>a value > 0.5 and <= 1.0 means that hunter lives here (1.0 = 100% health, 0.5 = 0% health)</item>
    /// </list>
    /// <list type="number">
    /// <item>
    /// Param1: fertility/climate, higher values of fertility have these effects:
    /// <list type="bullet">
    /// <item>plants grow faster</item>
    /// <item>healthy hunters/prey will spawn more offspring</item>
    /// <item>offspring will have more health from the start</item>
    /// </list>
    /// </item>
    /// <item>
    /// Param2: travel pattern:
    /// <list type="number">
    /// <item>hunters 3 from West to East, prey 2 from North to South</item>
    /// <item>spirals (that regularly start anew) for hunters and prey</item>
    /// <item>repeating pattern of prime numbers for both direction and steps</item>
    /// <item>individuals chose their own strategy and keep their strategies</item>
    /// <item>individuals chose their own strategy and change it from time to time</item>
    /// </list>
    /// </item>
    /// </list>
    /// </summary>
    public class HunterPreySimulation
    {
    }
}
