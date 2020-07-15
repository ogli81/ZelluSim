using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZelluSim.SimulationTypes
{
    /// <summary>
    /// Each simulation may have up to 2 adjustable parameters. Those parameters control how 
    /// the simulation behaves (e.g. "how much food is needed for a cell to survive?"). 
    /// In our application, the user may change these parameters and observe how the 
    /// simulation behaves with different parameter values.
    /// </summary>
    public struct SimulationParameter
    {
        /// <summary>
        /// This is the first c'tor variant. It will use the interval [0..1] (minimum is 0.0m 
        /// and maximum is 1.0m).
        /// </summary>
        /// <param name="name">A short(!) name, that will be displayed above a slider.</param>
        /// <param name="info">A few words/sentences about the nature of this parameter.</param>
        public SimulationParameter(string name, string info) : this(name, info, 0.0m, 1.0m)
        {

        }

        /// <summary>
        /// This is the second c'tor variant. You can specify the minimum and maximum of the
        /// interval that defines the possible values. The current value of this parameter
        /// will be in the middle of the interval (at 50% so to say).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="info"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public SimulationParameter(string name, string info, decimal min, decimal max) : this(name, info, min, max, (max+min)*0.5m)
        {

        }

        /// <summary>
        /// This is the third c'tor variant.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="info"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="current"></param>
        public SimulationParameter(string name, string info, decimal min, decimal max, decimal current)
        {
            if (name == null)
                throw new ArgumentNullException("name must not be null!");
            if (info == null)
                throw new ArgumentNullException("info must not be null!");
            if (min > max)
                throw new ArgumentException("min must be smaller than max!");
            if (current < min)
                throw new ArgumentException("current must be greater than or equal to min!");
            if (current > max)
                throw new ArgumentException("current musst be less than or equal to max!");

            Name = name;
            Info = info;
            Min = min;
            Max = max;
            curr = current;
        }

        public string Name { get; }
        public string Info { get; }

        public decimal Min { get; }
        public decimal Max { get; }

        private decimal curr;
        public decimal Current 
        {
            get => curr;
            set
            {
                if (value < Min) 
                    throw new ArgumentException("Current must be greater than or equal to Min!");
                if (value > Max) 
                    throw new ArgumentException("Current musst be less than or equal to Max!");
                curr = value;
            }
        }
    }
}
