using System;
using System.Diagnostics;
using System.Xml.Schema;
using OSECoreUI.Annotations;

namespace OSECoreUI.Document
{
    public struct DocType : IComparable<DocType>
    {
        public static DocType Null = new DocType();
        public static bool IsNull(DocType dt) => dt.Name == null;

        public string Name { get; }
        public Type Type { get; }
        public string Extension { get; }
        public string Description { get; }
        public string DefaultTitle { get; }

        public string FilterString =>  Description + " (*" + Extension + ")|" + "*" + Extension;


        public DocType(string name, Type type, string extension, string description = "", string defaultTitle = "")
        {
            Debug.Assert(!String.IsNullOrEmpty(name));
            Name = name;
            Debug.Assert(type != null);
            Type = type;
            Debug.Assert(!String.IsNullOrEmpty(extension));
            Extension = extension[0] != '.' ? "." + extension : extension;
            Description = String.IsNullOrEmpty(description) ? name + " Document" : description;
            DefaultTitle = String.IsNullOrEmpty(defaultTitle) ? "(Untitled)" : defaultTitle;
        }

        public int CompareTo(DocType other)
        {
            return String.CompareOrdinal(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if(obj is DocType a)
            {
                return a.Name == Name;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(DocType a, DocType b)
        {
            return a.Name == b.Name;
        }

        public static bool operator !=(DocType a, DocType b)
        {
            return a.Name != b.Name;
        }
    }
}