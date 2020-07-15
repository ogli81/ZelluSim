using System;
using System.Collections;
using System.Diagnostics;

namespace ZelluSim.RingBuffer
{
    /// <summary>
    /// A 1-dimensional ringbuffer (stores generic entries). 
    /// </summary>
    /// <typeparam name="T"> 
    /// The type that we store in each position, e.g. byte or double or decimal or Vector2.<br></br>
    /// Note: If T is 'bool' then we don't save our data in the most efficient manner (that would be the usage of 
    /// <see cref="BinaryRingBuffer1D"/>).<br></br>
    /// Note: If you are using reference types, your ring buffer might consume a lot of memory and you are 
    /// responsible to free that memory using <see cref="AbstractRingBuffer1D.FreeUnusedMemory"/>. You might 
    /// also be interested in reusing (recycling) old instances and in that case your program will usually 
    /// be more efficient - in that case don't call <see cref="AbstractRingBuffer1D.FreeUnusedMemory"/>.
    /// </typeparam>
    public class GenericRingBuffer1D<T> : AbstractRingBuffer1D
    {
        //state:

        //1st dim = "ring buffer position"
        protected T[] ringBuffer;


        //c'tors:

        /// <summary>
        /// Make a new instance (first c'tor variant).
        /// </summary>
        /// <param name="mem">number of memory slots in the ring buffer (1st dimension)</param>
        public GenericRingBuffer1D(int mem) : base(mem)
        {
            ringBuffer = new T[mem];
        }

        /// <summary>
        /// Make a new instance, copy from other instance (copy c'tor A, second c'tor variant). 
        /// Makes a copy of the other ring buffer. If you are recycling unused elements, you might be 
        /// interested in setting the third param to true.
        /// </summary>
        /// <param name="other">the other instance</param>
        /// <param name="tryDeepCopy">if the values are references that implement ICloneable then set this to true</param>
        /// <param name="copyUnusedElements">if you are recycling old elements, set this parameter to true</param>
        public GenericRingBuffer1D(GenericRingBuffer1D<T> other, bool tryDeepCopy = false, 
            bool copyUnusedElements = false) : this(other.MemSlots)
        {
            if (other.Empty)
            {
                firstPos = other.firstPos;
                lastPos = other.lastPos;
                empty = true;

                if(!copyUnusedElements)
                    return;
            }

            if (tryDeepCopy)
            {
                for (int pos = 0; pos < MemSlots - 1; pos++)
                    if (copyUnusedElements || other.IsUsed(pos))
                        //if (other.ringBuffer[pos] != null && other.ringBuffer[pos] is ICloneable)
                        if (other.ringBuffer[pos] is ICloneable) //null is not a ICloneable
                            ringBuffer[pos] = (T)(other.ringBuffer[pos] as ICloneable).Clone();
            }
            else
                Array.Copy(other.ringBuffer, ringBuffer, ringBuffer.Length); //just copy all there is (should be fast enough)
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
        public GenericRingBuffer1D(int mem, GenericRingBuffer1D<T> other, 
            RingBufferEnd startHere = RingBufferEnd.RIGHTMOST_LAST_NEWEST, bool tryDeepCopy = false, 
            bool copyUnusedElements = false) : this(mem)
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

            if (startHere == RingBufferEnd.RIGHTMOST_LAST_NEWEST) //go from rightmost to leftmost
            {
                int iother = other.lastPos;
                int ithis = MemSlots - 1;
                lastPos = ithis;
                for(int i = 0; i < iMax; i++)
                {
                    if (other.IsUsed(iother))
                    {
                        if (tryDeepCopy && other.ringBuffer[iother] is ICloneable)
                            ringBuffer[ithis] = (T)(other.ringBuffer[iother] as ICloneable).Clone();
                        else
                            ringBuffer[ithis] = other.ringBuffer[iother];
                    }

                    if (ithis == 0)
                        break;
                    ithis--;

                    iother--;
                    if (iother < 0)
                        iother = other.MemSlots - 1;
                }
                firstPos = ithis;
            }
            else //go from leftmost to rightmost
            {
                int iother = other.firstPos;
                int ithis = 0;
                firstPos = ithis;
                for (int i = 0; i < iMax; i++)
                {
                    if (other.IsUsed(iother))
                    {
                        if (tryDeepCopy && other.ringBuffer[iother] is ICloneable)
                            ringBuffer[ithis] = (T)(other.ringBuffer[iother] as ICloneable).Clone();
                        else
                            ringBuffer[ithis] = other.ringBuffer[iother];
                    }

                    if (ithis == MemSlots - 1)
                        break;
                    ithis++;

                    iother++;
                    if (iother >= other.MemSlots)
                        iother = 0;
                }
                lastPos = ithis;
            }
        }


