using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZelluSim.RingBuffer
{
    /// <summary>
    /// A 1-dimensional ringbuffer (stores a list of values with a fixed-size maximum number of entries). 
    /// </summary>
    public abstract class AbstractRingBuffer1D
    {
        //state:

        protected int firstPos; //where in the ringBuffer is the leftmost entry?
        protected int count; //how many entries do we have in our ringbuffer?
        //FYI: using int means that we can't have more than 2,147,483,648 positions in the buffer


        //c'tors:

        /// <summary>
        /// Make a new instance (first c'tor variant).
        /// </summary>
        /// <param name="mem">number of memory slots in the ring buffer (1st dimension)</param>
        protected AbstractRingBuffer1D(int mem)
        {
            SafetyCheckNewRingBuffer(mem);
        }


        //helper methods:

        /// <summary>
        /// Will throw an exception if any of these values violates our rules. 
        /// The rules are as follows: <br></br>
        /// 'mem' should be >= 2 <br></br>
        /// </summary>
        /// <param name="mem">number of memory slots in the ring buffer (1st dimension)</param>
        private void SafetyCheckNewRingBuffer(int mem)
        {
            if (mem < 2)
                throw new ArgumentException("please give us at least 2 memory slots!");
        }

        protected bool IsUsed(int arrayPos)
        {
#if DEBUG
            Debug.Assert(arrayPos >= 0, $"arrayPos too low: {arrayPos}");
            Debug.Assert(arrayPos < MemSlots, $"arrayPos too hig: {arrayPos}, we only have: {MemSlots} elements in array");
#endif
            if (Empty) return false;
            if (firstPos + count <= MemSlots)
                return arrayPos >= firstPos && arrayPos <= (firstPos + (count-1));
            else
                return !(arrayPos >= (firstPos + count - MemSlots) && arrayPos < firstPos);
        }

        protected void AdvanceFirst()
        {
            firstPos++;
            firstPos %= MemSlots; //possibly faster than the next two lines (branchless programming)
            //if (firstPos == MemSlots)
            //    firstPos = 0;
        }

        protected void MoveLastForward()
        {
            if (!Full)
                count++;
            else
                AdvanceFirst();
        }

        protected bool MoveLastBack()
        {
            if (Empty)
                return false;
            count--;
            return true;
        }

        protected bool MoveFirstForward()
        {
            if (Empty)
                return false;
            AdvanceFirst();
            return true;
        }

        protected void MoveFirstBack()
        {
            if (!Full)
                count++;
            firstPos--;
            if (firstPos == -1)
                firstPos = MemSlots - 1;
        }

        protected void BoundsCheck(int i)
        {
            if (Empty)
                throw new ArgumentException("This ring buffer is empty! (there are no valid position indices)");
            if (i > Length - 1)
                throw new ArgumentException("Index i can't be more than Length-1!");
            if (i < 0)
                throw new ArgumentException("Index i can't be less than zero!");
        }

        protected abstract void FreeBuffer(int pos); //we will use Array.Clear for that

        protected abstract void FreeBuffer(int fromInclusive, int toInclusive); //and for that too


        //public methods:

        /// <summary>
        /// The number of memory slots in this ring buffer (1st dim).
        /// </summary>
        public abstract int MemSlots { get; }

        /// <summary>
        /// The number of valid data entries in this ring buffer. 
        /// Directly after creating a new instance (first c'tor variant), this value is 0. 
        /// The value can never be more than the number of memory slots (first dimension).
        /// </summary>
        public int Length => count;

        /// <summary>
        /// Is this ring buffer fully filled? (Length is same as number of MemSlots => ring buffer is full)
        /// </summary>
        public bool Full => Length == MemSlots;

        /// <summary>
        /// Is this ring buffer totally empty?
        /// </summary>
        public bool Empty => count == 0;

        /// <summary>
        /// Make the ring buffer bigger, by reserving another entry.
        /// Add another new entry in the ring buffer. You can chose if you want to clear the cells 
        /// or if you want to use them as they are (possibly containing old data from previous allocations). 
        /// Clearing the cells will cost a bit of processing time (writes to memory) and after that process 
        /// the cells will all contain the default value of the type T / type bool (depending on subclass). 
        /// <seealso cref="AddFirstEntry(bool)"/>
        /// </summary>
        /// <param name="clearWithDefault">will clear the memory slot, using the default value of the type</param>
        public abstract void AddLastEntry(bool clearWithDefault = false);

        /// <summary>
        /// Make the ring buffer bigger, by reserving another entry.
        /// Similar to <see cref="AddLastEntry(bool)"/> but adds entries from the leftmost, not rightmost position.
        /// </summary>
        /// <param name="clearWithDefault">will clear the memory slot, using the default value of the type</param>
        public abstract void AddFirstEntry(bool clearWithDefault = false);

        /// <summary>
        /// Make the ring buffer bigger, by reserving another entry.
        /// You can select at which of the two ends of the ring you want to make another entry.
        /// </summary>
        /// <param name="where">select either the leftmost/first or rightmost/last end of the buffer</param>
        /// <param name="clearWithDefault">will clear the memory slot, using the default value of the type</param>
        public void AddEntry(RingBufferEnd where, bool clearWithDefault = true)
        {
            if (where == RingBufferEnd.RIGHTMOST_LAST_NEWEST)
                AddLastEntry(clearWithDefault);
            else
                AddFirstEntry(clearWithDefault);
        }

        /// <summary>
        /// Will try to remove one entry from the rightmost (newest) end of the buffer. This 
        /// attempt will fail, if no entry was left when you tried to remove one entry.
        /// </summary>
        /// <returns>Will return false, if there was no entry left when you tried to remove one 
        /// (and true otherwise).</returns>
        public bool RemoveLastEntry() => MoveLastBack();

        /// <summary>
        /// Will try to remove as many entries as you want to remove. If not that many 
        /// entries can be removed (so if less than 'howMany' entries are available), 
        /// false will be returned (and otherwise true).
        /// </summary>
        /// <param name="howMany">The number of entries that we want to remove.</param>
        /// <returns>Will return false, if not that many entries were removed (so when false: 
        /// it's either zero or less than intended were removed)</returns>
        public bool RemoveLastEntries(int howMany)
        {
            if (howMany < 0)
                throw new ArgumentException("can't be less than zero!");

            int todo = howMany;
            for(; todo > 0; todo--)
                if (!RemoveLastEntry())
                    return false;

            return true;
        }

        /// <summary>
        /// Will try to remove one entry from the leftmost (first) end of the buffer. This 
        /// attempt will fail, if no entry was left when you tried to remove one entry.
        /// </summary>
        /// <returns>Will return false, if there was no entry left when you tried to remove one 
        /// (and true otherwise).</returns>
        public bool RemoveFirstEntry() => MoveFirstForward();

        /// <summary>
        /// Will try to remove as many entries as you want to remove. If not that many 
        /// entries can be removed (so if less than 'howMany' entries are available), 
        /// false will be returned (and otherwise true).
        /// </summary>
        /// <param name="howMany">The number of entries that we want to remove.</param>
        /// <returns>Will return false, if not that many entries were removed (so when false: 
        /// it's either zero or less than intended were removed)</returns>
        public bool RemoveFirstEntries(int howMany)
        {
            if (howMany < 0)
                throw new ArgumentException("can't be less than zero!");

            int todo = howMany;
            while (todo > 0) //faster or slower than for-loop?
            {
                if (!RemoveFirstEntry())
                    return false;
                todo--;
            }
            return true;
        }

        /// <summary>
        /// Remove one entry. Returns false, if no entries were left in the ring.
        /// </summary>
        /// <param name="where">chose which side of the ring (first/leftmost or last/rightmost)</param>
        /// <returns>false, if no entries were left in the ring, true otherwise</returns>
        public bool RemoveEntry(RingBufferEnd where)
        {
            if (where == RingBufferEnd.RIGHTMOST_LAST_NEWEST)
                return RemoveLastEntry();
            else
                return RemoveFirstEntry();
        }

        /// <summary>
        /// Remove multiple entries. Returns false, if not that many entries were left in the ring.
        /// </summary>
        /// <param name="howMany">chose how many entries you want to remove</param>
        /// <param name="where">chose which side of the ring (first/leftmost or last/rightmost)</param>
        /// <returns>false, if not that many entries were left, true otherwise</returns>
        public bool RemoveEntries(int howMany, RingBufferEnd where)
        {
            if (where == RingBufferEnd.RIGHTMOST_LAST_NEWEST)
                return RemoveLastEntries(howMany);
            else
                return RemoveFirstEntries(howMany);
        }

        /// <summary>
        /// Discard everything except the rightmost (last) entry.
        /// </summary>
        public void RemoveAllExceptLast()
        {
            if (count < 2)
                return;
            firstPos += (count-1);
            firstPos %= MemSlots;
            count = 1;
        }

        /// <summary>
        /// Discard everything except the leftmost (first) entry.
        /// </summary>
        public void RemoveAllExceptFirst()
        {
            if (count < 2)
                return;
            count = 1;
        }

        /// <summary>
        /// Discard all entries.
        /// </summary>
        public void RemoveAll()
        {
            RemoveAllExceptLast();
            count = 0;
        }

        /// <summary>
        /// Only those entries will be kept in memory that are actually used (they are between first and last). 
        /// Using this method might free some memory but it might also cause more memory fragmentation! 
        /// Also, this won't bring you anything if value types are stored in the underlying array. 
        /// If references are stored (and not values) then they will be replaced by null during this operation. 
        /// If the full ring buffer is being used right now then we can't free any memory.
        /// </summary>
        public void FreeUnusedMemory()
        {
            if (Empty)
            {
                FreeBuffer(0, MemSlots - 1);
                return;
            }

            if (firstPos + count <= MemSlots)
            {
                if (firstPos > 0)
                    FreeBuffer(0, firstPos - 1);
                if (firstPos + count < MemSlots)
                    FreeBuffer(firstPos + count, MemSlots - 1);
            }
            else
            {
                int lastPos = firstPos + count - MemSlots - 1;
                if (lastPos < firstPos - 1)
                    FreeBuffer(lastPos + 1, firstPos - 1);
            }
        }
    }
}
