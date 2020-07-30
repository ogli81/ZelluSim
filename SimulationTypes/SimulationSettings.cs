using System;
using ZelluSim.Misc;

namespace ZelluSim.SimulationTypes
{
    /// <summary>
    /// The settings of a simulation comprises things like "how many cells" and 
    /// "does the world wrap at its borders" and "how many states from the past 
    /// should we keep as history" and things like that.
    /// </summary>
    public class SimulationSettings : ICloneable
    {
        //state:

        private bool suppress = false;

        private int memSlots = 128; //(initial capacity)
        private int sizeX = 32;
        private int sizeY = 32;

        private bool isWrap = true;

        private int lifeStatsMem = 1_000_000; //(initial capacity)
        private bool trackLifeStats = false;

        private MemFullBehavior memSlotsFullBehavior = MemFullBehavior.FORGET_SILENTLY;
        private decimal memSlotsGrow = 1; //(resize factor on cap hit), 1 = special value for "don't grow"
        private int memSlotsMax = 128; //(max. capacity), -1 = special value for "unlimited"
        private MemFullBehavior lifeStatsFullBehavior = MemFullBehavior.FORGET_SILENTLY;
        private decimal lifeStatsMemGrow = 2; //(resize factor on cap hit), 1 = special value for "don't grow"
        private int lifeStatsMemMax = -1; //(max. capacity), -1 = special value for "unlimited"

        public event EventHandler SettingsChanged;


        //c'tors:

        public SimulationSettings()
        {
            
        }


        //helper methods:

        private void OnSettingsChanged(EventArgs e)
        {
            //EventHandler handler = SettingsChanged;
            //if (handler != null)
            //    handler(this, e);
            SettingsChanged?.Invoke(this, e);
        }

        //we are now using proper C# events for this
        ///// <summary>
        ///// The simulation for which we will call 'SettingsChanged' after changes to our state.
        ///// </summary>
        //public ICellSimulation Sim { 
        //    get => sim; 
        //    set 
        //    {
        //        bool memSuppress = SuppressUpdates;
        //        bool wasNull = sim == null;
        //        if (sim == null)
        //            suppress = false;
        //        sim = value; 
        //        SettingsChanged();
        //        if (wasNull)
        //            suppress = memSuppress;
        //    } 
        //}
        //protected ICellSimulation sim = null;


        //public methods:

        /// <summary>
        /// If we want to change multiple values, we might be interested in temporarily silencing update events.
        /// </summary>
        public bool SuppressUpdates {
            get => suppress;
            set
            {
                if (value == suppress) //didn't change
                    return;
                if (value && !suppress) //was false, is now true
                {
                    suppress = value;
                }
                else //was true, is now false
                {
                    suppress = value;
                    OnSettingsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Number of field instances that we remember. Example: remember 100 cell fields from the past => MemSlots = 100.
        /// </summary>
        public int MemSlots { get => memSlots; set { memSlots = value; OnSettingsChanged(EventArgs.Empty); } }
        /// <summary>
        /// How many cells in x direction? Example: 32 x 32 field => SizeX = 32.
        /// </summary>
        public int SizeX { get => sizeX; set { sizeX = value; OnSettingsChanged(EventArgs.Empty); } }
        /// <summary>
        /// How many cells in y direction? Example: 32 x 32 field => SizeY = 32.
        /// </summary>
        public int SizeY { get => sizeY; set { sizeY = value; OnSettingsChanged(EventArgs.Empty); } }

        /// <summary>
        /// Do we have a wrap-around? (wrap = "if I leave east border, I enter from west border again...")
        /// </summary>
        public bool IsWrap { get => isWrap; set { isWrap = value; OnSettingsChanged(EventArgs.Empty); } }

        /// <summary>
        /// Should we track the overall life sum as statistics?
        /// </summary>
        public bool TrackLifeStats { get => trackLifeStats; set { trackLifeStats = value; OnSettingsChanged(EventArgs.Empty); } }
        /// <summary>
        /// How many data slots does the life sum statistics have?
        /// </summary>
        public int LifeStatsMem { get => lifeStatsMem; set { lifeStatsMem = value; OnSettingsChanged(EventArgs.Empty); } }

        /// <summary>
        /// How should the program react when MemSlots is exceeded?
        /// </summary>
        public MemFullBehavior MemSlotsFullBehavior { get => memSlotsFullBehavior; set { memSlotsFullBehavior = value; OnSettingsChanged(EventArgs.Empty); } }
        /// <summary>
        /// This is a factor that determines how fast the mem slots will grow (e.g. 2 means "double the size when resizing"). 
        /// The special value '1' means "don't grow". 
        /// Values smaller than 1 will either be ignored or may lead to exceptions.
        /// </summary>
        public decimal MemSlotsGrow { get => memSlotsGrow; set { memSlotsGrow = value; OnSettingsChanged(EventArgs.Empty); } }
        /// <summary>
        /// The maximum capacity (maximum value for mem slots). Attempts to resize beyond this value will fail. 
        /// The special value '-1' means "unlimited" (of course, there still is a limit of 2^31 due to the fact that we use 32 bit integers for the size).
        /// </summary>
        public int MemSlotsMax { get => memSlotsMax; set { memSlotsMax = value; OnSettingsChanged(EventArgs.Empty); } }

        /// <summary>
        /// How should the program react when LifeStatsMem is exceeded?
        /// </summary>
        public MemFullBehavior LifeStatsFullBehavior { get => lifeStatsFullBehavior; set { lifeStatsFullBehavior = value; OnSettingsChanged(EventArgs.Empty); } }
        /// <summary>
        /// This is a factor that determines how fast the life stats mem will grow (e.g. 2 means "double the size when resizing"). 
        /// The special value '1' means "don't grow". 
        /// Values smaller than 1 will either be ignored or may lead to exceptions.
        /// </summary>
        public decimal LifeStatsMemGrow { get => lifeStatsMemGrow; set { lifeStatsMemGrow = value; OnSettingsChanged(EventArgs.Empty); } }
        /// <summary>
        /// The maximum capacity (maximum value for life stats mem). Attempts to resize beyond this value will fail. 
        /// The special value '-1' means "unlimited" (of course, there still is a limit of 2^31 due to the fact that we use 32 bit integers for the size).
        /// </summary>
        public int LifeStatsMemMax { get => lifeStatsMemMax; set { lifeStatsMemMax = value; OnSettingsChanged(EventArgs.Empty); } }

        /// <summary>
        /// We return a clone that follows these rules:<br></br>
        /// Every primitive field (value types) is a memberwise copy (<see cref="object.MemberwiseClone"/>).<br></br>
        /// All events get disconnected from the previous listener.
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            SimulationSettings clone = (SimulationSettings)this.MemberwiseClone();

            //now, how do we remove event handlers?
            //(1)
            //this works: (wrote a testcase)
            clone.SettingsChanged = null;
            //(2)
            //this doesn't work (it's not how this works)
            //clone.SettingsChanged = new EventHandler();
            //(3)
            //this works: (wrote a testcase)
            //Delegate[] dels = (Delegate[])clone.SettingsChanged.GetInvocationList().Clone();
            //foreach (Delegate del in dels)
            //    SettingsChanged -= (EventHandler)del;
            //(4)
            //this works: (wrote a testcase)
            //Delegate[] dels = clone.SettingsChanged.GetInvocationList();
            //foreach (Delegate del in dels)
            //    SettingsChanged -= (EventHandler)del;

            return clone;
        }
    }
}