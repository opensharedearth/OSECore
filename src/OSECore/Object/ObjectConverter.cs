using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Schema;
using OSECore.Logging;
using OSECore.Text;

namespace OSECore.Object
{
    public class ObjectConverter
    {
        private Type _foundation;
        private Type[] _tlist;
        private Func<object, object[]> _extraction;
        private Func<object[], object> _construction;
        private Func<object, bool>[] _constraints;
        private string _format;
        private bool _isValid = false;
        private string _formatDescription;
        private string _constraintsDescription;

        public ObjectConverter(Type foundation, Type[] tlist, Func<object, object[]> extraction, Func<object[], object> construction,
            string format, string formatDescription = "", Func<object,bool>[] constraints = null, string constraintsDescription = "")
        {
            _foundation = foundation;
            _tlist = tlist;
            _extraction = extraction;
            _construction = construction;
            _format = format;
            _constraints = constraints;
            _formatDescription = formatDescription;
            _constraintsDescription = constraintsDescription;
            Validate();
        }

        public bool IsValid => _isValid;

        public Type Foundation
        {
            get { return _foundation; }
        }

        public Func<object, bool>[] Validation => _constraints;

        public string FormatDescription
        {
            get { return _formatDescription; }
        }

        public string ConstraintsDescription
        {
            get { return _constraintsDescription; }
        }

        private void Validate()
        {
            _isValid = _foundation != null && _extraction != null && _construction != null && _format != null;
        }

        public object[] Extract(object d)
        {
            if (IsValid && d != null && (d.GetType() == _foundation || d.GetType().IsSubclassOf(_foundation)))
            {
                return _extraction(d);
            }

            return new object[0];
        }

        public object Construct(object[] dl)
        {
            if (Validate(dl))
            {
                try
                {
                    return _construction(dl);
                }
                catch (Exception e)
                {
                    throw new FormatException("Exception while constructing: " + e.Message);
                }
            }
            else
            {
                string s = GetDescription();
                if (!String.IsNullOrEmpty(s))
                {
                    throw new FormatException("Invalid format: " + s);
                }
                else
                {
                    throw new FormatException("Invalid format.");
                }
            }
        }

        public string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrEmpty(FormatDescription))
            {
                sb.Append("Expecting " + FormatDescription);
                if (!String.IsNullOrEmpty(ConstraintsDescription))
                {
                    sb.Append(" where " + ConstraintsDescription);
                }

                sb.Append(".");
                return sb.ToString();
            }

            return "";
        }
        public string Format(object d)
        {
            object[] dl = Extract(d);
            return String.Format(_format, dl);
        }

        public bool Validate(object[] dl)
        {
            if (dl != null && dl.Length == _tlist.Length)
            {
                if (_constraints != null && _constraints.Length == _tlist.Length)
                {
                    for (int i = 0; i < dl.Length; ++i)
                    {
                        if (_constraints[i] != null && !_constraints[i](dl[i]))
                            return false;
                    }
                }
                return true;
            }

            return false;
        }
        public object Construct(string s)
        {
            if (!String.IsNullOrEmpty(s))
            {
                object[] dl = s.Parse(_format, _tlist);
                return Construct(dl);
            }

            return null;
        }
        public object[] Parse(string s)
        {
            if (!String.IsNullOrEmpty(s))
            {
                object[] dl = s.Parse(_format, _tlist);
                return dl;
            }

            return new object[0];
        }

    }
}
