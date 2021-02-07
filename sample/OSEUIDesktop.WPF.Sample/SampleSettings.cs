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
        int _dateFormatIndex = 7;
        public SampleSettings()
        {
        }
        public SampleSettings(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            try
            {
                _dateFormatIndex = info.GetInt32("DateFormatIndex");
            }
            catch(SerializationException ex)
            {
                System.Diagnostics.Trace.WriteLine("Failed to completely serialization SampleSettings: " + ex.Message);
            }
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("DateFormatIndex", _dateFormatIndex);
            base.GetObjectData(info, context);
        }
        public override bool IsDirty => base.IsDirty;

        public int DateFormatIndex
        {
            get => _dateFormatIndex;
            set 
            {
                if(_dateFormatIndex != value)
                {
                    _dateFormatIndex = value;
                    Dirty();
                }
            }
        }
        public string DateFormatString => DateFormats[_dateFormatIndex].Format;
        public struct DateFormat
        {
            public DateFormat(string name, string format)
            {
                Name = name;
                Format = format;

            }
            public string Name { get; set; }
            public string Format { get; set; }
            public string Example => DateTime.Now.ToString(Format);
        };
        public DateFormat[] DateFormats { get; } = new DateFormat[] {
            new DateFormat("Short date", "d"),
            new DateFormat("Long date", "D"),
            new DateFormat("Full/short time", "f"),
            new DateFormat("Full/long time", "F"),
            new DateFormat("General", "g"),
            new DateFormat("Universal", "d-MMM-yy"),
            new DateFormat("Universal/short time", "d-MMM-yy h:mm tt"),
            new DateFormat("Universal/long time", "d-MMM-yy h:mm:ss tt")
        };
    }
}
