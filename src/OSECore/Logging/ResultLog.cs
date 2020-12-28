// Open Shared Earth, LCC licenses this file to you under the MIT license.
// See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace OSECore.Logging
{
    /// <summary>
    /// A list of results. Typically holds the results from a series of operations.
    /// </summary>
    /// <example>
    /// ResultLog log0 = new ResultLog("First log");<br />
    ///  log0.LogBad("This is an error message.");<br />
    ///  log0.LogSuspect("This is a warning message");<br />
    ///  ResultLog log1 = new ResultLog("Second Log", log0);<br />
    ///  log1.LogGood("This is a good message.");<br />
    ///  Console.Write(log1);<br />
    ///  // Result:<br />
    ///  // Error: This is an error message.<br />
    ///  // Warning: This is a warning message<br />
    ///  // This is a good message.
    /// </example>
    public class ResultLog : IList<Result>, IErrorMessage, IResultLogEventSource
    {
        private List<Result> _list;
        private Result _statusLine;

        /// <summary>
        /// Default constructor.  Creates an empty list.
        /// </summary>
        public ResultLog()
        {
            Caption = "";
            _statusLine = null;
            _list = new List<Result>();
            Reset();
        }
        /// <summary>
        /// Constuctor which takes a caption and an operation array of result logs.  Can be used to combine a series of result logs under a single caption.
        /// </summary>
        /// <param name="caption">The tile of this list of results.</param>
        /// <param name="logs">Zero or more result logs to be merged into this log.</param>
        public ResultLog(string caption, params ResultLog[] logs)
        {
            _statusLine = null;
            _list = new List<Result>();
            Caption = caption;
            Reset();
            foreach (var log in logs)
                AddRange(log);
        }
        /// <summary>
        /// The title to be applied to this list of results.  Suitable for a caption
        /// </summary>
        public string Caption { get; private set; }
        /// <summary>
        /// Count of good results in log.
        /// </summary>
        public int GoodCount { get; private set; }
        /// <summary>
        /// Count of bad results in log.
        /// </summary>
        public int BadCount { get; private set; }
        /// <summary>
        /// Count of suspect results in log.
        /// </summary>
        public int SuspectCount { get; private set; }
        /// <summary>
        /// Returns true if there are good results and no bad or suspect results in log.
        /// </summary>
        public bool IsGood => GoodCount > 0 && BadCount == 0 && SuspectCount == 0;
        /// <summary>
        /// Returns true if there are bad results in log.
        /// </summary>
        public bool IsBad => BadCount > 0;
        /// <summary>
        /// Returns true if there are suspect results in log.
        /// </summary>
        public bool IsSuspect => SuspectCount > 0 && BadCount == 0;
        /// <summary>
        /// Returns true if the log is empty.
        /// </summary>
        public bool IsNull => Count == 0;

        public bool HasError => IsBad;

        /// <summary>
        /// Resets the counts.
        /// </summary>
        private void Reset()
        {
            GoodCount = 0;
            BadCount = 0;
            SuspectCount = 0;
            ResultLogReset?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Log a good result.
        /// </summary>
        /// <param name="description">Description of the result.</param>
        public void LogGood(string description)
        {
            LogResult(ResultType.Good, description);
        }
        /// <summary>
        /// Log a bad result.
        /// </summary>
        /// <param name="description">Description of the result.</param>
        public void LogBad(string description)
        {
            LogResult(ResultType.Bad, description);
        }
        /// <summary>
        /// Log a suspect result.
        /// </summary>
        /// <param name="description">Description of the result</param>
        public void LogSuspect(string description)
        {
            LogResult(ResultType.Suspect, description);
        }
        /// <summary>
        /// Log a result with type.
        /// </summary>
        /// <param name="type">Type of result.</param>
        /// <param name="description">Description of the result.</param>
        private void LogResult(ResultType type, string description)
        {
            LogResult(new Result(type, description));
        }
        /// <summary>
        /// Add a result to the log.
        /// </summary>
        /// <param name="result">The result to be added to the log.</param>
        private void LogResult(Result result)
        {
            Add(result);
            switch (result.Type)
            {
                case ResultType.Good:
                    GoodCount++;
                    break;
                case ResultType.Bad:
                    BadCount++;
                    break;
                case ResultType.Suspect:
                    SuspectCount++;
                    break;
            }

            OnResultAdded(result);

            UpdateStatus(result);
        }

        private void UpdateStatus(Result result)
        {
            if (_statusLine == null)
            {
                StatusLine = result;
            }
            else
            {
                StatusLine = Result.GetMostSevere(_statusLine, result);
            }
        }

        public void Add(Result item)
        {
            _list.Add(item);
        }

        /// <summary>
        /// Clear the counts and empty the log.
        /// </summary>
        public void Clear()
        {
            _statusLine = null;
            _list.Clear();
            Reset();
        }

        public bool Contains(Result item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(Result[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(Result item)
        {
            return _list.Remove(item);
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public Result StatusLine
        {
            get => _statusLine;
            private set
            {
                if (_statusLine != value)
                {
                    _statusLine = value;
                    OnStatusChanged();
                }
            } 
        }

        /// <summary>
        /// Add a range of results.
        /// </summary>
        /// <param name="results">The results to be added.</param>
        public void AddRange(IEnumerable<Result> results)
        {
            var enumerable = results as Result[] ?? results.ToArray();
            foreach (var r in enumerable)
                switch (r.Type)
                {
                    case ResultType.Bad:
                        {
                            BadCount++;
                            break;
                        }
                    case ResultType.Suspect:
                        SuspectCount++;
                        break;
                    case ResultType.Good:
                        GoodCount++;
                        break;
                }
            _list.AddRange(collection: enumerable);
            OnResultLogReset();
            StatusLine = FindStatus();
        }

        public IEnumerator<Result> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Convert the log to a string.
        /// </summary>
        /// <returns>Returns the results in the log converted into text.  Each result is on  separate line.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var r in _list.ToArray())
                sb.AppendLine(r.ToString());
            return sb.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _list).GetEnumerator();
        }

        public string GetErrorMessage()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Result r in this.ToArray())
            {
                if (r.Type == ResultType.Bad) sb.AppendLine(r.Description);
            }

            return sb.ToString();
        }

        public Result FindStatus()
        {
            if (IsBad) return _list.FindLast((r) => r.Type == ResultType.Bad);
            if (IsSuspect) return _list.FindLast((r) => r.Type == ResultType.Suspect);
            if (IsGood) return _list.FindLast((r) => r.Type == ResultType.Good);
            return null;
        }

        public int IndexOf(Result item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, Result item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public Result this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public event EventHandler<ResultLogEventArgs> StatusChanged;
        public event EventHandler<ResultLogEventArgs> ResultAdded;
        public event EventHandler<EventArgs> ResultLogReset;

        public void RegisterEvents(IResultLogEvents e)
        {
            StatusChanged += e.StatusChanged;
            ResultAdded += e.ResultAdded;
            ResultLogReset += e.ResultLogReset;
        }

        public void UnregisterEvents(IResultLogEvents e)
        {
            StatusChanged -= e.StatusChanged;
            ResultAdded -= e.ResultAdded;
            ResultLogReset -= e.ResultLogReset;
        }

        protected virtual void OnStatusChanged()
        {
            EventHandler<ResultLogEventArgs> handler = StatusChanged;
            handler?.Invoke(this, new ResultLogEventArgs(_statusLine));
        }

        protected virtual void OnResultAdded(Result result)
        {
            EventHandler<ResultLogEventArgs> handler = ResultAdded;
            handler?.Invoke(this, new ResultLogEventArgs(result));
        }

        protected virtual void OnResultLogReset()
        {
            EventHandler<EventArgs> handler = ResultLogReset;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}