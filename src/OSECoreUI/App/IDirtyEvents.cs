using System;

namespace OSECoreUI.App
{
    public interface IDirtyEvents
    {
        void DirtiedHandler(object sender, EventArgs args);
        void UndirtiedHandler(object sender, EventArgs args);
    }
}