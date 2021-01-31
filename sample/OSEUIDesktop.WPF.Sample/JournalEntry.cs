using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OSEUIDesktop.WPF.Sample
{
    public class JournalEntry : INotifyPropertyChanged
    {
        string _title;
        string _entry;
        ImageSource _image;
        DateTime _dateLine;

        public JournalEntry()
        {
            _title = "";
            _entry = "";
            _image = null;
            _dateLine = DateTime.MinValue;
        }
        public JournalEntry(JournalEntry d)
        {
            _title = d._title;
            _entry = d._entry;
            _image = d._image;
            _dateLine = d._dateLine;
        }
        public string Title { get => _title; set => _title = value; }
        public string Entry { get => _entry; set => _entry = value; }
        public ImageSource Image { get => _image; set => _image = value; }
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
            return _title.GetHashCode() ^ _entry.GetHashCode() ^ _image.GetHashCode() ^ _dateLine.GetHashCode();
        }
    }
}
