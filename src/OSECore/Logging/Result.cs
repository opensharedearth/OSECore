// Open Shared Earth, LCC licenses this file to you under the MIT license.
// See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace OSECore.Logging
{
    /// <summary>
    /// A single result from an operation that is classified as either good, bad or suspect and contains a description.  A string conversion of a result is normally
    /// a single line of text.
    /// </summary>
    public class Result
    {
        public const string DefaultGoodFormat = "{0}";
        public const string DefaultSuspectFormat = "Warning: {0}";
        public const string DefaultBadFormat = "Error: {0}";
        public static string GoodFormat { get; set; } = DefaultGoodFormat;
        public static string SuspectFormat { get; set; } = DefaultSuspectFormat;
        public static string BadFormat { get; set; } = DefaultBadFormat;
        /// <summary>
        /// Construct a result from type and description.
        /// </summary>
        /// <param name="type">The result type, either good, suspect or error.</param>
        /// <param name="description">The description of the result.</param>
        public Result(ResultType type, string description)
        {
            Type = type;
            Description = description;
        }
        /// <summary>
        /// Type of result.
        /// </summary>
        public ResultType Type { get; private set; }
        /// <summary>
        /// Descriptiom of result.
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// Convert result to a string description.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (Type)
            {
                case ResultType.Bad:
                    return String.Format(BadFormat, Description);
                case ResultType.Suspect:
                    return String.Format(SuspectFormat, Description);
                default:
                    return String.Format(GoodFormat, Description);
            }
        }

        public static Result GetMostSevere(params Result[] results)
        {
            Result r1 = null;
            foreach (Result r0 in results)
            {
                if (r1 == null)
                {
                    r1 = r0;
                }
                else if (r1.Type == r0.Type)
                {
                    r1 = r0;
                }
                else if ((int) r0.Type > (int) r1.Type)
                {
                    r1 = r0;
                }
            }

            return r1;
        }
    }
}