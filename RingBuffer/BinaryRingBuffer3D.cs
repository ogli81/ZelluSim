using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZelluSim.CellField;
using ZelluSim.Misc;

namespace ZelluSim.RingBuffer
{
    /// <summary>
    /// A 3-dimensional ringbuffer (stores a '2-dimensional cell field' with a 'memory of the past'). 
    /// This version of the 3-dimensional ring buffer idea uses a BitArray (just a huge series of bits, 
    /// each representing the content of a cell in our cell field).<br></br>
    /// (IDEA for later: We might want to save even more memory space by using some kind of in-memory compression)
    /// </summary>
    public class BinaryRingBuffer3D : AbstractRingBuffer3D<IBinaryCellField2D>
    {
        //state:

        protected IBinaryCellField2D template;
        protected IBinaryCellField2D templateWithDefault;


        //c'tors:
        /// <summary>
        /// Make a new instance (first c'tor variant).
        /// </summary>
        /// <param name="mem">number of memory slots in the ring buffer (1st dimension)</param>
        /// <param name="template">we will create clones of this template</param>
        public BinaryRingBuffer3D(int mem, IBinaryCellField2D template) : base(mem, template.CellsX, template.CellsY)
        {
            SafetyCheckNewRingBuffer(template);

            IBinaryCellField2D template1 = (IBinaryCellField2D)template.Clone();
            IBinaryCellField2D template2 = (IBinaryCellField2D)template.Clone();
            template2.SetAllCells(false);
            AcceptTemplates(template1, template2);

            //earlier idea: reserve memory now for the entire ring buffer
            //current idea: reserve memory as soon as it is needed
        }

        /// <summary>
        /// Make a new instance, copy from other instance (copy c'tor A, second c'tor variant). 
        /// Makes a copy of the other ring buffer If you are recycling unused elements, you might be 
        /// interested in setting the second param to true.
        /// </summary>
        /// <param name="other">the other instance</param>
        /// <param name="copyUnusedElements">if you are recycling old elements, set this parameter to true</param>
        public BinaryRingBuffer3D(BinaryRingBuffer3D other,
            bool copyUnusedElements = false) : base(other, true, copyUnusedElements)
        {
            //no safety check needed, since the other must have undergone that check

            AcceptTemplates((IBinaryCellField2D)other.template.Clone(),
                (IBinaryCellField2D)other.templateWithDefault.Clone());
        }

        /// <summary>
        /// Make a new instance, copy from other instance (copy c'tor B, third c'tor variant). 
        /// Tries to grab as much data from the other instance as possible.
        /// </summary>
        /// <param name="mem">number of memory slots in the ring buffer (1st dimension)</param>
        /// <param name="other">the other instance</param>
        /// <param name="startHere">do we start copying at the "leftmost" or "rightmost" end?</param>
        /// <param name="copyUnusedElements">if you are recycling old elements, set this parameter to true</param>
        public BinaryRingBuffer3D(int mem, BinaryRingBuffer3D other,
            RingBufferEnd startHere = RingBufferEnd.RIGHTMOST_LAST_NEWEST,
            bool copyUnusedElements = false) : this(mem, other.template, other, startHere, copyUnusedElements)
        {

        }

        /// <summary>
        /// Make a new instance, copy from other instance (copy c'tor C, fourth c'tor variant). 
        /// Tries to grab as much data from the other instance as possible.
        /// </summary>
        /// <param name="template">we will create clones of this template</param>
        /// <param name="other">the other instance</param>
        /// <param name="startHere">do we start copying at the "leftmost" or "rightmost" end?</param>
        /// <param name="copyUnusedElements">if you are recycling old elements, set this parameter to true</param>
        public BinaryRingBuffer3D(IBinaryCellField2D template, BinaryRingBuffer3D other,
            RingBufferEnd startHere = RingBufferEnd.RIGHTMOST_LAST_NEWEST,
            bool copyUnusedElements = false) : this(other.MemSlots, template, other, startHere, copyUnusedElements)
        {

        }

