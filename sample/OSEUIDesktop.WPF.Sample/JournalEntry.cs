using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OSEUIDesktop.WPF.Sample
{
    [Serializable]
    public class JournalEntry : INotifyPropertyChanged, ISerializable
    {
        string _title;
        string _entry;
        BitmapSource _image;
        DateTime _dateLine;

        public JournalEntry()
        {
            _title = "";
            _entry = "";
            _image = null;
            _dateLine = DateTime.Now;
        }
        public JournalEntry(JournalEntry d)
        {
            _title = d._title;
            _entry = d._entry;
            _image = d._image;
            _dateLine = d._dateLine;
        }
        public JournalEntry(SerializationInfo info, StreamingContext context)
        {
            try
            {
                _title = info.GetString("Title");
                _entry = info.GetString("Entry");
                _dateLine = info.GetDateTime("DateLine");
                byte[] bytes = info.GetValue("Image", typeof(byte[])) as byte[];
                if(bytes != null)
                {
                    MemoryStream ms = new MemoryStream(bytes);
                    PngBitmapDecoder decoder = new PngBitmapDecoder(ms, BitmapCreateOptions.None, BitmapCacheOption.None);
                    _image = decoder.Frames[0];
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Unable to deserialize journal entry.", ex);
            }
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Title", _title);
            info.AddValue("Entry", _entry);
            info.AddValue("DateLine", _dateLine);
            if(_image != null)
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(_image));
                MemoryStream ms = new MemoryStream();
                encoder.Save(ms);
                info.AddValue("Image", ms.ToArray());
            }
            else
            {
                info.AddValue("Image", null);
            }
        }
        public string Title
        {
            get => _title;
            set
            {
                if(_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Entry
        {
            get => _entry;
            set
            {
                if (_entry != value)
                {
                    _entry = value;
                    OnPropertyChanged();
                }
            }
        }
        public BitmapSource Image 
        { 
            get => _image; 
            set {
                if(_image != value)
                {
                    _image = value;
                    OnPropertyChanged();
                }
            }
        }
        public DateTime DateLine { get => _dateLine; set => _dateLine = value; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public override bool Equals(object obj)
        {
            if(obj is JournalEntry d)
            {
                return _title == d._title && _entry == d._entry && _image == d._image && _dateLine == d._dateLine;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return _title.GetHashCode() ^ _entry.GetHashCode() ^ (_image != null ? _image.GetHashCode() : 0) ^ _dateLine.GetHashCode();
        }

    }
}
