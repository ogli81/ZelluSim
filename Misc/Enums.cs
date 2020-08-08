using System;
using System.Linq;
using System.Reflection;

namespace ZelluSim.Misc
{
    /// <summary>
    /// Static utility class that provides more information about special objects of type <see cref="Enum"/>.
    /// </summary>
    public static class Enums
    {
        //static methods:

        public static bool EnumHasFlags(Enum en) => en.GetType().GetCustomAttributes<FlagsAttribute>().Any();
    }
}
