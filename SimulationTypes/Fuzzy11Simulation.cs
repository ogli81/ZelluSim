using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZelluSim.CellField;

namespace ZelluSim.SimulationTypes
{
    /// <summary>
    /// A variant of the classic Game of Life simulation which uses 11 different states. 
    /// A value of 0 (zero) means that the cell is 100% dead (0% alive). 
    /// A value of 10 (ten) means that the cell is 100% alive (0% damaged).
    /// <list type="bullet">
    /// <item>
    /// Param1: min combined 'life' of neighbors to survive without damage, default: 20
    /// </item>
    /// <item>
    /// Param2: min combined 'life' of neighbors where overpopulation starts (with 1 pt damage), default: 31
    /// </item>
    /// </list>
    /// </summary>
    public class Fuzzy11Simulation : GenericCellSimulation<byte>
    {
        //state:

        //-


        //c'tors:

        public Fuzzy11Simulation(SimulationSettings settings) : base(settings)
        {
            Init();
        }


        //helper methods:

        protected override void CreateParams()
        {
            param1 = new SimulationParameter("minimum survival", "combined life value of neighbors needed to survive unharmed", 0, 90, 20);
            param2 = new SimulationParameter("overpopulation", "combined life where overpopulation starts (with 1 pt damage)", 1, 91, 31);
        }

        protected override void DoCalculateNextGen()
        {
            throw new NotImplementedException(); //TODO
        }

        protected override void Param1Changed(object sender, EventArgs e)
        {
            if (param1.Current >= param2.Current)
                param2.Current = param1.Current + 1;
        }

        protected override void Param2Changed(object sender, EventArgs e)
        {
            if (param2.Current <= param1.Current)
                param1.Current = param2.Current - 1;
        }

        protected override decimal GetCellValue(IGenericCellField2D<byte> cells, int x, int y)
        {
            switch (cells[x, y])
            {
                case 0: return 0.0m;
                case 1: return 0.1m;
                case 2: return 0.2m;
                case 3: return 0.3m;
                case 4: return 0.4m;
                case 5: return 0.5m;
                case 6: return 0.6m;
                case 7: return 0.7m;
                case 8: return 0.8m;
                case 9: return 0.9m;
                case 10: return 1.0m;
                default: throw new ArgumentException("Only values 0,1,2,3,4,5,6,7,8,9,10 are allowed here!");
            }
        }

        protected override void SetCellValue(IGenericCellField2D<byte> cells, int x, int y, decimal val)
        {
            decimal n = val * 10m;
            byte target = n >= 10m ? (byte)10 : n <= 0 ? (byte)0 : (byte)128;
            cells[x, y] = target != 128 ? target : (byte)(Math.Ceiling(n));
        }


        //public methods:

        public override string Info => "A modified version of the life simulation by John Horton Conway - we use 11 different states: 0,1,2,3,4,5,6,7,8,9,10 (0 = fully dead, 10 = fully alife).";
    }
}
