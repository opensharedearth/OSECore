using OSECoreUI.App;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OSEConfig
{
    public class ConfigSection : IDictionary<string, ConfigVariable>, IDirty, IDirtyEventSource, IDirtyEvents
    {
        static public ConfigSection Null = new ConfigSection();
        public ConfigSection()
        {

        }
        public ConfigSection(string name)
        {
            ValidateName(name);
            Name = name;
        }
        private void ValidateName(string name)
        {
            if (!Regex.IsMatch(name, "^[a-zA-Z_][a-zA-Z0-9_-]*$"))
            {
                throw new ArgumentException("Invalid name for configuration variable");
            }
        }
        string Name { get; } = "";
        bool _isDirty = false;
        Dictionary<string, ConfigVariable> _map = new Dictionary<string, ConfigVariable>();
        public ConfigVariable this[string key] 
        { 
            get
            {
                if(_map.TryGetValue(key, out ConfigVariable v))
                {
                    return v;
                }
                return ConfigVariable.Null;
            }
            set
            {
                if(!_map.TryGetValue(key, out ConfigVariable v) || v != value)
                {
                    _map[key] = value;
                    Dirty();
                }
            }
        }

        public ICollection<string> Keys => _map.Keys;

        public ICollection<ConfigVariable> Values => _map.Values;

        public int Count => _map.Count;

        public bool IsReadOnly => false;

        public bool IsDirty => _isDirty;
        public bool IsNull => String.IsNullOrEmpty(Name);

        public event EventHandler<EventArgs> Dirtied;
        public event EventHandler<EventArgs> Undirtied;

        public void Add(string key, ConfigVariable value)
        {
            _map.Add(key, value);
        }

        public void Add(KeyValuePair<string, ConfigVariable> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            _map.Clear();
        }

        public bool Contains(KeyValuePair<string, ConfigVariable> item)
        {
            return _map.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return _map.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, ConfigVariable>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void DirtiedHandler(object sender, EventArgs args)
        {
            Dirty();
        }

        public void Dirty()
        {
            if(!_isDirty)
            {
                OnDirtied();
                _isDirty = true;
            }
        }

        public IEnumerator<KeyValuePair<string, ConfigVariable>> GetEnumerator()
        {
            return _map.GetEnumerator();
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
            return _map.Remove(key);
        }

        public bool Remove(KeyValuePair<string, ConfigVariable> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out ConfigVariable value)
        {
            return _map.TryGetValue(key, out value);
        }

        public void UndirtiedHandler(object sender, EventArgs args)
        {
        }

        public void Undirty()
        {
            if(!_isDirty)
            {
                _isDirty = false;
                OnUndirtied();
                foreach(var v in _map.Values)
                {
                    v.Undirty();
                }
            }
        }

        public void UnregisterEvents(IDirtyEvents events)
        {
            Dirtied -= events.DirtiedHandler;
            Undirtied -= events.UndirtiedHandler;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _map.GetEnumerator();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[" + Name + "]");
            foreach(var v in _map.Values)
            {
                sb.AppendLine(v.ToString());
            }
            sb.AppendLine();
            return sb.ToString();
        }
        public T GetValue<T>(string name, T defaultValue = default(T))
        {
            return this[name].GetValue<T>(defaultValue);
        }
        public string GetSecretValue(string name)
        {
            return this[name].GetSecretValue();
        }
        public void SetValue(string name, object value)
        {
            var cv = this[name];
            if(cv.IsNull)
            {
                this[name] = new ConfigVariable(name, value);
            }
            else
            {
                cv.SetValue(value);
            }
        }
        public void SetSecretValue(string name, string value)
        {
            var cv = this[name];
            if (cv.IsNull)
            {
                this[name] = cv = new ConfigVariable(name, null);
            }
            cv.SetSecretValue(value);
        }
    }
}
