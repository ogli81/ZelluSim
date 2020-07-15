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
            //idea: have one large BitArray, reserve memory for the entire buffer 
            //we might want to implement that idea (and do a performance test)
        }

        /// <summary>
        /// Make a new instance, copy from other instance (copy c'tor A, second c'tor variant). 
        /// Makes a copy of the other ring buffer (but only copies useful data, to safe time).
        /// </summary>
        /// <param name="other">the other instance</param>
        /// <param name="tryDeepCopy">if the values are references that implement ICloneable then set this to true</param>
        public BinaryRingBuffer3D(BinaryRingBuffer3D other) : base(other, true)
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
        public BinaryRingBuffer3D(int mem, BinaryRingBuffer3D other,
            RingBufferEnd startHere = RingBufferEnd.RIGHTMOST_LAST_NEWEST) : base(mem, other, startHere, true)
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
        /// <param name="template">we will create clones of this template</param>
        /// <param name="other">the other instance</param>
        /// <param name="startHere">do we start copying at the "leftmost" or "rightmost" end?</param>
        public BinaryRingBuffer3D(int mem, IBinaryCellField2D template, BinaryRingBuffer3D other,
            RingBufferEnd startHere = RingBufferEnd.RIGHTMOST_LAST_NEWEST) : this(mem, template)
        {
            //we copy as many from the active entries from the other ring buffer as will fit into this new ring buffer
            //start at the desired end, until mem is full
            //use top-left corner (0,0), work into x and y direction as far as we can
            //if this new ringbuffer is bigger than the other: fill with 'default value'

            if (other.Empty)
            {
                return;
            }

            int x = CellsX;
            int y = CellsY;
            int ox = other.CellsX;
            int oy = other.CellsY;

            if (startHere == RingBufferEnd.RIGHTMOST_LAST_NEWEST)
            {
                int iother = other.lastPos;
                int ithis = MemSlots - 1;
                lastPos = ithis;
                do
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
                        for (int a = 0; a < x && a < ox; a++)
                            for (int b = 0; b < y && b < oy; b++)
                                arr[a, b] = oarr[a, b];
                    }

                    if (ithis == 0)
                        break;
                    ithis--;

                    iother--;
                    if (iother < 0)
                        iother = other.MemSlots - 1;
                }
                while (iother != other.firstPos);
                firstPos = ithis;
            }
            else
            {
                int iother = other.firstPos;
                int ithis = 0;
                firstPos = ithis;
                do
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
                        for (int a = 0; a < x && a < ox; a++)
                            for (int b = 0; b < y && b < oy; b++)
                                arr[a, b] = oarr[a, b];
                    }

                    if (ithis == MemSlots - 1)
                        break;
                    ithis++;

                    iother++;
                    if (iother == other.MemSlots)
                        iother = 0;
                }
                while (iother != other.lastPos);
                lastPos = ithis;
            }
        }


        //helper methods:

        private void AcceptTemplates(IBinaryCellField2D template, IBinaryCellField2D templateWithDefault)
        {
            //not much to do here, but we want to stay consistent and make it like in GenericRingBuffer3D
            this.template = template;
            this.templateWithDefault = templateWithDefault;
        }

        protected override void MakeEntry(int where, bool clearWithDefault)
        {
            if (clearWithDefault || ringBuffer[where] == null)
                ringBuffer[where] = CreateCellField2D(clearWithDefault);
        }

        /// <summary>
        /// Will throw an exception if any of these values violate our rules. 
        /// The rules are as follows: <br></br>
        /// 'template' can't be null - we want to make clones from it
        /// </summary>
        /// <param name="template">we will create clones of this template</param>
        protected void SafetyCheckNewRingBuffer(IBinaryCellField2D template)
        {
            if (template == null) throw new ArgumentNullException("The template can't be null!");
        }

        //Array.Clear in parent class 'GenericRingBuffer1D' does the same as setting ringBuffer[pos] = null
        //protected override void FreeBuffer(int pos)
        //{
        //    ringBuffer[pos] = null;
        //}

        //TODO: pull up to base class, make base interface for IGenericCellField2D and IBinaryCellField2D
        protected override void ValueCheck(IBinaryCellField2D newValue)
        {
            base.ValueCheck(newValue); //currently does nothing

            if (newValue != null)
            {
                if (newValue.CellsX != CellsX) throw new ArgumentException($"cell field must match our CellsX value of {CellsX}!");
                if (newValue.CellsY != CellsY) throw new ArgumentException($"cell field must match our CellsY value of {CellsY}!");
            }
        }


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
