using System;
using System.Collections;
using ZelluSim.CellField;
using ZelluSim.Misc;

namespace ZelluSim.RingBuffer
{
    /// <summary>
    /// A 3-dimensional ringbuffer (stores a '2-dimensional cell field' with a 'memory of the past'). 
    /// </summary>
    /// <typeparam name="T"> The type that we store in a cell, e.g. byte or double or decimal.<br></br>
    /// Note: If T is 'bool' then we don't save our data in the most efficient manner (that would be the usage of 
    /// <see cref="BinaryRingBuffer3D"/>).
    /// </typeparam>
    public class GenericRingBuffer3D<T> : AbstractRingBuffer3D<IGenericCellField2D<T>> //: AbstractRingBuffer3D
    {
        //constants:

        private static (int x, int y) UPPER_LEFT = (0, 0);


        //state:

        protected IGenericCellField2D<T> template;
        protected IGenericCellField2D<T> templateWithDefault;


        //c'tors:

        /// <summary>
        /// Make a new instance (first c'tor variant).
        /// </summary>
        /// <param name="mem">number of memory slots in the ring buffer (1st dimension)</param>
        /// <param name="template">we will create clones of this template</param>
        public GenericRingBuffer3D(int mem, IGenericCellField2D<T> template) : base(mem, template.CellsX, template.CellsY)
        {
            SafetyCheckNewRingBuffer(template);

            IGenericCellField2D<T> template1 = (IGenericCellField2D<T>)template.Clone();
            IGenericCellField2D<T> template2 = (IGenericCellField2D<T>)template.Clone();
            template2.ClearAllWithDefault();
            AcceptTemplates(template1, template2);

            //earlier idea: reserve memory now for the entire ring buffer
            //current idea: reserve memory as soon as it is needed
        }

        /// <summary>
        /// Make a new instance, copy from other instance (copy c'tor A, second c'tor variant). 
        /// Makes a copy of the other ring buffer (but only copies useful data, to safe time).
        /// </summary>
        /// <param name="other">the other instance</param>
        /// <param name="copyUnusedElements">if you are recycling old elements, set this parameter to true</param>
        public GenericRingBuffer3D(GenericRingBuffer3D<T> other, 
            bool copyUnusedElements = false) : base(other, true, copyUnusedElements)
        {
            //no safety check needed, since the other must have undergone that check

            AcceptTemplates((IGenericCellField2D<T>)other.template.Clone(), 
                (IGenericCellField2D<T>)other.templateWithDefault.Clone());
        }

        /// <summary>
        /// Make a new instance, copy from other instance (copy c'tor B, third c'tor variant). 
        /// Tries to grab as much data from the other instance as possible.
        /// </summary>
        /// <param name="mem">number of memory slots in the ring buffer (1st dimension)</param>
        /// <param name="other">the other instance</param>
        /// <param name="startHere">do we start copying at the "leftmost" or "rightmost" end?</param>
        /// <param name="copyUnusedElements">if you are recycling old elements, set this parameter to true</param>
        public GenericRingBuffer3D(int mem, GenericRingBuffer3D<T> other,
            RingBufferEnd startHere = RingBufferEnd.RIGHTMOST_LAST_NEWEST,
            bool copyUnusedElements = false) : this(mem, other.template, other, startHere, copyUnusedElements)
        {

        }

        //fifth variant: change x/y, but keep mem as it is
        public GenericRingBuffer3D(IGenericCellField2D<T> template, GenericRingBuffer3D<T> other,
            RingBufferEnd startHere = RingBufferEnd.RIGHTMOST_LAST_NEWEST,
            bool copyUnusedElements = false) : this(other.MemSlots, template, other, startHere, copyUnusedElements)
        {

        }

