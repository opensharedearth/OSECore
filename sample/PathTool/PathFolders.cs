using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PathTool
{
    public class PathFolders : IList<PathFolder>
    {
        PathFolderOptions _options = PathFolderOptions.Process;
        public PathFolders(IEnumerable<PathFolder> folders)
        {
            _list.AddRange(folders);
        }
        public PathFolders(PathFolderOptions options = PathFolderOptions.Process)
        {
            _options = options;
        }
        private static char pathDivider = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ';' : ':';

        public void Fill()
        {
            Clear();
            string paths = Environment.GetEnvironmentVariable("PATH", GetEnvironmentVariableTarget(_options));
            if (!String.IsNullOrEmpty(paths))
            {
                string[] elements = paths.Split(pathDivider);
                int pos = 1;
                HashSet<string> pathHash = new HashSet<string>();
                foreach (var element in elements)
                {
                    var pf = CreatePathFolder(element);
                    pf.SetPosition(pos++);
                    if (pathHash.Contains(element)) pf.SetDuplicate();
                    pathHash.Add(element);
                    Add(pf);
                }
            }
        }
        public void Commit()
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (var f in this)
            {
                if (!first) sb.Append(';');
                first = false;
                sb.Append(f.Path);
            }
            Environment.SetEnvironmentVariable("PATH", sb.ToString(), GetEnvironmentVariableTarget(_options));
        }

        private PathFolder CreatePathFolder(string element)
        {
            return new PathFolder(element);
        }

        private EnvironmentVariableTarget GetEnvironmentVariableTarget(PathFolderOptions options)
        {
            switch (options)
            {
                case PathFolderOptions.Process:
                    return EnvironmentVariableTarget.Process;
                case PathFolderOptions.User:
                    return EnvironmentVariableTarget.User;
                case PathFolderOptions.Machine:
                    return EnvironmentVariableTarget.Machine;
                default:
                    return EnvironmentVariableTarget.Process;
            }
        }

        private List<PathFolder> _list = new List<PathFolder>();
        public PathFolder this[int index] { get => _list[index]; set => _list[index] = value; }

        public int Count => _list.Count();

        public bool IsReadOnly => false;

        public void Add(PathFolder item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(PathFolder item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(PathFolder[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<PathFolder> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(PathFolder item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, PathFolder item)
        {
            _list.Insert(index, item);
        }

        public bool Remove(PathFolder item)
        {
            return _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var f in this)
            {
                sb.AppendLine(f.ToString());
            }
            return sb.ToString();
        }
        public string ToVerboseString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("Position Status           Folder");
            sb.AppendLine("-------- ---------------- -----------------------");
            foreach (var f in this)
            {
                sb.AppendLine(f.ToVerboseString());
            }
            sb.AppendLine();
            sb.AppendLine($"{Count} entries.");
            return sb.ToString();
        }
        public string ToInlineString()
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (var f in this)
            {
                if (!first) sb.Append(pathDivider);
                first = false;
                sb.Append(f.ToString());
            }
            return sb.ToString();
        }
    }
}
