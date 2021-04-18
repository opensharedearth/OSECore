using OSECoreUI.App;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OSELogic.Config
{
    public class ConfigFile : IDictionary<string, ConfigSection>, IDirty, IDirtyEventSource, IDirtyEvents
    {
        public static ConfigFile _instance;
        public static ConfigFile Instance 
        { 
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        Dictionary<string, ConfigSection> _sections = new Dictionary<string, ConfigSection>();

        public ConfigSection this[string key]
        {
            get
            {
                if (!_sections.TryGetValue(key, out ConfigSection s))
                {
                    return ConfigSection.Null;
                }
                return s;
            }
            set
            {
                _sections[key] = value;
            }
        }

        public ICollection<string> Keys => _sections.Keys;

        public ICollection<ConfigSection> Values => _sections.Values;

        public int Count => _sections.Count;

        public bool IsReadOnly => false;

        private bool _isDirty = false;
        public bool IsDirty => _isDirty;

        public event EventHandler<EventArgs> Dirtied;
        public event EventHandler<EventArgs> Undirtied;

        public void Add(string key, ConfigSection value)
        {
            _sections.Add(key, value);
            Dirty();
        }

        public void Add(KeyValuePair<string, ConfigSection> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            _sections.Clear();
            Dirty();
        }

        public bool Contains(KeyValuePair<string, ConfigSection> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            return _sections.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, ConfigSection>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void Dirty()
        {
            if (!IsDirty)
            {
                _isDirty = true;
                OnDirtied();
            }
        }

        public IEnumerator<KeyValuePair<string, ConfigSection>> GetEnumerator()
        {
            return _sections.GetEnumerator();
        }

        public void OnDirtied()
        {
            Dirtied?.Invoke(this, EventArgs.Empty);
        }

        public void OnUndirtied()
        {
            Undirtied?.Invoke(this, EventArgs.Empty);
        }

        public void RegisterEvents(IDirtyEvents events)
        {
            Dirtied += events.DirtiedHandler;
            Undirtied += events.UndirtiedHandler;
        }

        public bool Remove(string key)
        {
            return _sections.Remove(key);
        }

        public bool Remove(KeyValuePair<string, ConfigSection> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out ConfigSection value)
        {
            return _sections.TryGetValue(key, out value);
        }

        public void Undirty()
        {
            if (_isDirty)
            {
                foreach (var s in _sections.Values)
                {
                    s.Undirty();
                }
                OnUndirtied();
            }
        }

        public void UnregisterEvents(IDirtyEvents events)
        {
            Dirtied -= events.DirtiedHandler;
            Undirtied -= events.UndirtiedHandler;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_sections).GetEnumerator();
        }
        private static string GetDefaultConfigFilePath()
        {
            Assembly a = Assembly.GetEntryAssembly();
            string appName = a.GetName().Name;
            string root = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(root, Path.Combine("." + appName, "Config"));
        }
        public void Save(string path = null)
        {
            if (path == null) path = GetDefaultConfigFilePath();
            string folder = Path.GetDirectoryName(path);
            if (!String.IsNullOrEmpty(folder))
            {
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            }
            using (TextWriter writer = new StreamWriter(path))
            {
                foreach(var section in _sections.Values)
                {
                    writer.Write(section.ToString());
                }
            }
        }
        public static ConfigFile Load(string path = null)
        {
            ConfigSection section = null;
            if (path == null) path = GetDefaultConfigFilePath();
            try
            {
                ConfigFile file = new ConfigFile();
                using (TextReader reader = new StreamReader(path))
                {
                    int lineNo = 0;
                    String line = reader.ReadLine();
                    while(line != null)
                    {
                        lineNo++;
                        line = line.Trim();
                        if(String.IsNullOrEmpty(line))
                        {
                            ;
                        }
                        else if(Regex.IsMatch(line, @"^\[[a-zA-Z_][a-zA-Z0-9_-]*\]$"))
                        {
                            string sectionName = line.Substring(1, line.Length - 2);
                            section = new ConfigSection(sectionName);
                            file.Add(sectionName, section);
                        }
                        else if(Regex.IsMatch(line, @"^[a-zA-Z_][a-zA-Z0-9_-]*=.*$"))
                        {
                            if (section == null) throw new ApplicationException("Variable definition outside of section");
                            int i = line.IndexOf('=');
                            string name = line.Substring(0, i);
                            string value = line.Substring(i + 1);
                            var v = new ConfigVariable(name, value);
                            section.Add(name, v);
                        }
                        line = reader.ReadLine();
                    }
                }
                return file;
            }
            catch(Exception ex)
            {
                Trace.WriteLine("Unable to read config file: " + ex.Message);
                return new ConfigFile();
            }
        }

        public void DirtiedHandler(object sender, EventArgs args)
        {
            Dirty();
        }

        public void UndirtiedHandler(object sender, EventArgs args)
        {
        }
        public ConfigSection FindOrCreate(string name)
        {
            if(TryGetValue(name, out ConfigSection v))
            {
                return v;
            }
            else
            {
                Add(name, v = new ConfigSection(name));
                return v;
            }
        }
    }
}
