using System;

namespace OSECore.Logging
{
    public interface IResultLogEvents
    {
        void StatusChanged(object sender, ResultLogEventArgs args);
        void ResultAdded(object sender, ResultLogEventArgs args);
        void ResultLogReset(object sender, EventArgs args);
    }
}