        /// <summary>
        /// Make a new instance, copy from other instance (copy c'tor C, fourth c'tor variant). 
        /// Tries to grab as much data from the other instance as possible.
        /// </summary>
        /// <param name="mem">number of memory slots in the ring buffer (1st dimension)</param>
        /// <param name="template">we will create clones of this template</param>
        /// <param name="other">the other instance</param>
        /// <param name="startHere">do we start copying at the "leftmost" or "rightmost" end?</param>
        /// <param name="copyUnusedElements">if you are recycling old elements, set this parameter to true</param>
        public GenericRingBuffer3D(int mem, IGenericCellField2D<T> template, GenericRingBuffer3D<T> other,
            RingBufferEnd startHere = RingBufferEnd.RIGHTMOST_LAST_NEWEST,
            bool copyUnusedElements = false) : this(mem, template)
        {
            //we copy as many from the active entries from the other ring buffer as will fit into this new ring buffer
            //start at the desired end, until mem is full
            //use top-left corner (0,0), work into x and y direction as far as we can
            //if this new ringbuffer is bigger than the other: fill with 'default value'

            if (other.Empty)
            {
                firstPos = other.firstPos;
                lastPos = other.lastPos;
                empty = true;

                if (!copyUnusedElements)
                    return;
            }

            int x = CellsX;
            int y = CellsY;
            int ox = other.CellsX;
            int oy = other.CellsY;
            int xBound = Math.Min(x, ox);
            int yBound = Math.Min(y, oy);

            if (startHere == RingBufferEnd.RIGHTMOST_LAST_NEWEST)
            {
                int iother = other.lastPos;
                int ithis = MemSlots - 1;
                lastPos = ithis;
                ?;//TODO: make it like in GenericRingBuffer1D
                do
                {
                    CloneCopyEntry(ithis, iother, other, xBound, yBound);

                    if (ithis == 0)
                        break;
                    ithis--;

                    iother--;
                    if (iother == -1)
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
                    CloneCopyEntry(ithis, iother, other, xBound, yBound);

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

        private void AcceptTemplates(IGenericCellField2D<T> template, IGenericCellField2D<T> templateWithDefault)
        {
            template.CloningPolicy = template.CloningPolicy; //handle AUTO_DETECT
            templateWithDefault.CloningPolicy = template.CloningPolicy; //same policy for both
            this.template = template;
            this.templateWithDefault = templateWithDefault;
        }

        private void CloneCopyEntry(int ithis, int iother, GenericRingBuffer3D<T> other, int xBound, int yBound)
        {
            IGenericCellField2D<T> oarr = other.ringBuffer[iother];
            if (oarr == null)
            {
                ringBuffer[ithis] = null;
            }
            else
            {
                MakeEntry(ithis, true);
                IGenericCellField2D<T> arr = ringBuffer[ithis];

                //for (int a = 0; a < xBound; a++)
                //    for (int b = 0; b < yBound; b++)
                //        arr.CloneFromOther(oarr, a, b, a, b);
                arr.CloneFromRegion(oarr, UPPER_LEFT, (xBound, yBound), UPPER_LEFT);
            }
        }

        protected override void MakeEntry(int where, bool clearWithDefault)
        {
            if (clearWithDefault || ringBuffer[where] == null)
                ringBuffer[where] = CreateCellField2D(clearWithDefault);
        }

        //TODO: pull up to base class, make base interface for IGenericCellField2D and IBinaryCellField2D
        /// <summary>
        /// Will throw an exception if any of these values violate our rules. 
        /// The rules are as follows: <br></br>
        /// 'template' can't be null - we want to make clones from it
        /// </summary>
        /// <param name="template">we will create clones of this template</param>
        private void SafetyCheckNewRingBuffer(IGenericCellField2D<T> template)
        {
            if (template == null) throw new ArgumentNullException("The template can't be null!");
        }

        //Array.Clear in parent class 'GenericRingBuffer1D' does the same as setting ringBuffer[pos] = null
        //protected override void FreeBuffer(int pos)
        //{
        //    ringBuffer[pos] = null;
        //}

        //TODO: pull up to base class, make base interface for IGenericCellField2D and IBinaryCellField2D
        protected override void ValueCheck(IGenericCellField2D<T> newValue)
        {
            base.ValueCheck(newValue); //currently does nothing

            if (newValue != null)
            {
                if (newValue.CellsX != CellsX) throw new ArgumentException($"cell field must match our CellsX value of {CellsX}!");
                if (newValue.CellsY != CellsY) throw new ArgumentException($"cell field must match our CellsY value of {CellsY}!");
            }
        }


        //public methods:

        public virtual IGenericCellField2D<T> CreateCellField2D(bool clearWithDefault)
        {
            if (clearWithDefault)
                return (IGenericCellField2D<T>)templateWithDefault.Clone();
            else
                return (IGenericCellField2D<T>)template.Clone();
        }
    }
}
