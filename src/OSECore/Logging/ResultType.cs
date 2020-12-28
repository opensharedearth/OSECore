using System.ComponentModel;

namespace OSECore.Logging
{
    /// <summary>
    /// Type of result: either good, bad or suspect.
    /// </summary>
    [TypeConverter(typeof(ResultTypeConverter))]
    public enum ResultType
    {
        /// <summary>
        /// Unknown result.  Invalid value.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Good result. String conversion to empty label.
        /// </summary>
        Good = 1,
        /// <summary>
        /// Suspect result.  String conversion to "Warning" label.
        /// </summary>
        Suspect = 2,
        /// <summary>
        /// Bad result.  String conversion to "Error" label.
        /// </summary>
        Bad = 3
    }
}