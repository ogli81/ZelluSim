using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZelluSim.RingBuffer
{
    /// <summary>
    /// A 3-dimensional ringbuffer (stores a '2-dimensional cell field' with a 'memory of the past'). 
    /// </summary>
    public abstract class AbstractRingBuffer3D<T> : GenericRingBuffer1D<T> // : AbstractRingBuffer
    {
        //state:

        //-


        /// <summary>
        /// The number of cells in x direction (2nd dim).
        /// </summary>
        public int CellsX { get; }

        /// <summary>
        /// The number of cells in y direction (3rd dim).
        /// </summary>
        public int CellsY { get; }


        //c'tors:

        /// <summary>
        /// Make a new instance (first c'tor variant).
        /// </summary>
        /// <param name="mem">number of memory slots in the ring buffer (1st dimension)</param>
        /// <param name="x">number of cells in x direction (2nd dimension)</param>
        /// <param name="y">number of cells in y direction (3rd dimension)</param>
        public AbstractRingBuffer3D(int mem, int x, int y) : base(mem)
        {
            SafetyCheckNewRingBuffer(x, y);
            CellsX = x;
            CellsY = y;
        }

        /// <summary>
        /// Make a new instance, copy from other instance (copy c'tor A, second c'tor variant). 
        /// Makes a copy of the other ring buffer (but only copies useful data, to safe time).
        /// </summary>
        /// <param name="other">the other instance</param>
        /// <param name="tryDeepCopy">if the values are references that implement ICloneable then set this to true</param>
        /// <param name="copyUnusedElements">if you are recycling old elements, set this parameter to true</param>
        public AbstractRingBuffer3D(AbstractRingBuffer3D<T> other, bool tryDeepCopy = true, 
            bool copyUnusedElements = false) : base(other, tryDeepCopy, copyUnusedElements)
        {
            SafetyCheckNewRingBuffer(other.CellsX, other.CellsY);
            CellsX = other.CellsX;
            CellsY = other.CellsY;
        }

        /// <summary>
        /// Make a new instance, copy from other instance (copy c'tor B, third c'tor variant). 
        /// Tries to grab as much data from the other instance as possible.
        /// </summary>
        /// <param name="mem">number of memory slots in the ring buffer (1st dimension)</param>
        /// <param name="other">the other instance</param>
        /// <param name="startHere">do we start copying at the "leftmost" or "rightmost" end?</param>
        /// <param name="tryDeepCopy">if the values are references that implement ICloneable then set this to true</param>
        /// <param name="copyUnusedElements">if you are recycling old elements, set this parameter to true</param>
        public AbstractRingBuffer3D(int mem, AbstractRingBuffer3D<T> other,
            RingBufferEnd startHere = RingBufferEnd.RIGHTMOST_LAST_NEWEST, bool tryDeepCopy = true, 
            bool copyUnusedElements = false) : base(mem, other, startHere, tryDeepCopy, copyUnusedElements)
        {
            SafetyCheckNewRingBuffer(other.CellsX, other.CellsY);
            CellsX = other.CellsX;
            CellsY = other.CellsY;
        }


        //helper methods:

        /// <summary>
        /// Will throw an exception if any of these values violates our rules. 
        /// The rules are as follows: <br></br>
        /// 'x' and 'y' should be >= 1
        /// </summary>
        /// <param name="x">number of cells in x direction (2nd dimension)</param>
        /// <param name="y">number of cells in y direction (3rd dimension)</param>
        private void SafetyCheckNewRingBuffer(int x, int y)
        {
            if (x < 1) throw new ArgumentException("x can't be less than 1!");
            if (y < 1) throw new ArgumentException("y can't be less than 1!");
        }

        //public methods:
    
        //-
    }
}
