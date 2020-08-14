using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZelluSim.Misc
{
    /// <summary>
    /// This data structure maps keys to values like a dictionary/hashtable/hashmap. 
    /// It also always has one "current object" that reflects the object which is 
    /// currently selected/active.
    /// </summary>
    /// <typeparam name="I">the generic type for the IDs</typeparam>
    /// <typeparam name="O">the generic type for the objects</typeparam>
    public class ObjectsWithIDs<I, O>
    {
        //state:

        //usually this should not be necessary
        //we may want to let the user configure this behavior in later versions
        protected static IEqualityComparer<O> comparer = new RefEqual<O>();

        //the underlying data structure
        protected Dictionary<I, O> map = new Dictionary<I, O>();

        //the "current object" (currently active/selected entry)
        protected O currentObject = default(O);

        /// <summary>
        /// This property depends on the type of c'tor that you've used. 
        /// If you provide an initial object and intial key(s) this property
        /// will be false. If you use the empty (default) c'tor this property
        /// will be true. <br></br>
        /// If this property is false then you must always have at least one
        /// object in this data structure and you can't simply remove the
        /// current object (<see cref="ObjectsWithIDs{I, T}.CurrentObject"/>). 
        /// If this property is true, the current object will be the default
        /// (usually 'null') and the data structure will start with zero
        /// objects (and zero ID keys registered).
        /// </summary>
        public bool CanBeEmpty { get; }


        //c'tors:

        public ObjectsWithIDs()
        {
            CanBeEmpty = true;
        }

        public ObjectsWithIDs(I initialID, O initialObject) : this(new I[] { initialID }, initialObject)
        {
        }

        public ObjectsWithIDs(I[] initialIDs, O initialObject)
        {
            if (initialIDs == null)
                throw new ArgumentNullException("can't be null!");
            if (initialIDs.Length < 1)
                throw new ArgumentException("can't be empty!");
            if (initialObject == null)
                throw new ArgumentException("can't be null!");
            Register(initialIDs, initialObject);
            currentObject = initialObject;
            CanBeEmpty = false;
        }


        //public methods:

        public O CurrentObject
        {
            get => currentObject;
            set
            {
                if (HasObject(value))
                    currentObject = value;
            }
        }

        public int NumIDs => map.Keys.Count;
        public int NumObjects => map.Values.Count;

        public I GetID(int i) => map.Keys.ElementAt(i);
        public O GetObject(int i) => map.Values.ElementAt(i);

        public bool HasID(I id) => map.Keys.Contains(id);
        public bool HasObject(O ob) => map.Values.Contains(ob, comparer);

        public bool GetObject(I id, out O ob) => map.TryGetValue(id, out ob);

        public bool Register(I id, O ob)
        {
            if (ob == null)
                throw new ArgumentException("can't be null!");

            bool hasAlready = map.ContainsKey(id);
            map.Add(id, ob);
            return hasAlready;
        }
        public bool Register(I[] ids, O ob)
        {
            if (ids == null)
                throw new ArgumentNullException("can't be null!");
            if (ids.Length < 1)
                throw new ArgumentException("can't be empty!");
            if (ob == null)
                throw new ArgumentException("can't be null!");

            bool any = false;
            foreach (I id in ids)
                any |= Register(id, ob);
            return any;
        }

        public bool UnregisterID(I id)
        {
            if (!HasID(id))
                return false;
            if (NumObjects == 1 && !CanBeEmpty)
                return false;
            if (GetObject(id, out O ob) && ReferenceEquals(ob, currentObject))
                if (map.Where(kvp => ReferenceEquals(kvp.Value, currentObject)).ToList().Count == 1)
                    if (CanBeEmpty)
                        currentObject = default(O);
                    else
                        return false;

            return map.Remove(id);
        }
        public bool UnregisterObject(O ob)
        {
            if (ob == null)
                throw new ArgumentException("can't be null!");

            if (!HasObject(ob))
                return false;
            if (NumObjects == 1 && !CanBeEmpty)
                return false;
            if (ReferenceEquals(currentObject,ob))
                if (CanBeEmpty)
                    currentObject = default(O);
                else
                    return false;

            //solution by Paul Turner @ stackoverflow.com
            foreach (KeyValuePair<I,O> item in map.Where(kvp => ReferenceEquals(kvp.Value,ob)).ToList())
                map.Remove(item.Key);
            return true;
        }
    }
}
