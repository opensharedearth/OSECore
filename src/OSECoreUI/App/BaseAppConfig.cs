using OSECore.IO;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace OSECoreUI.App
{
    public class BaseAppConfig
    {
        bool _noIni;
        bool _defaultIni;
        string _alternateIniPath = "";
        private BaseAppSettings _settings;
        public BaseAppSettings Settings
        {
            get => _settings;
            set => _settings = value;
        }


        public BaseAppConfig(BaseAppSettings settings = null)
        {
            _settings = settings;
        }
        public virtual bool EvaluateArguments(string[] args)
        {
            for (int iarg = 0; iarg < args.Length; ++iarg)
            {
                switch (args[iarg].ToLower())
                {
                    case "/?":
                        Usage();
                        return false;
                    case "/noini":
                        _noIni = true;
                        break;
                    case "/defaultini":
                        _defaultIni = true;
                        break;
                    case "/ini":
                        if (iarg < args.Length)
                        {
                            _alternateIniPath = args[++iarg];
                            if (!FileSupport.IsFileWritable(_alternateIniPath))
                            {
                                throw new ApplicationException("Alternate initialization file '" + _alternateIniPath + "' is not writable.");
                            }
                        }
                        break;
                    default:
                        Usage();
                        return false;
                }
            }
            return true;
        }

        protected virtual void Usage()
        {
        }

        public string IniName => AppName + ".ini";

        public string IniPath => Path.Combine(AppDataFolderPath, IniName);

        public string AppFolderPath
        {
            get
            {
                Assembly a = Assembly.GetEntryAssembly();
                string path = a.Location;
                if (Path.IsPathRooted(path))
                {
                    return Path.GetDirectoryName(path);
                }
                else
                {
                    return "";
                }
            }
        }
        public string DefaultIniPath => Path.Combine(AppFolderPath, IniName);

        public string AppDataFolderPath => Path.Combine(CompanyFolderPath, AppName);

        public string CompanyFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenSharedEarth.org");

        public string AppName
        {
            get
            {
                Assembly a = Assembly.GetEntryAssembly();
                return a.GetName().Name;
            }
        }

        public string AppTitle
        {
            get
            {
                Assembly a = Assembly.GetEntryAssembly();
                AssemblyTitleAttribute ta = a.GetCustomAttribute<AssemblyTitleAttribute>();
                return ta != null ? ta.Title : AppName;
            }
        }

        public virtual void Load()
        {
            try
            {
                if (!_noIni)
                {
                    string path = IniPath;
                    if (!File.Exists(path) || _defaultIni)
                    {
                        path = DefaultIniPath;
                    }
                    else if (_alternateIniPath.Length > 0)
                    {
                        path = _alternateIniPath;
                    }
                    Load(path);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception reading ini file: " + ex.Message);
            }
        }

        protected virtual void Load(string path)
        {
            if (!String.IsNullOrEmpty(path) && File.Exists(path))
            {
                try
                {
                    using (FileStream s = File.OpenRead(path))
                    {

                        BinaryFormatter formatter = new BinaryFormatter();
                        Settings = formatter.Deserialize(s) as BaseAppSettings;
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Unable to load ini file: " + e.Message);
                }
            }
        }

        public virtual void Save()
        {
            string path = IniPath;
            if (_alternateIniPath.Length > 0)
            {
                path = _alternateIniPath;
            }
            Directory.CreateDirectory(AppDataFolderPath);
            Save(path);
        }

        protected virtual void Save(string path)
        {
            if (Settings != null && Settings.IsDirty)
            {
                using (FileStream s = File.Create(path))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(s, Settings);
                }
            }
        }
    }
}
