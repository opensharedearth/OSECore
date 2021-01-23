using System;

namespace OSECoreUI.App
{
    public interface IDirtyEventSource
    {
        event EventHandler<EventArgs> Dirtied;
        event EventHandler<EventArgs> Undirtied;
        void RegisterEvents(IDirtyEvents events);
        void UnregisterEvents(IDirtyEvents events);
        void OnDirtied();
        void OnUndirtied();
    }
}