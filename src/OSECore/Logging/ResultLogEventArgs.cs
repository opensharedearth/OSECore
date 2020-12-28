using System;

namespace OSECore.Logging
{
    public class ResultLogEventArgs : EventArgs
    {
        public ResultLogEventArgs(Result result)
        {
            Result = result;
        }

        public Result Result { get; }
    }
}