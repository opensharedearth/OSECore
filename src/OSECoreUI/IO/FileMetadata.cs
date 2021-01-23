using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Xml.Linq;
using OSECoreUI.App;

namespace OSECoreUI.IO
{
    [Serializable]
    public sealed class FileMetadata : ISerializable, IEquatable<FileMetadata>, IDirty, ICloneable
    {
        public static readonly FileMetadata Empty = new FileMetadata();
        public FileMetadata()
        {
        }

        public FileMetadata(SerializationInfo info, StreamingContext context)
        {
            try
            {
                _title = info.GetString("Title");
                _subject = info.GetString("Subject");
                _author = info.GetValue("Author", typeof(string[])) as string[];
                _copyright = info.GetString("Copyright");
                _keywords = info.GetValue("Keywords", typeof(string[])) as string[];
                _comment = info.GetString("Comment");
                _isDefined = info.GetBoolean("IsDefined");
            }
            catch (Exception e)
            {
                Trace.WriteLine("Unable to deserialize file metadata: " + e.Message);
            }

        }

        public FileMetadata(FileMetadata d)
        {
            _title = d.Title;
            _subject = d.Subject;
            _author = d.Author.Clone() as string[];
            _copyright = d.Copyright;
            _keywords = d.Keywords.Clone() as string[];
            _comment = d.Comment;
            _isDefined = d.IsDefined;
            Dirty();
        }

        private bool _isDefined = false;
        private string _title = "";
        private string _subject = "";
        private string[] _author = new string[0];
        private string _copyright = "";
        private string[] _keywords = new string[0];
        private string _comment = "";

        public bool IsEmpty => this == Empty;
        public bool IsDefined
        {
            get => _isDefined;
            set
            {
                if (_isDefined != value)
                {
                    _isDefined = value;
                    Dirty();
                }
            }
        }
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value && value != null)
                {
                    _title = value;
                    Dirty();
                }
            }
        }

        public string Subject
        {
            get => _subject;
            set
            {
                if (_subject != value && value != null)
                {
                    _subject = value;
                    Dirty();
                }
            }
        }

        public string[] Author
        {
            get => _author;
            set
            {
                if (_author != value && value != null)
                {
                    _author = value;
                    Dirty();
                }
            }
        }

        public string Copyright
        {
            get => _copyright;
            set
            {
                if (_copyright != value && value != null)
                {
                    _copyright = value;
                    Dirty();
                }
            }
        }

        public string[] Keywords
        {
            get => _keywords;
            set
            {
                if (_keywords != value && value != null)
                {
                    _keywords = value;
                    Dirty();
                }
            }
        }

        public string Comment
        {
            get => _comment;
            set
            {
                if (_comment != value && value != null)
                {
                    _comment = value;
                    Dirty();
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Title", Title);
            info.AddValue("Subject", Subject);
            info.AddValue("Author", Author);
            info.AddValue("Copyright", Copyright);
            info.AddValue("Keywords", Keywords);
            info.AddValue("Comment", Comment);
            info.AddValue("IsDefined", _isDefined);
        }

        public bool Equals(FileMetadata other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Title, other.Title) 
                   && string.Equals(Subject, other.Subject) 
                   && Author.SequenceEqual(other.Author) 
                   && string.Equals(Copyright, other.Copyright) 
                   && Keywords.SequenceEqual(other.Keywords) 
                   && string.Equals(Comment, other.Comment)
                   && IsDefined == other.IsDefined;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FileMetadata) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Title != null ? Title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Subject != null ? Subject.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Author != null ? Author.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Copyright != null ? Copyright.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Keywords != null ? Keywords.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Comment != null ? Comment.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsDefined.GetHashCode();
                return hashCode;
            }
        }

        public object Clone()
        {
            return new FileMetadata(this);
        }

        public static bool operator ==(FileMetadata a, FileMetadata b)
        {
            return a?.Equals(b) ?? ReferenceEquals(b, null);
        }

        public static bool operator !=(FileMetadata a, FileMetadata b)
        {
            return !(a == b);
        }

        private bool _isDirty = false;

        public void Dirty()
        {
            _isDirty = true;
        }

        public void Undirty()
        {
            _isDirty = false;
        }

        public bool IsDirty => _isDirty;
    }
}