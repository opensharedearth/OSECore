using OSECore.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTool
{
    public class PathFolder : IEquatable<PathFolder>, IComparable<PathFolder>
    {
        int _position = -1;
        string _path = "";
        public int Position => _position;
        public string Path => _path;
        FolderStatus _status = FolderStatus.Valid;
        FolderStatus Status => _status;
        public bool IsValid => _status == 0;
        public bool IsNullPath => (_status & FolderStatus.NullPath) != 0;
        public bool HasInvalidPath => (_status & FolderStatus.InvalidPath) != 0;
        public bool NotFullyQualified => (_status & FolderStatus.NotFullyQualified) != 0;
        public bool NonExtant => (_status & FolderStatus.Nonextant) != 0;
        public bool IsUnreadable => (_status & FolderStatus.Unreadable) != 0;
        public bool IsDuplicate => (_status & FolderStatus.Duplicate) != 0;
        public void SetDuplicate()
        {
            _status |= FolderStatus.Duplicate;
        }
        public PathFolder(string path)
        {
            _path = FileSupport.NormalizePath(path);
            _status = Validate(_path);
        }

        public static FolderStatus Validate(string path)
        {
            FolderStatus status = FolderStatus.Valid;
            if(String.IsNullOrEmpty(path))
            {
                status |= FolderStatus.NullPath;
            }
            else if(!FileSupport.IsValidPath(path))
            {
                status |= FolderStatus.InvalidPath;
            }
            else if(!System.IO.Path.IsPathFullyQualified(path))
            {
                status |= FolderStatus.NotFullyQualified;
            }
            else if(!Directory.Exists(path))
            {
                status |= FolderStatus.Nonextant;
            }
            else if(!FileSupport.IsFolderReadable(path))
            {
                status |= FolderStatus.Unreadable;
            }
            return status;
        }
        public void SetPosition(int position)
        {
            _position = position;
        }

        public bool Equals(PathFolder other)
        {
            if (other != null)
                return _path == other.Path;
            else
                return false;
        }

        public int CompareTo(PathFolder other)
        {
            if (other != null)
                return _path.CompareTo(other.Path);
            else
                return 1;
        }
        public override bool Equals(object obj)
        {
            if (obj is PathFolder path)
                return Equals(path);
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return _path.GetHashCode();
        }
        public static bool operator==(PathFolder a, PathFolder b)
        {
            if ((object)a == null)
                return (object)b == null;
            else if ((object)b == null)
                return false;
            return a.Equals(b);
        }
        public static bool operator!=(PathFolder a, PathFolder b)
        {
            if ((object)a == null)
                return (object)b != null;
            else if ((object)b == null)
                return true;
            return !a.Equals(b);
        }
        public static bool operator<(PathFolder a, PathFolder b)
        {
            if ((object)a == null)
                return (object)b != null;
            else if ((object)b == null)
                return false;
            return a.CompareTo(b) < 0;
        }
        public static bool operator <=(PathFolder a, PathFolder b)
        {
            if ((object)a == null)
                return true;
            else if ((object)b == null)
                return false;
            return a.CompareTo(b) <= 0;
        }
        public static bool operator >(PathFolder a, PathFolder b)
        {
            if ((object)a == null)
                return (object)b == null;
            else if ((object)b == null)
                return true;
            return a.CompareTo(b) > 0;
        }
        public static bool operator >=(PathFolder a, PathFolder b)
        {
            if ((object)a == null)
                return (object)b == null;
            else if ((object)b == null)
                return true;
            return a.CompareTo(b) >= 0;
        }
        public override string ToString()
        {
            return _path;
        }
        public string ToVerboseString(string format = "{0,8:d} {1,-16:s} {2:s}")
        {
            return String.Format(format, Position, GetStatusDescription(Status), Path);
        }
        public static string GetStatusDescription(FolderStatus status)
        {
            StringBuilder sb = new StringBuilder();
            if (status == FolderStatus.Valid)
                return "Valid";
            else if ((status & FolderStatus.NullPath) != 0)
                sb.Append("Null path");
            else if ((status & FolderStatus.InvalidPath) != 0)
                sb.Append("Invalid path");
            else if ((status & FolderStatus.NotFullyQualified) != 0)
                sb.Append("Not fully qualified");
            else if ((status & FolderStatus.Nonextant) != 0)
                sb.Append("Does not exist");
            else if ((status & FolderStatus.Unreadable) != 0)
                sb.Append("Cannot be read");
            else if ((status & FolderStatus.Duplicate) != 0)
                sb.Append("Duplicate");
            return sb.ToString();
        }
    }
}
