using System;

namespace OSECore.Logging
{
    public interface IResultLogEventSource
    {
        event EventHandler<ResultLogEventArgs> StatusChanged;
        event EventHandler<ResultLogEventArgs> ResultAdded;
        event EventHandler<EventArgs> ResultLogReset;
        void RegisterEvents(IResultLogEvents e);
        void UnregisterEvents(IResultLogEvents e);
    }
}