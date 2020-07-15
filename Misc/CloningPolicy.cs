namespace ZelluSim.Misc
{
    /// <summary>
    /// In some places we might use the prototype pattern (having a "template" of 
    /// which we make clones/copies). The system can't know what's best and so we 
    /// must configure the behavior. For value types cloning doesn't make sense. 
    /// For reference types cloning makes only sense if the ICloneable interface 
    /// is correctly implemented.
    /// </summary>
    public enum CloningPolicy
    {
        /// <summary>
        /// Always try to detect ICloneable instances and use their clone method.
        /// </summary>
        TRY_DEEP_CLONE,

        /// <summary>
        /// Do not try to look for ICloneable (saves us quite some time).
        /// </summary>
        DO_NOT_TRY_DEEP_CLONE,

        /// <summary>
        /// Use the default behavior (see code documentation of our classes).
        /// </summary>
        USE_DEFAULT,

        /// <summary>
        /// Try to detect if any of the instances is ICloneable and if any of 
        /// them was in fact ICloneable then use deep cloning for all future 
        /// situations.
        /// </summary>
        AUTO_DETECT
    }
}
