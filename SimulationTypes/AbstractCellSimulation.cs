using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZelluSim.RingBuffer;
using ZelluSim.Misc;

namespace ZelluSim.SimulationTypes
{
    public abstract class AbstractCellSimulation<T> : ICellSimulation
    {
        //state:

        protected int currentGen = 0;
        protected AbstractRingBuffer3D<T> aring;
        public SimulationSettings Settings { get; }
        protected SimulationParameter? param1 = null;
        protected SimulationParameter? param2 = null;

        //now this is still an open qeustion:
        protected List<decimal> stats1;
        //---> should we use a normal List<T> or something more flexible?
        //
        //a LinkedList usually is very slow, but might work (
        //protected LinkedList<decimal> stats1;
        //
        //a really good "double ended queue" might do the trick
        //ideas here:
        //https://www.c-sharpcorner.com/UploadFile/b942f9/implementing-a-double-ended-queue-or-deque-in-C-Sharp/
        //
        //or we could write our own array-based List (not a good idea, 
        //or we use a 1-dimensional ringbuffer! (works well, since we know the size is fixed)


        //c'tors:

        public AbstractCellSimulation(SimulationSettings settings)
        {
            Settings = settings;
            stats1 = settings.TrackLifeStats ? new List<decimal>() : null;
        }
        /// <summary>
        /// Call this in the last line of your c'tor. When making subclasses of subclasses: 
        /// Overwrite this method with an empty body and make your own injection method.
        /// </summary>
        protected void InjectSettings() { Settings.Sim = this; }


        //helper methods:

        /// <summary>
        /// This is the core method of our simulation classes. 
        /// Override this to implement the logic of your specific type of simulation.
        /// </summary>
        /// <returns></returns>
        protected abstract bool DoCalculateNextGen();

        protected bool DoCreateNewGeneration()
        {
            if (aring.Full)
            {
                switch (Settings.MemSlotsFullBehavior)
                {
                    case MemFullBehavior.FORGET_SILENTLY:
                        break;
                    case MemFullBehavior.STOP_SILENTLY:
                        return false;
                    case MemFullBehavior.THROW_EXCEPTION:
                        throw new OutOfMemoryException("not enough memory slots for new cell fields!");
                        //throw new InsufficientMemoryException("not enough memory slots for new cell fields!");
                }
            }
            aring.AddLastEntry();
            return true;
        }

        protected virtual void DoResizeRingBuffer(int mem, int x, int y)
        {
            AbstractRingBuffer3D<T>.SafetyCheckNewRingBuffer(mem, x, y);
        }

        protected void LifeStatsEnsureCapacity()
        {
            if (stats1 == null)
                return;

            if (Settings.LifeStatsMem < 1)
                stats1.Clear();
            else
                LifeStatsRemoveOldest(stats1.Count - Settings.LifeStatsMem);
        }

        protected void LifeStatsRemoveNewest(int howMany)
        {
            if (stats1 != null)
                if (howMany >= stats1.Count)
                    stats1.Clear();
                else
                    stats1.RemoveRange(stats1.Count - howMany, howMany);
        }

        protected void LifeStatsRemoveOldest(int howMany)
        {
            if (stats1 != null)
                if (howMany >= stats1.Count)
                    stats1.Clear();
                else
                    stats1.RemoveRange(0, howMany);
        }


        //abstract public methods:

        public abstract string Info { get; }

        public abstract void SetCellValue(int x, int y, decimal val);

        public abstract decimal GetCellValue(int x, int y);


        //public methods:

        public void SettingsChanged()
        {
            //Settings.MemSlots
            //Settings.SizeX
            //Settings.SizeY
            if (Settings.MemSlots != aring.MemSlots ||
                Settings.SizeX != aring.CellsX ||
                Settings.SizeY != aring.CellsY)
                DoResizeRingBuffer(Settings.MemSlots, Settings.SizeX, Settings.SizeY);

            //Settings.IsWrap
            //(nothing to do - future calculations will be performed with/without wrap)

            //Settings.TrackLifeStats
            if (Settings.TrackLifeStats == true && stats1 == null)
                stats1 = new List<decimal>(Settings.LifeStatsMem);
            else
            if (Settings.TrackLifeStats == false && stats1 != null)
                stats1 = null;

            //Settings.LifeStatsMem
            LifeStatsEnsureCapacity();

            //Settings.MemSlotsFullBehavior
            //Settings.LifeStatsFullBehavior
            //(nothing to do - behavior will change for next "mem full" situation)
        }

        public void GoToOldestGen()
        {
            aring.RemoveAllExceptFirst();
        }

        public void RelabelCurrentAsZero()
        {
            aring.RemoveAllExceptLast();
        }

        public bool CalculateNextGen()
        {
            if (!DoCreateNewGeneration())
                return false;
            DoCalculateNextGen();
            return true;
        }

        public int CurrentGen => currentGen;

        public int OldestGen => currentGen - aring.Length + 1;

        public int NumGens => aring.Length;

        public bool GoBackOneGen() => aring.RemoveLastEntry();

        public bool GoToGen(int genId)
        {
            if (genId < 0)
                return false;
            int remove = currentGen - genId;
            if (remove <= 0)
                return aring.RemoveLastEntries(remove);
            else
                for (int i = -1; i >= remove; i--)
                    if (!CalculateNextGen())
                        return false;
            return true;
        }

        public SimulationParameter? Param1 => param1;

        public SimulationParameter? Param2 => param2;

        public abstract void ParamsChanged();
    }
}
