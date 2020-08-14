using System;
using System.Collections.Generic;
using System.Text;

namespace ZelluSim.Misc
{
    public class NamedObjects<T> : ObjectsWithIDs<string, T>
    {
        //we may want to add string-specific behavior later
        //for example we could add regex searches on names
    }
}
