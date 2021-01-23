using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using OSECore.IO;
using OSECore.Logging;
using OSECore.Text;

namespace OSECoreUI.App
{
    [Serializable]
    public class BaseAppSettings : IDirty, ISerializable, IDisposable
    {
        private const string DocumentFolderKey = "DocumentFolder";
        private const string TempFolderKey = "TempFolder";
        public BaseAppSettings()
        {
            Initialize();
        }

        private void Initialize()
        {
            DocumentFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            TempFolder = Path.GetTempPath();
        }

        public BaseAppSettings(SerializationInfo info, StreamingContext context)
        {
            try
            {
                _documentFolder = info.GetString(DocumentFolderKey);
                _tempFolder = info.GetString(TempFolderKey);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Unable to deserialize base settings: " + e.Message);
                Initialize();
            }

        }
        private bool _isDirty = false;
        public virtual bool IsDirty => _isDirty;

        public string DocumentFolder
        {
            get => _documentFolder.SafeString();
            set
            {
                if (_documentFolder != value)
                {
                    _documentFolder = value;
                    Dirty();
                }
            }
        }

        public string TempFolder
        {
            get => _tempFolder.SafeString();
            set
            {
                if (_tempFolder != value)
                {
                    _tempFolder = value;
                    Dirty();
                }
            }
        }

        public bool IsDisposed = false;

        public virtual void Dirty()
        {
            _isDirty = true;
        }

        public virtual void Undirty()
        {
            _isDirty = false;
        }

        private string _documentFolder;
        private string _tempFolder;
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(DocumentFolderKey, DocumentFolder);
            info.AddValue(TempFolderKey, TempFolder);
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

        public virtual bool Validate(ResultLog log = null, bool fix = false)
        {
            bool result = true;
            if (!FileSupport.IsFolderWritable(DocumentFolder))
            {
                result = false;
                log?.LogBad("'" + DocumentFolder + "' is not writable.");
                if (fix)
                {
                    DocumentFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    log?.LogSuspect("Document folder reset to '" + DocumentFolder + "'.");
                }
            }

            if (!FileSupport.IsFolderWritable(TempFolder))
            {
                result = false;
                log?.LogBad("'" + TempFolder + "' is not writable.");
                if (fix)
                {
                    TempFolder = Path.GetTempPath();
                    log?.LogSuspect("Temp folder reset to '" + TempFolder + "'.");
                }
            }

            return result;
        }
    }
}
