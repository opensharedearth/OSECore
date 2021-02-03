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
            try
            {
                var entries = new JournalEntries();
                int count = info.GetInt32("EntryCount");
                for(int i = 0; i < count; ++i)
                {
                    var entry = info.GetValue($"Entry{i}", typeof(JournalEntry)) as JournalEntry;
                    entries.Add(entry);
                }
                _entries = entries;
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Unable to read Sample document", ex);
            }
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("EntryCount", _entries.Count);
            for(int i = 0; i < _entries.Count; i++)
            {
                info.AddValue($"Entry{i}", _entries[i]);
            }
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
