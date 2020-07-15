using System;
using System.Collections.Generic;
using System.Text;

namespace ZelluSim.SimulationTypes
{
    /// <summary>
    /// This sim is similar to the 'Classic' game of life simulation. Growth 
    /// is achieved along a "root" instead of having direct neighbors. The 
    /// root will grow if two or more neighbors form a "stick". So consider 
    /// the following example of a cell field:
    /// <code>
    /// //a "stick" consisting of two neighbors:<br></br>
    /// [ ][ ][ ][ ]<br></br>
    /// [ ][X][X][ ]<br></br>
    /// [ ][ ][ ][ ]<br></br>
    /// <br></br>
    /// //leads to growth in the cells next to that "stick":<br></br>
    /// [ ][ ][ ][ ]<br></br>
    /// [X][X][X][X]<br></br>
    /// [ ][ ][ ][ ]<br></br>
    /// <br></br>
    /// //these "sticks" might also be diagonal<br></br>
    /// </code>
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
    class RootSimulation
    {
    }
}
