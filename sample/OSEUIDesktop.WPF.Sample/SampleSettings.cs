using OSEUI.WPF.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OSEUIDesktop.WPF.Sample
{
    [Serializable]
    public class SampleSettings : DesktopAppSettings
    {
        public SampleSettings()
        {
        }
        public SampleSettings(SerializationInfo info, StreamingContext context)
            : base(info,context)
        {

        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        public override bool IsDirty => base.IsDirty;

    }
}
