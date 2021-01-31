using OSECoreUI.Document;
using OSEUI.WPF.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OSEUIDesktop.WPF.Sample
{
    [Serializable]
    public sealed class SampleDocument : DesktopDocument
    {
        JournalEntries _entries = new JournalEntries();
        public SampleDocument()
        {
            Setup();
        }
        public SampleDocument(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        protected override void Setup()
        {
            RequiredVersion = 1;
            DocType = DocTypeRegistry.Instance["Sample"];
            base.Setup();
        }
        public override bool IsDirty => base.IsDirty;

        public JournalEntries Entries { get => _entries; }
    }
}
