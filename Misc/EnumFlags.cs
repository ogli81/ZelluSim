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
        protected object[] intValues;


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
            intValues = new object[Values.Length];
            int i = -1;
            foreach (object val in Values)
            {
                ++i;
                intValues[i] = Convert.ChangeType(val, valueType);
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
                if (valueType == typeof(long))
                {
                    for (int i = 0; i < intValues.Length; ++i)
                        bits |= (flags[i] ? (long)intValues[i] : 0);
                }
                else
                {
                    long converted;
                    for (int i = 0; i < intValues.Length; ++i)
                    {
                        converted = (long)System.Convert.ChangeType(intValues[i], typeof(long));
                        bits |= (flags[i] ? converted : 0);
                    }
                }
                return (Enum)Enum.ToObject(type, bits);
            }
            else //unsigned
            {
                //we take the biggest unsigned integer type: ulong
                ulong bits = 0;
                if (valueType == typeof(ulong))
                {
                    for (int i = 0; i < intValues.Length; ++i)
                        bits |= (flags[i] ? (ulong)intValues[i] : 0);
                }
                else
                {
                    ulong converted;
                    for (int i = 0; i < intValues.Length; ++i)
                    {
                        converted = (ulong)System.Convert.ChangeType(intValues[i], typeof(ulong));
                        bits |= (flags[i] ? converted : 0);
                    }
                }
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

        public void SetAllFlags(bool set)
        {
            //Array.Fill() not in my version of .Net :-(
            for (int i = 0; i < flags.Length; ++i)
                flags[i] = set;
        }

        public void CheckIntValue(object intValue)
        {
            if (intValue == null)
                throw new ArgumentNullException("can't be null!");

            switch (intValue.GetType().ToString())
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

                default: throw new ArgumentException($"Can't handle type {intValue.GetType()}");
            }

            if (!IntValueKnown(intValue))
                throw new ArgumentException($"value {intValue} not valid!");
        }

        public void SetFlag(object intValue, bool set)
        {
            CheckIntValue(intValue);
            int index = intValue.GetType().Equals(valueType) ?
                Array.IndexOf(intValues, intValue) :
                Array.IndexOf(intValues, Convert.ChangeType(intValue, valueType));
            flags[index] = set;
        }

        public bool IndexOkay(int index) => index >= 0 && index < Values.Length;

        public bool NameKnown(string name) => name != null && Array.IndexOf(Names, name) > -1;

        public bool IntValueKnown(object value) => value != null && Array.IndexOf(intValues, Convert.ChangeType(value, valueType)) > -1;

        public bool EnumOfCorrectType(Enum en) => en != null && en.GetType().Equals(type); 

        public Type Type => type;

        public Type IntValueType => valueType;
        
        public bool Signed => signed;

        public int NumValues => flags.Length;

        public string GetName(int index) => Names[index];

        public object GetIntValue(int index) => intValues[index];

        public bool GetFlag(int index) => flags[index]; //might throw out-of-bounds exception

        public bool GetFlag(string name)
        {
            if (!NameKnown(name))
                throw new ArgumentException($"name {name} not valid!");
            int index = Array.IndexOf(Names, name);
            return flags[index];
        }

        public bool GetFlag(object intValue)
        {
            CheckIntValue(intValue);
            intValue = Convert.ChangeType(intValue, valueType);
            int index = intValue.GetType().Equals(valueType) ?
                Array.IndexOf(intValues, intValue) :
                Array.IndexOf(intValues, Convert.ChangeType(intValue, valueType));
            return flags[index];
        }

        public bool IsZero(int index)
        {
            object val = GetIntValue(index);
            
            if(signed)
            {
                long converted = (long)System.Convert.ChangeType(val, typeof(long));
                return converted == 0;
            }
            else //unsigned
            {
                ulong converted = (ulong)System.Convert.ChangeType(val, typeof(ulong));
                return converted == 0;
            }
        }
    }
}
