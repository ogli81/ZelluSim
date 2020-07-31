using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ZelluSim.RingBuffer
{
    /// <summary>
    /// A 1-dimensional ringbuffer (stores binary/boolean entries). This version of the 1-dimensional 
    /// ring buffer idea uses a BitArray (just a huge series of bits, each representing one of the 
    /// boolean entries in the ringbuffer). The intention is to save as much space as possible by 
    /// packing all the values in the BitArray instead of using a bool value for each boolean value. <br/>
    /// (IDEA for later: We might want to save even more memory space by using some kind of in-memory compression)
    /// </summary>
    class BinaryRingBuffer1D : AbstractRingBuffer1D
    {
        //state:

        readonly BitArray ringBuffer;


        //c'tors:

        /// <summary>
        /// Make a new instance (first c'tor variant).
        /// </summary>
        /// <param name="mem">number of memory slots in the ring buffer (1st dimension)</param>
        public BinaryRingBuffer1D(int mem) : base(mem)
        {
            ringBuffer = new BitArray(mem);
        }

        /// <summary>
        /// Make a new instance, copy from other instance (copy c'tor A, second c'tor variant). 
        /// Makes a copy of the other ring buffer.
        /// </summary>
        /// <param name="other">the other instance</param>
        public BinaryRingBuffer1D(BinaryRingBuffer1D other) : base(other.ringBuffer.Count)
        {
            firstPos = other.firstPos;
            count = other.count;

            ringBuffer = new BitArray(other.ringBuffer);
        }

        /// <summary>
        /// Make a new instance, copy from other instance (copy c'tor B, third c'tor variant). 
        /// Tries to grab as much data from the other instance as possible.
        /// </summary>
        /// <param name="mem">number of memory slots in the ring buffer (1st dimension)</param>
        /// <param name="other">the other instance</param>
        /// <param name="startHere">do we start copying at the "leftmost" or "rightmost" end?</param>
        public BinaryRingBuffer1D(int mem, BinaryRingBuffer1D other, 
            RingBufferEnd startHere = RingBufferEnd.RIGHTMOST_LAST_NEWEST) : this(mem)
        {
            if (other.Empty)
            {
                firstPos = 0;
                count = 0;
                return;
            }

            //we copy as much as we can from the other ring buffer

            int otherLast = (other.firstPos + (other.count - 1)) % other.MemSlots;
            int counter = 0;

            if (startHere == RingBufferEnd.RIGHTMOST_LAST_NEWEST) //go from rightmost to leftmost
            {
                int iother = otherLast;
                int ithis = MemSlots - 1;
                do
                {
                    counter++;
                    ringBuffer[ithis] = other.ringBuffer[iother];
                    if (ithis == 0)
                        break;
                    if (iother == other.firstPos)
                        break;
                    ithis--;
                    iother--;
                    iother = (iother < 0) ? other.MemSlots - 1 : iother;
                }
                while (true);
                firstPos = ithis;
                count = counter;
            }
            else //go from leftmost to rightmost
            {
                int iother = other.firstPos;
                int ithis = 0;
                firstPos = ithis;
                do
                {
                    counter++;
                    ringBuffer[ithis] = other.ringBuffer[iother];
                    if (ithis == MemSlots - 1)
                        break;
                    if (iother == otherLast)
                        break;
                    ithis++;
                    iother++;
                    iother = (iother > other.MemSlots - 1) ? 0 : iother;
                }
                while (true);
                count = counter;
            }
        }


        //helper methods:

        protected override void FreeBuffer(int pos)
        {
            //(nothing to do)
        }

        protected override void FreeBuffer(int fromInclusive, int toInclusive)
        {
            //(nothing to do)
        }


        //public methods:

        public override int MemSlots => ringBuffer.Length;
        //public override int MemSlots => ringBuffer.GetLength(0);

        public override void AddLastEntry(bool clearWithDefault = false)
        {
            MoveLastForward();
            if (clearWithDefault)
                ringBuffer[(firstPos + (count - 1)) % MemSlots] = false;
        }

        public override void AddFirstEntry(bool clearWithDefault = false)
        {
            MoveFirstBack();
            if (clearWithDefault)
                ringBuffer[firstPos] = false;
        }

        /// <summary>
        /// Set the value at a specific position, using the specified value.
        /// </summary>
        /// <param name="i">the position of the entry, with first=0 and last=(Length-1)</param>
        /// <param name="value">the value that you want to set</param>
        public void SetEntry(int i, bool value)
        {
            BoundsCheck(i);
            int pos = firstPos + i;
            pos %= MemSlots; //this is a bit faster than the next two lines (by about 20 percent)
            //if (pos >= MemSlots) //watch the video "Branchless Programming [...]" on Youtube
            //    pos -= MemSlots; //to understand what might be the reason for that difference
            ringBuffer[pos] = value;
        }

        /// <summary>
        /// Get the value at a specific position.
        /// </summary>
        /// <param name="i">the position of the entry, with first=0 and last=(Length-1)</param>
        /// <returns>will return the entry at that position (or throw an exception)</returns>
        public bool GetEntry(int i)
        {
            BoundsCheck(i);
            int pos = firstPos + i;
            pos %= MemSlots; //this is a bit faster than the next two lines (by about 20 percent)
            //if (pos >= MemSlots) //watch the video "Branchless Programming [...]" on Youtube
            //    pos -= MemSlots; //to understand what might be the reason for that difference
            return ringBuffer[pos];
        }

        /// <summary>
        /// Get or set the last/rightmost data from the ring buffer (position is Length-1). 
        /// Might throw an exception if that position is not available.
        /// </summary>
        public bool Last
        {
            get => GetEntry(Length - 1);
            set => SetEntry(Length - 1, value);
        }

        /// <summary>
        /// Get or set the first/leftmost data from the ring buffer (position is 0). 
        /// Might throw an exception if that position is not available.
        /// </summary>
        public bool First
        {
            get => GetEntry(0);
            set => SetEntry(0, value);
        }

        /// <summary>
        /// Get or set the data entry right before the last/rightmost entry (position is Length-2). 
        /// Might throw an exception if that position is not available.
        /// </summary>
        public bool Previous
        {
            get => Length > 1 ? GetEntry(Length - 2) : throw new InvalidOperationException("We need at least two entries!");
            set
            {
                if (Length < 2)
                    throw new InvalidOperationException("We need at least two entries!");
                SetEntry(Length - 2, value);
            }
        }

        /// <summary>
        /// Get or set the data entry right after the first/leftmost entry (position is 1). 
        /// Might throw an exception if that position is not available.
        /// </summary>
        public bool Second
        {
            get => Length > 1 ? GetEntry(1) : throw new InvalidOperationException("We need at least two entries!");
            set
            {
                if (Length < 2)
                    throw new InvalidOperationException("We need at least two entries!");
                SetEntry(1, value);
            }
        }

        /// <summary>
        /// The indexer can be used like this: 
        /// <code>
        /// var first = myRingBuffer[0]; //<- this will return the first/leftmost entry from the ring buffer<br></br>
        /// var last = myRingBuffer[myRingBuffer.Length - 1]; //this will return the last/rightmost entry
        /// </code>
        /// The indexer can be used for reading or writing the value (<seealso cref="BinaryRingBuffer1D.SetEntry(int, bool)"/>).
        /// <br></br>
        /// 0 = leftmost = First  and  Length-1 = rightmost = Last
        /// </summary>
        /// <param name="i">the position of the entry, with first=0 and last=(Length-1)</param>
        /// <returns>if used for reading, this indexer will return the entry at that position (or throw an exception)</returns>
        public bool this[int i]
        {
            get => GetEntry(i);
            set => SetEntry(i, value);
        }
    }
}
