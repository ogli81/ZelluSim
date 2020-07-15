namespace ZelluSim.RingBuffer
{
    /// <summary>
    /// Our ring buffers have two ends: oldest(first) and newest(last). 
    /// Usually we want to insert new entries at the newest(last) end. 
    /// But there are operations (like iterating or copying) where the 
    /// situation is not that simple - thus, the API offers the 
    /// possiblity to configure which end exactly will be used.
    /// </summary>
    public enum RingBufferEnd
    {
        /// <summary>
        /// The "leftmost" end of the ring buffer. Other 
        /// terms are "first" and "oldest", because usually we 
        /// add new values at the "rightmost" end of the ring buffer.
        /// </summary>
        LEFTMOST_FIRST_OLDEST,

        /// <summary>
        /// The "rightmost" end of the ring buffer. Other 
        /// terms are "last" and "newest", because usually we 
        /// add new values at the "rightmost" end of the ring buffer.
        /// </summary>
        RIGHTMOST_LAST_NEWEST
    }
}
