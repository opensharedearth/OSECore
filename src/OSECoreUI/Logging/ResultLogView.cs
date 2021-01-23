using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using OSECore.Logging;
using OSECoreUI.Annotations;

namespace OSECoreUI.Logging
{
    public sealed class ResultLogView : IList<Result>, IBindingList, INotifyPropertyChanged, IDisposable, IResultLogEvents, INotifyCollectionChanged
    {
        public ResultLogView(ResultLog log)
        {
            _log = log;
            log.RegisterEvents(this);
            UpdateView();
        }

        event ListChangedEventHandler IBindingList.ListChanged
        {
            add => ListChanged += value;

            remove => ListChanged -= value;
        }

        public event ListChangedEventHandler ListChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable Items
        {
            get
            {
                return this;
            }
        }

        private void UpdateView()
        {
            _results.Clear();
            foreach(Result result in _log)
            {
                UpdateView(result);
            }
        }

        private void UpdateView(Result result)
        {
            switch (result.Type)
            {
                case ResultType.Bad:
                    if (ShowBad)
                    {
                        _results.Add(result);
                    }
                    break;
                case ResultType.Good:
                    if (ShowGood)
                    {
                        _results.Add(result);
                    }
                    break;
                case ResultType.Suspect:
                    if (ShowSuspect)
                    {
                        _results.Add(result);
                    }
                    break;
                case ResultType.Unknown:
                default:
                    _results.Add(result);
                    break;
            }
        }

        ResultLog _log = null;
        List<Result> _results = new List<Result>();
        bool _showBad = true;
        bool _showGood = true;
        bool _showSuspect = true;

        public int Count
        {
            get
            {
                return _results.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public bool ShowBad
        {
            get
            {
                return _showBad;
            }

            set
            {
                if(_showBad != value)
                {
                    _showBad = value;
                    UpdateView();
                    OnListReset();
                }
            }
        }

        public Result StatusLine => _log.StatusLine;
        public bool HasStatusLine => _log.StatusLine != null;

        private void OnListReset()
        {
            ListChangedEventHandler handler = ListChanged;
            handler?.Invoke(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
        }

        public void OnItemAdded()
        {
            ListChangedEventHandler handler = ListChanged;
            handler?.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemAdded, Count));
        }
        public bool ShowGood
        {
            get
            {
                return _showGood;
            }

            set
            {
                if(_showGood != value)
                {
                    _showGood = value;
                    UpdateView();
                    OnListReset();
                }
            }
        }

        public bool ShowSuspect
        {
            get
            {
                return _showSuspect;
            }

            set
            {
                if(_showSuspect != value)
                {
                    _showSuspect = value;
                    UpdateView();
                    OnListReset();
                }
            }
        }

        bool IBindingList.AllowNew
        {
            get
            {
                return false;
            }
        }

        bool IBindingList.AllowEdit
        {
            get
            {
                return false;
            }
        }

        bool IBindingList.AllowRemove
        {
            get
            {
                return false;
            }
        }

        bool IBindingList.SupportsChangeNotification
        {
            get
            {
                return true;
            }
        }

        bool IBindingList.SupportsSearching
        {
            get
            {
                return false;
            }
        }

        bool IBindingList.SupportsSorting
        {
            get
            {
                return false;
            }
        }

        bool IBindingList.IsSorted
        {
            get
            {
                return false;
            }
        }

        PropertyDescriptor IBindingList.SortProperty
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        ListSortDirection IBindingList.SortDirection
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return _results;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        int ICollection.Count
        {
            get
            {
                return _results.Count;
            }
        }

        public ResultLog ResultLog
        {
            get { return _log; }
        }

        object IList.this[int index]
        {
            get
            {
                return _results[index];
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Result this[int index]
        {
            get
            {
                return _results[index];
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        void ICollection<Result>.Add(Result item)
        {
            throw new NotImplementedException();
        }

        void ICollection<Result>.Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(Result item)
        {
            return _results.Contains(item);
        }

        void ICollection<Result>.CopyTo(Result[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return _results.GetEnumerator();
        }

        IEnumerator<Result> IEnumerable<Result>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(Result item)
        {
            return _results.IndexOf(item);
        }

        void IList<Result>.Insert(int index, Result item)
        {
            throw new NotImplementedException();
        }

        bool ICollection<Result>.Remove(Result item)
        {
            throw new NotImplementedException();
        }

        void IList<Result>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        object IBindingList.AddNew()
        {
            throw new NotImplementedException();
        }

        void IBindingList.AddIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new NotImplementedException();
        }

        int IBindingList.Find(PropertyDescriptor property, object key)
        {
            throw new NotImplementedException();
        }

        void IBindingList.RemoveIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        void IBindingList.RemoveSort()
        {
            throw new NotImplementedException();
        }

        int IList.Add(object value)
        {
            throw new NotImplementedException();
        }

        bool IList.Contains(object value)
        {
            return _results.Contains(value as Result);
        }

        void IList.Clear()
        {
            throw new NotImplementedException();
        }

        int IList.IndexOf(object value)
        {
            return _results.IndexOf(value as Result);
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        void IList.Remove(object value)
        {
            throw new NotImplementedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            foreach(Result r in _results)
            {
                array.SetValue(r, index++);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _results.GetEnumerator();
        }

        public void Dispose()
        {
            _log?.UnregisterEvents(this);
        }

        public void StatusChanged(object sender, ResultLogEventArgs args)
        {
            OnPropertyChanged(nameof(StatusLine));
            OnPropertyChanged(nameof(HasStatusLine));
        }

        public void ResultAdded(object sender, ResultLogEventArgs args)
        {
            UpdateView(args.Result);
            OnItemAdded();
            OnCollectionItemAdded(args.Result);
        }

        public void ResultLogReset(object sender, EventArgs args)
        {
            UpdateView();
            OnListReset();
            OnCollectionReset();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionReset()
        {
            NotifyCollectionChangedEventHandler handler = CollectionChanged;
            handler?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void OnCollectionItemAdded(Result result)
        {
            NotifyCollectionChangedEventHandler handler = CollectionChanged;
            handler?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, result));
        }
    }
}
