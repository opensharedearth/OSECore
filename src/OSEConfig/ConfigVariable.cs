using OSECoreUI.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OSEConfig
{
    public class ConfigVariable : IEquatable<ConfigVariable>, IComparable<ConfigVariable>, IDirty, IDirtyEventSource
    {
        public static ConfigVariable Null = new ConfigVariable();
        public ConfigVariable()
        {

        }
        public ConfigVariable(string name, object value)
        {
            ValidateName(name);
            Name = name;
            SetValueInternal(value);
        }
        private void ValidateName(string name)
        {
            if(!Regex.IsMatch(name,"^[a-zA-Z_][a-zA-Z0-9_-]*$"))
            {
                throw new ArgumentException("Invalid name for configuration variable");
            }
        }
        private void ValidateValue(object value)
        {
            if(value != null && !Regex.IsMatch(value.ToString(),"^.*$"))
            {
                throw new ArgumentException("Invalid value for configuration valiable");
            }
        }
        string _value = null;
        bool _isDirty = false;

        public bool IsDirty => _isDirty;
        public bool IsNull => String.IsNullOrEmpty(Name);

        public string Name { get; } = "";
        public string Value 
        {
            set
            {
                if(_value != value)
                {
                    ValidateValue(Value);
                    _value = value;
                    Dirty();
                }
            }
            get => _value; 
        }

        public T GetValue<T>(T defaultValue = default(T))
        {
            if (_value == null)
            {
                return defaultValue;
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)(object)_value;
            }
            else if (typeof(T) == typeof(int) && int.TryParse(_value, out int iv))
            {
                return (T)(object)iv;
            }
            else if (typeof(T) == typeof(float) && float.TryParse(_value, out float fv))
            {
                return (T)(object)fv;
            }
            else if(typeof(T) == typeof(double) && double.TryParse(_value, out double dv))
            {
                return (T)(object)dv;
            }
            else if(typeof(T) == typeof(bool) && bool.TryParse(_value, out bool bv))
            {
                return (T)(object)bv;
            }
            else if(typeof(T) == typeof(DateTime) && DateTime.TryParse(_value, out DateTime dtv))
            {
                return (T)(object)dtv;
            }
            else if(typeof(T).IsEnum && Enum.TryParse(typeof(T), _value, true, out object ev))
            {
                return (T)ev;
            }
            else
            {
                return defaultValue;
            }
        }
        public void SetValue(object value)
        {
            if(!Object.Equals(_value, value))
            {
                SetValueInternal(value);
                Dirty();
            }
        }
        private void SetValueInternal(object value)
        {
            if (value == null)
            {
                _value = null;
            }
            else if(value is int || value is bool)
            {
                _value = value.ToString();
            }
            else if(value is float)
            {
                _value = ((float)value).ToString("R");
            }
            else if(value is double)
            {
                _value = ((double)value).ToString("R");
            }
            else if(value is DateTime)
            {
                _value = ((DateTime)value).ToString("O");
            }
            else if(value.GetType().IsEnum)
            {
                _value = value.ToString();
            }
            else
            {
                string s = value.ToString();
                ValidateValue(s);
                _value = s;
            }
        }

        public event EventHandler<EventArgs> Dirtied;
        public event EventHandler<EventArgs> Undirtied;

        public void Dirty()
        {
            if(!_isDirty)
            {
                _isDirty = true;
                OnDirtied();
            }
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

        public void Undirty()
        {
            if(_isDirty)
            {
                _isDirty = false;
                OnUndirtied();
            }
        }

        public void UnregisterEvents(IDirtyEvents events)
        {
            Dirtied -= events.DirtiedHandler;
            Undirtied -= events.UndirtiedHandler;
        }
        public override bool Equals(object obj)
        {
            if(obj is ConfigVariable v)
            {
                if (Name == v.Name && Object.Equals(Value, v.Value))
                {
                    return true;
                }
                return false;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ (Value != null ? Value.GetHashCode() : 0);
        }
        public static bool operator==(ConfigVariable v1, ConfigVariable v2)
        {
            if ((object)v1 == null && (object)v2 == null)
                return true;
            else if ((object)v1 == null || (object)v2 == null)
                return false;
            else
                return v1.Equals(v2);
        }
        public static bool operator!=(ConfigVariable v1, ConfigVariable v2)
        {
            if ((object)v1 == null && (object)v2 == null)
                return false;
            else if ((object)v1 == null || (object)v2 == null)
                return true;
            else
                return !v1.Equals(v2);
        }
        public override string ToString()
        {
            if(!IsNull)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Name);
                sb.Append('=');
                if (_value != null)
                {
                    sb.Append(_value.ToString());
                }
                return sb.ToString();
            }
            return String.Empty;
        }
        [SupportedOSPlatform("windows")]
        public void SetSecretValue(string s)
        {
            if(!String.IsNullOrEmpty(s))
            {
                var encoding = new UTF8Encoding();
                byte[] plain = encoding.GetBytes(s);
                byte[] secret = ProtectedData.Protect(plain,null, DataProtectionScope.CurrentUser);
                SetValue(Convert.ToBase64String(secret));
            }
            else
            {
                SetValue(s);
            }
        }
        [SupportedOSPlatform("windows")]
        public string GetSecretValue()
        {
            if(_value == null || _value.GetType() != typeof(string))
            {
                return null;
            }
            else
            {
                byte[] secret = Convert.FromBase64String(_value as string);
                byte[] plain = ProtectedData.Unprotect(secret,null,DataProtectionScope.CurrentUser);
                var encoding = new UTF8Encoding();
                return encoding.GetString(plain);
            }
        }

        public int CompareTo(ConfigVariable other)
        {
            if (other == null)
                return 1;
            else if(Name == other.Name)
            {
                if (Value == other.Value)
                    return 0;
                else if (Value == null)
                    return -1;
                else
                    return Value.CompareTo(other.Value);
            }
            else
            {
                return Name.CompareTo(other.Name);
            }
        }

        public bool Equals(ConfigVariable other)
        {
            return CompareTo(other) == 0;
        }
        public static bool operator<(ConfigVariable v1, ConfigVariable v2)
        {
            return v1.CompareTo(v2) < 0;
        }
        public static bool operator>(ConfigVariable v1, ConfigVariable v2)
        {
            return v1.CompareTo(v2) > 0;
        }
        public static bool operator<=(ConfigVariable v1, ConfigVariable v2)
        {
            return v1.CompareTo(v2) <= 0;
        }
        public static bool operator>=(ConfigVariable v1, ConfigVariable v2)
        {
            return v1.CompareTo(v2) >= 0;
        }
    }

}