        //helper methods:

        private bool IsUsed(int arrayPos)
        {
#if DEBUG
            Debug.Assert(arrayPos >= 0, $"arrayPos too low: {arrayPos}");
            Debug.Assert(arrayPos < ringBuffer.Length, $"arrayPos too hig: {arrayPos}, we only have: {ringBuffer.Length} elements in array");
#endif
            if (Empty) return false;
            if (firstPos <= lastPos)
                return arrayPos >= firstPos && arrayPos <= lastPos;
            else
                return !(arrayPos > lastPos && arrayPos < firstPos);
        }

        protected virtual void MakeEntry(int where, bool clearWithDefault)
        {
            if (clearWithDefault)
                Array.Clear(ringBuffer, where, 1);
        }

        //private void MakeEntry(int where, T initialValue, bool tryDeepCopy = false)
        //{
        //    if (tryDeepCopy && initialValue is ICloneable)
        //        ringBuffer[where] = (T)(initialValue as ICloneable).Clone();
        //    else
        //        ringBuffer[where] = initialValue;
        //}

        protected override void FreeBuffer(int pos)
        {
            Array.Clear(ringBuffer, pos, 1);
        }

        protected override void FreeBuffer(int fromInclusive, int toInclusive)
        {
            Array.Clear(ringBuffer, fromInclusive, toInclusive - fromInclusive + 1);
        }

        protected virtual void ValueCheck(T newValue)
        {
            //(nothing to do - even null values are accepted)
        }


        //public methods:

        public override int MemSlots => ringBuffer.Length;
        //public override int MemSlots => ringBuffer.GetLength(0);

        public override void AddLastEntry(bool clearWithDefault = false)
        {
            MoveLastForward();
            MakeEntry(lastPos, clearWithDefault);
        }

        public override void AddFirstEntry(bool clearWithDefault = false)
        {
            MoveFirstBack();
            MakeEntry(firstPos, clearWithDefault);
        }

        /// <summary>
        /// Set the value at a specific position, using the specified value.
        /// </summary>
        /// <param name="i">the position of the entry, with first=0 and last=(Length-1)</param>
        /// <param name="value">the value that you want to set</param>
        public virtual void SetEntry(int i, T value)
        {
            BoundsCheck(i);
            ValueCheck(value);
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
        public T GetEntry(int i)
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
        public T Last
        {
            get => GetEntry(Length - 1);
            set => SetEntry(Length - 1, value);
        }

        /// <summary>
        /// Get or set the first/leftmost data from the ring buffer (position is 0). 
        /// Might throw an exception if that position is not available.
        /// </summary>
        public T First
        {
            get => GetEntry(0);
            set => SetEntry(0, value);
        }

        /// <summary>
        /// Get or set the data entry right before the last/rightmost entry (position is Length-2). 
        /// Might throw an exception if that position is not available.
        /// </summary>
        public T Previous
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
        public T Second
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
        /// The indexer can be used for reading or writing the value (<seealso cref="GenericRingBuffer1D{T}.SetEntry(int, T)"/>).
        /// <br></br>
        /// 0 = leftmost = First  and  Length-1 = rightmost = Last
        /// </summary>
        /// <param name="i">the position of the entry, with first=0 and last=(Length-1)</param>
        /// <returns>if used for reading, this indexer will return the entry at that position (or throw an exception)</returns>
        public T this[int i]
        {
            get => GetEntry(i);
            set => SetEntry(i, value);
        }
    }
}
