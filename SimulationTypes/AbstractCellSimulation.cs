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

        //counter for our current generation (simulation starts with generation zero and adds +1 for each computation step)
        protected int currentGen = 0;
        
        //ringbuffer with N ("mem") slots, each slot contains a 2d field of cells
        protected AbstractRingBuffer3D<T> aring;

        protected SimulationParameter param1 = null;
        protected SimulationParameter param2 = null;

        public SimulationSettings Settings { get; protected set; }
        //protected SimulationSettings Old { get; protected set; } //the previously used settings (initially null)


        //TODO
        //now this is still an open question:
        //protected List<decimal> stats1;
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
        }


        //helper methods:

        /// <summary>
        /// Call this in the last line of your c'tor. When making subclasses of subclasses: 
        /// Overwrite this method with an empty body and make your own injection method.
        /// </summary>
        protected void Init()
        {
            CreateRingBuffer();
            CreateParams();
            CreateStats();
            ConnectEvents();
        }

        protected abstract void CreateRingBuffer(); //see: BinaryCellSimulation  and  GenericCellSimulation
        protected virtual void CreateParams()
        {
            //the params can be 'null'
        }
        protected virtual void CreateStats()
        {
            //stats1 = settings.TrackLifeStats ? new List<decimal>() : null; //TODO
        }
        protected void ConnectEvents()
        {
            Settings.SettingsChanged += SettingsChanged;
            if (Param1 != null)
                Param1.ParamsChanged += ParamsChanged;
            if (Param2 != null)
                Param2.ParamsChanged += ParamsChanged;
        }

        /// <summary>
        /// This is the core method of our simulation classes. 
        /// Override this to implement the logic of your specific type of simulation.
        /// </summary>
        protected abstract void DoCalculateNextGen();

        protected abstract void DoResizeRingBuffer(int mem, int x, int y);

        protected bool DoCreateNewGeneration()
        {
            if (aring.Full && !TryGrowBuffer())
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

        protected bool TryGrowBuffer()
        {
            if (Settings.MemSlotsMax <= aring.MemSlots)
                return false;
            if (Settings.MemSlotsGrow <= 1)
                return false;
            int newSize = (int)(((decimal)aring.MemSlots) * Settings.MemSlotsGrow);
            newSize = Math.Min(newSize, Settings.MemSlotsMax);
            if (newSize == aring.MemSlots)
                return false;
            DoResizeRingBuffer(newSize, aring.CellsX, aring.CellsY);
            return true;
        }

        //TODO

        //protected void LifeStatsEnsureCapacity()
        //{
        //    if (stats1 == null)
        //        return;

        //    if (Settings.LifeStatsMem < 1)
        //        stats1.Clear();
        //    else
        //        LifeStatsRemoveOldest(stats1.Count - Settings.LifeStatsMem);
        //}

        //protected void LifeStatsRemoveNewest(int howMany)
        //{
        //    if (stats1 != null)
        //        if (howMany >= stats1.Count)
        //            stats1.Clear();
        //        else
        //            stats1.RemoveRange(stats1.Count - howMany, howMany);
        //}

        //protected void LifeStatsRemoveOldest(int howMany)
        //{
        //    if (stats1 != null)
        //        if (howMany >= stats1.Count)
        //            stats1.Clear();
        //        else
        //            stats1.RemoveRange(0, howMany);
        //}

        protected virtual void SettingsChanged(object sender, EventArgs e)
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

            //TODO
            //Settings.TrackLifeStats
            //if (Settings.TrackLifeStats == true && stats1 == null)
            //    stats1 = new List<decimal>(Settings.LifeStatsMem);
            //else
            //if (Settings.TrackLifeStats == false && stats1 != null)
            //    stats1 = null;

            //TODO
            //Settings.LifeStatsMem
            //LifeStatsEnsureCapacity();

            //Settings.MemSlotsFullBehavior
            //Settings.LifeStatsFullBehavior
            //(nothing to do - behavior will change for next "mem full" situation)

            //Settings.MemSlotsGrow
            //Settings.LifeStatsMemGrow
            //(nothing to do - will only be relevant for next "must grow" situation)

            //Settings.MemSlotsMax
            //Settings.LifeStatsMemMax
            //(nothing to do - if lower than current values then next "must grow" attempt will simply fail)
        }

        protected virtual void ParamsChanged(object sender, EventArgs e)
        {
            if (sender == (object)Param1)
                Param1Changed(sender, e);
            else
            if (sender == (object)Param2)
                Param2Changed(sender, e);
        }

        protected virtual void Param1Changed(object sender, EventArgs e)
        {
            //there are some simulations that need to react to this event
            //example: param2's bounds must change because param1's bounds have changed
        }

        protected virtual void Param2Changed(object sender, EventArgs e)
        {
            //there are some simulations that need to react to this event
            //example: param1's bounds must change because param2's bounds have changed
        }

        protected decimal GetNeighborsSum(T cellfield2d, int x, int y, bool wrap, decimal outsideVal = 0.0m)
        {
            return wrap ? GetNeighborsSumWithWrap(cellfield2d, x, y) : GetNeighborsSumWithoutWrap(cellfield2d, x, y, outsideVal);
        }

        protected decimal GetNeighborsSumWithWrap(T cellfield2d, int x, int y)
        {
            int yMax = aring.CellsY - 1;
            int xMax = aring.CellsX - 1;
            int yN = y == 0 ? yMax : y - 1;
            int yS = y == yMax ? 0 : y + 1;
            int xW = x == 0 ? xMax : x - 1;
            int xE = x == xMax ? 0 : x + 1;
            decimal v = 0;
            v += GetCellValue(cellfield2d, x, yN); //N
            v += GetCellValue(cellfield2d, xE, yN); //NE
            v += GetCellValue(cellfield2d, xE, y); //E
            v += GetCellValue(cellfield2d, xE, yS); //SE
            v += GetCellValue(cellfield2d, x, yS); //S
            v += GetCellValue(cellfield2d, xW, yS); //SW
            v += GetCellValue(cellfield2d, xW, y); //W
            v += GetCellValue(cellfield2d, xW, yN); //NW
            return v;
        }

        protected decimal GetNeighborsSumWithoutWrap(T cellfield2d, int x, int y, decimal outsideVal)
        {
            int yMax = aring.CellsY - 1;
            int xMax = aring.CellsX - 1;
            decimal v = 0;
            v += y == 0 ? outsideVal : GetCellValue(cellfield2d, x, y - 1); //N
            v += x == xMax || y == 0 ? outsideVal : GetCellValue(cellfield2d, x + 1, y - 1); //NE
            v += x == xMax ? outsideVal : GetCellValue(cellfield2d, x + 1, y); //E
            v += x == xMax || y == yMax ? outsideVal : GetCellValue(cellfield2d, x + 1, y + 1); //SE
            v += y == yMax ? outsideVal : GetCellValue(cellfield2d, x, y + 1); //S
            v += x == 0 || y == yMax ? outsideVal : GetCellValue(cellfield2d, x - 1, y + 1); //SW
            v += x == 0 ? outsideVal : GetCellValue(cellfield2d, x - 1, y); //W
            v += x == 0 || y == 0 ? outsideVal : GetCellValue(cellfield2d, x - 1, y - 1); //NW
            return v;
        }

        protected abstract decimal GetCellValue(T cellfield2d, int x, int y);

        protected abstract void SetCellValue(T cellfield2d, int x, int y, decimal value);


        //public methods:

        public abstract string Info { get; }

        public virtual decimal GetCellValue(int x, int y) => GetCellValue(aring.Last, x, y);

        public virtual void SetCellValue(int x, int y, decimal val) => SetCellValue(aring.Last, x, y, val);

        public void GoToOldestGen()
        {
            aring.RemoveAllExceptFirst();
            currentGen = 0;
        }

        public void RelabelCurrentAsZero()
        {
            aring.RemoveAllExceptLast();
            currentGen = 0;
        }

        public bool CalculateNextGen()
        {
            if (!DoCreateNewGeneration())
                return false;
            DoCalculateNextGen();
            ++currentGen;
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

        public SimulationParameter Param1 => param1;

        public SimulationParameter Param2 => param2;
    }
}
