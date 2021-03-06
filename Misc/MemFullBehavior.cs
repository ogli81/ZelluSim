﻿namespace ZelluSim.Misc
{
    /// <summary>
    /// How should the program react when the memory is full (and resizing to a bigger capacity is not allowed)?
    /// </summary>
    public enum MemFullBehavior
    {
        /// <summary>
        /// Forget old data, don't report anything.
        /// </summary>
        FORGET_SILENTLY,
        
        //FORGET_AND_LOG, //write to logfile
        
        /// <summary>
        /// Stop working (further attempts to continue the work will be ignored silently).
        /// </summary>
        STOP_SILENTLY,

        //STOP_AND_LOG, //write to logfile

        /// <summary>
        /// An exception is thrown (and usually the program is terminated due to that exception).
        /// </summary>
        THROW_EXCEPTION
    }
}