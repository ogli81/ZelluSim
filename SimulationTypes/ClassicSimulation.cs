using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

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
        public ClassicSimulation(SimulationSettings settings) : base(settings)
        {
            InjectSettings();
        }

        public override string Info => "The classic game of life simulation by John Horton Conway - simulates birth and death of a population of micro organisms.";

        protected override bool DoCalculateNextGen()
        {
            //hint: the new generation was already allocated in AbstractCellSimulation
            BitArray prev = ring.Previous;
            BitArray last = ring.Last;


        }
    }



}
