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
        //state:

        /// <summary>
        /// A short(!) name, that will be displayed above a slider.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// A few words/sentences about the nature of this parameter.
        /// </summary>
        public string Info { get; }

        private decimal min, max; //lower and upper bounds
        private decimal current; //current value

        /// <summary>
        /// This event will be triggered, whenever you set a property on this SimulationParameter. 
        /// The argument will always be empty (<see cref="EventArgs.Empty"/>).
        /// </summary>
        public event EventHandler ParamsChanged;


        //c'tors:

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
        /// <param name="name">A short(!) name, that will be displayed above a slider.</param>
        /// <param name="info">A few words/sentences about the nature of this parameter.</param>
        /// <param name="min">The lower bound for allowed values.</param>
        /// <param name="max">The upper bound for allowed values.</param>
        public SimulationParameter(string name, string info, decimal min, decimal max) : this(name, info, min, max, (max+min)*0.5m)
        {

        }

        /// <summary>
        /// This is the third c'tor variant.
        /// </summary>
        /// <param name="name">A short(!) name, that will be displayed above a slider.</param>
        /// <param name="info">A few words/sentences about the nature of this parameter.</param>
        /// <param name="min">The lower bound for allowed values.</param>
        /// <param name="max">The upper bound for allowed values.</param>
        /// <param name="current">The current value (must be inside lower/upper bounds).</param>
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
            
            this.min = min;
            this.max = max;
            this.current = current;

            ParamsChanged = null;
        }


        //helper methods:

        private void OnParamsChanged(EventArgs e)
        {
            //EventHandler handler = ParamsChanged;
            //if (handler != null)
            //    handler(this, e);
            ParamsChanged?.Invoke(this, e);
        }


        //public methods:

        /// <summary>
        /// Get or set the lower bound (smallest legal value for the 'current' value).
        /// </summary>
        public decimal Min 
        {
            get => min;
            set
            {
                if (value > max) 
                    throw new ArgumentException("min must be smaller than max!");
                min = value;
                if (value > current)
                    current = value;
                OnParamsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Get or set the upper bound (biggest legal value for the 'current' value).
        /// </summary>
        public decimal Max 
        {
            get => max;
            set
            {
                if (value < min)
                    throw new ArgumentException("min must be smaller than max!");
                max = value;
                if (value < current)
                    current = value;
                OnParamsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Get or set the 'current' value (must be within min/max, lower/upper bounds).
        /// </summary>
        public decimal Current 
        {
            get => current;
            set
            {
                if (value < Min) 
                    throw new ArgumentException("Current must be greater than or equal to Min!");
                if (value > Max) 
                    throw new ArgumentException("Current musst be less than or equal to Max!");
                current = value;
                OnParamsChanged(EventArgs.Empty);
            }
        }
    }
}
