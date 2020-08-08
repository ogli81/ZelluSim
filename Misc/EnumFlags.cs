using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace ZelluSim.Misc
{
    /// <summary>
    /// Helps us with the management of <see cref="Enum"/> subclasses that have a 
    /// <see cref="FlagsAttribute"/> associated ([Flags]). These enums can be 
    /// treated as bitfields/bitmasks/flags.
    /// </summary>
    public class EnumFlags
    {
        //state:

        protected Type type;
        protected Type valueType;
        protected bool signed;
        protected readonly bool[] flags;
        protected string[] Names => Enum.GetNames(type);
        protected Array Values => Enum.GetValues(type);


        //c'tor:

        public EnumFlags(Enum en)
        {
            if (!Enums.EnumHasFlags(en))
                throw new ArgumentException("Enum should have flags!");

            type = en.GetType();
            valueType = Enum.GetUnderlyingType(type);
            switch (valueType.ToString())
            {
                case "System.SByte":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64": signed = true; break;

                case "System.Byte":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64": signed = false; break;

                //we should never end here, because only the 8 integer types (see above) are allowed
                default: throw new ArgumentException($"Can't handle type {Enum.GetUnderlyingType(type)}");
            }

            flags = new bool[Values.Length];
            UpdateFrom(en); //fills the 'flags' array
        }


        //public methods:

        public void UpdateFrom(Enum en)
        {
            if(!EnumOfCorrectType(en))
                throw new ArgumentException("Wrong Enum type!");

            int i = -1;
            foreach (object val in Values)
            {
                //TODO: test if this is okay:
                //flags[++i] = en.HasFlag((Enum)(Enum.ToObject(type, Values.GetValue(i))));
                ++i;
                flags[i] = en.HasFlag((Enum)(Enum.ToObject(type, Values.GetValue(i))));
            }
        }

        public Enum Create()
        {
            if (signed)
            {
                //we take the biggest signed integer type: long
                long bits = 0;
                for (int i = 0; i < Values.Length; ++i)
                    bits |= (flags[i] ? (long)Values.GetValue(i) : 0);
                return (Enum)Enum.ToObject(type, bits);
            }
            else
            {
                //we take the biggest unsigned integer type: ulong
                ulong bits = 0;
                for (int i = 0; i < Values.Length; ++i)
                    bits |= (flags[i] ? (ulong)Values.GetValue(i) : 0);
                return (Enum)Enum.ToObject(type, bits);
            }
        }

        public void SetFlag(int index, bool set)
        {
            if (!IndexOkay(index))
                throw new ArgumentException($"index {index} not valid!");
            flags[index] = set;
        }

        public void SetFlag(string name, bool set)
        {
            if (!NameKnown(name))
                throw new ArgumentException($"name {name} not valid!");
            int index = Array.IndexOf(Names, name);
            flags[index] = set;
        }

        public void CheckValue(object value)
        {
            if (value == null)
                throw new ArgumentNullException("can't be null!");

            switch (value.GetType().ToString())
            {
                case "System.SByte":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                    if (!signed)
                        throw new ArgumentException("We operate with signed integers!");
                    break;

                case "System.Byte":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                    if (signed)
                        throw new ArgumentException("We operate with unsigned integers!");
                    break;

                default: throw new ArgumentException($"Can't handle type {value.GetType()}");
            }

            if (!ValueKnown(value))
                throw new ArgumentException($"value {value} not valid!");
        }

        public void SetFlag(object value, bool set)
        {
            CheckValue(value);
            value = Convert.ChangeType(value, valueType);
            int index = Array.IndexOf(Values, value);
            flags[index] = set;
        }

        public bool IndexOkay(int index) => index >= 0 && index < Values.Length;

        public bool NameKnown(string name) => name != null && Array.IndexOf(Names, name) > -1;

        public bool ValueKnown(object value) => value != null && Array.IndexOf(Values, Convert.ChangeType(value, valueType)) > -1;

        public bool EnumOfCorrectType(Enum en) => en != null && en.GetType().Equals(type); 

        public Type Type => type;

        public Type ValueType => valueType;
        
        public bool Signed => signed;

        public int NumValues => flags.Length;

        public string GetName(int index) => Names[index];

        public object GetValue(int index) => Values.GetValue(index);

        public bool GetFlag(int index) => flags[index]; //might throw out-of-bounds exception

        public bool GetFlag(string name)
        {
            if (!NameKnown(name))
                throw new ArgumentException($"name {name} not valid!");
            int index = Array.IndexOf(Names, name);
            return flags[index];
        }

        public bool GetFlag(object value)
        {
            CheckValue(value);
            value = Convert.ChangeType(value, valueType);
            int index = Array.IndexOf(Values, value);
            return flags[index];
        }
    }
}