        /// <summary>
        /// Make a new instance, copy from other instance (copy c'tor D, fifth c'tor variant). 
        /// Tries to grab as much data from the other instance as possible.
        /// </summary>
        /// <param name="mem">number of memory slots in the ring buffer (1st dimension)</param>
        /// <param name="template">we will create clones of this template</param>
        /// <param name="other">the other instance</param>
        /// <param name="startHere">do we start copying at the "leftmost" or "rightmost" end?</param>
        /// <param name="copyUnusedElements">if you are recycling old elements, set this parameter to true</param>
        public BinaryRingBuffer3D(int mem, IBinaryCellField2D template, BinaryRingBuffer3D other,
            RingBufferEnd startHere = RingBufferEnd.RIGHTMOST_LAST_NEWEST,
            bool copyUnusedElements = false) : this(mem, template)
        {
            if (other.Empty)
            {
                firstPos = other.firstPos;
                lastPos = other.lastPos;
                empty = true;

                if (!copyUnusedElements)
                    return;
            }

            //we copy as many from the active entries from the other ring buffer as will fit into this new ring buffer
            //start at the desired end, until mem is full

            int iMax = copyUnusedElements ? other.MemSlots : other.Length;

            //use top-left corner (0,0), work into x and y direction as far as we can
            //if this new ringbuffer is bigger than the other: fill with 'default value'

            int xBound = Math.Min(CellsX, other.CellsX);
            int yBound = Math.Min(CellsY, other.CellsY);

            //this code can also be found in a base class (with some slight modifications)
            //I tried to have it all in one place, but the resulting code was too ugly

            int add;
            if (startHere == RingBufferEnd.RIGHTMOST_LAST_NEWEST) //go from rightmost to leftmost (down)
                add = -1;
            else //go from leftmost to rightmost (up)
                add = +1;

            int iother = (add == -1) ? other.lastPos : other.firstPos;
            int ithis = (add == -1) ? MemSlots - 1 : 0;
            int ithisEnd = (add == -1) ? 0 : MemSlots - 1;
            int iotherWrap = (add == -1) ? -1 : other.MemSlots;
            if (add == -1) lastPos = ithis; else firstPos = ithis;
            for (int i = 0; i < iMax; i++)
            {
                CloneCopyEntry(ithis, iother, other, xBound, yBound);

                if (ithis == ithisEnd)
                    break;
                ithis += add;

                iother += add;
                if (iother == iotherWrap)
                    iother = other.MemSlots - 1;
            }
            if (add == -1) firstPos = ithis; else lastPos = ithis;
        }


        //helper methods:

        private void AcceptTemplates(IBinaryCellField2D template, IBinaryCellField2D templateWithDefault)
        {
            //not much to do here, but we want to stay consistent and make it like in GenericRingBuffer3D
            this.template = template;
            this.templateWithDefault = templateWithDefault;
        }

        private void CloneCopyEntry(int ithis, int iother, BinaryRingBuffer3D other, int xBound, int yBound)
        {
            IBinaryCellField2D oarr = other.ringBuffer[iother];
            if (oarr == null)
            {
                ringBuffer[ithis] = null;
            }
            else
            {
                MakeEntry(ithis, true);
                IBinaryCellField2D arr = ringBuffer[ithis];

                //for (int a = 0; a < xBound; a++)
                //    for (int b = 0; b < yBound; b++)
                //        arr.CopyFromOther(oarr, a, b, a, b);
                arr.CopyFromRegion(oarr, UPPER_LEFT, (xBound, yBound), UPPER_LEFT);
                //TODO: which of these two ways is faster?
            }
        }

        protected override void MakeEntry(int where, bool clearWithDefault)
        {
            //TODO: which way is faster?

            if (clearWithDefault || ringBuffer[where] == null)
                ringBuffer[where] = CreateCellField2D(clearWithDefault);
            //this way also puts a bit more pressure on the GC

            //if (ringBuffer[where] == null)
            //    ringBuffer[where] = CreateCellField2D(clearWithDefault);
            //else
            //    if (clearWithDefault)
            //    ringBuffer[where].ClearAllWithDefault();
            ////this way also needs time to clear all cells (overwrite with default)
        }

        //wanted to pull this up to base class, using ICellField2D, didn't find a good way to achieve this
        /// <summary>
        /// Will throw an exception if any of these values violate our rules. 
        /// The rules are as follows: <br></br>
        /// 'template' can't be null - we want to make clones from it
        /// </summary>
        /// <param name="template">we will create clones of this template</param>
        private void SafetyCheckNewRingBuffer(IBinaryCellField2D template)
        {
            if (template == null) throw new ArgumentNullException("The template can't be null!");
        }

        //wanted to pull this up to base class, using ICellField2D, didn't find a good way to achieve this
        protected override void ValueCheck(IBinaryCellField2D newValue)
        {
            base.ValueCheck(newValue); //currently does nothing

            if (newValue != null)
            {
                if (newValue.CellsX != CellsX) throw new ArgumentException($"cell field must match our CellsX value of {CellsX}!");
                if (newValue.CellsY != CellsY) throw new ArgumentException($"cell field must match our CellsY value of {CellsY}!");
            }
        }

        //Array.Clear in parent class 'GenericRingBuffer1D' does the same as setting ringBuffer[pos] = null
        //protected override void FreeBuffer(int pos)
        //{
        //    ringBuffer[pos] = null;
        //}


        //public methods:

        public virtual IBinaryCellField2D CreateCellField2D(bool clearWithDefault)
        {
            if (clearWithDefault)
                return (IBinaryCellField2D)templateWithDefault.Clone();
            else
                return (IBinaryCellField2D)template.Clone();
        }
    }
}
