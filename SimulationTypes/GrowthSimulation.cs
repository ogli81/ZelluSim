using System;
using ZelluSim.CellField;
using ZelluSim.Misc;

namespace ZelluSim.SimulationTypes
{
    /// <summary>
    /// Simulates growth: The cells either grow by themselves (if not 100% dead) or 
    /// they get help from their neighbors (if there are any). You can configure 
    /// how strong the influence from within the cell itself or from its neighbors 
    /// will be via the two params.
    /// <list type="bullet">
    /// <item>
    /// Param1: growth influence due to the current value of the cell itself (default: 0.5). This param 
    /// is actually based on the square root of the influence factor (a value of 0.5 actually means an 
    /// influence of 0.25 and a value of 0.1 means an influence of 0.01).
    /// </item>
    /// <item>
    /// Param1: growth influence due to the values of the neighbors of the cell (default: 0.2). This param 
    /// is actually based on the square root of the influence factor (a value of 0.5 actually means an 
    /// influence of 0.25 and a value of 0.1 means an influence of 0.01).
    /// </item>
    /// </list>
    /// </summary>
    public class GrowthSimulation : GenericCellSimulation<decimal>
    {
        //state:

        protected decimal influenceSelf = 0.5m * 0.5m;
        protected decimal influenceOuter = 0.2m * 0.2m;


        //c'tors:

        public GrowthSimulation(SimulationSettings settings) : base(settings)
        {
            Init();
        }


        //helper methods:

        protected override void CreateParams()
        {
            param1 = new SimulationParameter("self-influence", "the influence on growth due to the current value of the cell", 0, 0.5m, 2);
            param2 = new SimulationParameter("neighbor-influence", "the influence on growth due to the current value of the neighbors", 0, 0.2m, 2);
        }

        protected override void DoCalculateNextGen()
        {
            IGenericCellField2D<decimal> last = ring.Last;
            IGenericCellField2D<decimal> prev = ring.Previous;

            bool wrap = Settings.IsWrap;
            int xBound = prev.CellsX;
            int yBound = prev.CellsY;
            decimal self, outer;

            for (int x = 0; x < xBound; ++x)
                for (int y = 0; y < yBound; ++y)
                {
                    self = prev[x, y];
                    outer = GetNeighborsSum(prev, x, y, wrap);
                    last[x, y] = self + (influenceSelf * self) + (influenceOuter * outer);
                }
        }

        protected override void Param1Changed(object sender, EventArgs e)
        {
            influenceSelf = DecimalMath.Sqrt(param1.Current);
        }

        protected override void Param2Changed(object sender, EventArgs e)
        {
            influenceOuter = DecimalMath.Sqrt(param2.Current);
        }

        protected override decimal GetCellValue(IGenericCellField2D<decimal> cellfield2d, int x, int y) => cellfield2d[x, y];

        protected override void SetCellValue(IGenericCellField2D<decimal> cellfield2d, int x, int y, decimal val)
        {
            cellfield2d[x, y] = val > 1.0m ? 1.0m : val < 0m ? 0m : val;
        }


        //public methods:

        public override string Info => "A growth simulation. Cells either grow by themselves (if not dead) or due to their neighbors influence.";

        public void SetSelfInfluence(decimal value)
        {
            decimal pVal = DecimalMath.Sqrt(value); //may throw exception
            param1.Current = (pVal < param1.Min) ? param1.Min : (pVal > param1.Max) ? param1.Max : pVal;
        }

        public void SetOuterInfluence(decimal value)
        {
            decimal pVal = DecimalMath.Sqrt(value); //may throw exception
            param2.Current = (pVal < param2.Min) ? param2.Min : (pVal > param2.Max) ? param2.Max : pVal;
        }
    }
}
