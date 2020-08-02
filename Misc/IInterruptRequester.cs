using System;
using System.Collections.Generic;
using System.Text;

namespace ZelluSim.Misc
{
    /// <summary>
    /// This interface is intended for situations where a computation needs to be done 
    /// that might run for a very long time. The user of the system may want to request
    /// a premature exit/escape from the computation loop. For example, there may be a
    /// [Stop] button in a GUI or the [ESC] key was pressed in a CLI. <br></br>
    /// The <see cref="IInterruptRequester"/> should be asked periodically, if it wants
    /// the computation to interrupt and leave right now. If the <see cref="IInterruptRequester"/>
    /// says 'yes'('true'), the computation should stop and if it says 'no'('false')
    /// the computation may continue for a while.
    /// </summary>
    public interface IInterruptRequester
    {
        /// <summary>
        /// The <see cref="IInterruptRequester"/> should be asked periodically, if it wants
        /// the computation to interrupt and leave right now. If the <see cref="IInterruptRequester"/>
        /// says 'yes'('true'), the computation should stop and if it says 'no'('false')
        /// the computation may continue for a while.
        /// </summary>
        /// <returns>true, if an interrupt was requested (and false if no interrupt was requested)</returns>
        bool RequestingInterrupt();
    }
}
