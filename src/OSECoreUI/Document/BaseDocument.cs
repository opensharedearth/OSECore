using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using OSECore.Logging;
using OSECoreUI.App;

namespace OSECoreUI.Document
{
    [Serializable]
    public class BaseDocument : IDirty, ISerializable, IDeserializationCallback, IDisposable
    {
        protected int RequiredVersion;
        public DocType DocType { get; protected set; }
        protected int ActualVersion;
        protected string ActualDocTypeName;
        public bool IsDisposed = false;
        private string _title;

        public string Title
        {
            get => String.IsNullOrEmpty(_title) ? String.IsNullOrEmpty(FileName) ? DocType.DefaultTitle : FileName : _title;
            set => _title = value;
        }
        public string Path { get; set; }
        public string FileExtention => String.IsNullOrEmpty(Path) ? DocType.Extension : System.IO.Path.GetExtension(Path);
        public string FileName => String.IsNullOrEmpty(Path) ? "" : System.IO.Path.GetFileNameWithoutExtension(Path);
        public BaseDocument()
        {
            Log = new ResultLog();
        }

        public BaseDocument(SerializationInfo info, StreamingContext context)
        {
            if (context.Context is BaseAppContext ac)
            {
                Log = ac.Log;
            }
            else
            {
                Log = new ResultLog();
            }
            try
            {
                ActualDocTypeName = info.GetString("DocType");
                ActualVersion = info.GetInt32("Version");
                Title = info.GetString("Title");
            }
            catch (Exception ex)
            {
                Log.LogBad("Unable to load document: " + ex.Message);
            }
        }

        private bool _isDirty = false;
        public virtual void Dirty()
        {
            _isDirty = true;
        }

        public virtual void Undirty()
        {
            _isDirty = false;
        }

        public virtual bool IsDirty => _isDirty;
        public bool IsUnsaved => String.IsNullOrEmpty(Path);
        public bool IsEmpty => IsUnsaved && !IsDirty;
        
        public ResultLog Log { get; private set; }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("DocType", DocType.Name);
            info.AddValue("Version", RequiredVersion);
            info.AddValue("Title", Title);
        }

        protected virtual bool Validate()
        {
            return DocType.Name == ActualDocTypeName && RequiredVersion == ActualVersion;
        }

        public virtual void OnDeserialization(object sender)
        {
            if (Validate())
            {
                Fixup();
            }
        }

        protected virtual void Fixup()
        {

        }

        protected virtual void Setup()
        {
            RequiredVersion = 1;
            DocType = DocTypeRegistry.Instance["OSE"];
        }

        public static BaseDocument Open(string path, ResultLog log = null)
        {
            ResultLog openLog = log ?? new ResultLog("Loading '" + (path ?? "(null)") + "'");
            BaseDocument bd = null;
            if (String.IsNullOrEmpty(path))
            {
                openLog.LogBad("Path is null or empty.");
            }
            else if (!File.Exists(path))
            {
                openLog.LogBad("File '" + path + "' does not exist.");
            }
            else
            {
                BaseAppContext ac = new BaseAppContext(openLog);
                StreamingContext sc = new StreamingContext(StreamingContextStates.File, ac);
                BinaryFormatter bf = new BinaryFormatter(null, sc);
                try
                {
                    using (FileStream fs = File.OpenRead(path))
                    {
                        bd = bf.Deserialize(fs) as BaseDocument;
                        if (bd != null)
                        {
                            bd.Path = path;
                            bd.Title = bd.FileName;
                        }
                    }

                }
                catch (Exception e)
                {
                    openLog.LogBad("Unable to load '" + path + "': " + e.Message);
                }
            }

            return bd;
        }

        public bool Save()
        {
            return Save(Path, true);
        }
        public bool Save(string path, bool overwrite = false)
        {
            Log.Clear();
            if (String.IsNullOrEmpty(path))
            {
                Log.LogBad("Path is null or empty.");
            }
            else if (File.Exists(path) && !overwrite)
            {
                Log.LogBad("File '" + path + "' already exists.");
            }
            else
            {
                BaseAppContext ac = new BaseAppContext(Log);
                StreamingContext sc = new StreamingContext(StreamingContextStates.File, ac);
                BinaryFormatter bf = new BinaryFormatter(null, sc);
                try
                {
                    using (FileStream fs = File.Create(path))
                    {
                        bf.Serialize(fs, this);
                        Path = path;
                        Undirty();
                    }
                }
                catch (Exception e)
                {
                    Log.LogBad("Unable to save '" + path + "': " + e.Message);
                }
            }

            return !Log.HasError;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static string GetFilterString(bool addAllFiles, params DocType[] docTypes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DocType docType in docTypes)
            {
                if (sb.Length > 0) sb.Append('|');
                sb.Append(docType.FilterString);
            }
            if (addAllFiles)
            {
                if (sb.Length > 0) sb.Append('|');
                sb.Append("(*.*)|All Files (*.*)");
            }

            return sb.ToString();
        }
    }
}
