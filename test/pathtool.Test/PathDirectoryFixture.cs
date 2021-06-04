using OSEConfig;
using OSETesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace pathtool.Test
{
    public class PathDirectoryFixture : TestFileFixture
    {
        static public string[] ValidFolders = { "z00", "w01", "c2", "d2", "e4" };
        static public string[] UnreadableFolders = { "f", "g"};
        static public string[] InvalidFolders = { "aa\"", "bb<|>" };
        static public string[] NotFullyQualifiedFolders = { "b", "i" };
        static public string[] NonExtantFolders = { "n11", "o33" };
        static public string[] AddFolders = { "j", "k" };


        [SupportedOSPlatform("windows")]
        public PathDirectoryFixture()
            : base("pathtool.Test")
        {
            CreateValidFolders(TestDir, ValidFolders);
            CreateUnreadableFolders(TestDir, UnreadableFolders);
            CreateAddFolders(TestDir, AddFolders);
            GeneralParameters.Instance.WorkingFolder = TestDir;
        }

        private void CreateValidFolders(string parent, string[] folders)
        {
            foreach (var folder in folders)
                Directory.CreateDirectory(Path.Combine(parent, folder));
        }
        private void DeleteValidFolders(string parent, string[] folders)
        {
            foreach (var folder in folders)
                Directory.Delete(Path.Combine(parent, folder));
        }
        private void CreateAddFolders(string parent, string[] folders)
        {
            foreach (var folder in folders)
                Directory.CreateDirectory(Path.Combine(parent, folder));
        }
        private void DeleteAddFolders(string parent, string[] folders)
        {
            foreach (var folder in folders)
                Directory.Delete(Path.Combine(parent, folder));
        }

        private void CreateUnreadableFolders(string parent, string[] folders)
        {
            foreach (var folder in folders)
            {
                string path = Path.Combine(parent, folder);
                Directory.CreateDirectory(path);
                if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    FileSecurity fs = new FileSecurity(path, AccessControlSections.Access);
                    SecurityIdentifier user = WindowsIdentity.GetCurrent().User;
                    FileSystemAccessRule r = new FileSystemAccessRule(user, FileSystemRights.Read, AccessControlType.Deny);
                    fs.AddAccessRule(r);
                    FileInfo fi = new FileInfo(path);
                    fi.SetAccessControl(fs);
                }
            }
        }
        private static char pathDivider = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ';' : ':';

        internal void SetTestPath(bool addInvalid = true)
        {
            List<string> paths = new List<string>();
            paths.AddRange(GetValidPaths());
            if(addInvalid)
            {
                paths.AddRange(GetInvalidPaths());
                paths.Add("");
                paths.AddRange(GetNonextantPaths());
                paths.AddRange(GetNotFullyQualifiedPaths());
                paths.AddRange(GetUnreadablePaths());
            }
            string path = String.Join(pathDivider, paths.ToArray());
            Environment.SetEnvironmentVariable("PATH", path);
        }

        private string[] GetValidPaths()
        {
            List<string> paths = new List<string>();
            foreach (var folder in ValidFolders)
                paths.Add(Path.Combine(TestDir, folder));
            return paths.ToArray();
        }
        private string[] GetUnreadablePaths()
        {
            List<string> paths = new List<string>();
            foreach (var folder in UnreadableFolders)
                paths.Add(Path.Combine(TestDir, folder));
            return paths.ToArray();
        }
        private string[] GetInvalidPaths()
        {
            List<string> paths = new List<string>();
            foreach(var folder in InvalidFolders)
                paths.Add(Path.Combine(TestDir, folder));
            return paths.ToArray();
        }
        private string[] GetNonextantPaths()
        {
            List<string> paths = new List<string>();
            foreach (var folder in NonExtantFolders)
                paths.Add(Path.Combine(TestDir, folder));
            return paths.ToArray();
        }
        private string[] GetNotFullyQualifiedPaths()
        {
            return NotFullyQualifiedFolders;
        }

        private void DeleteUnreadableFolders(string parent, string[] folders)
        {
            foreach (var folder in folders)
                Directory.Delete(Path.Combine(parent, folder));
        }
        public override void Dispose()
        {
            DeleteValidFolders(TestDir, ValidFolders);
            DeleteUnreadableFolders(TestDir, UnreadableFolders);
            DeleteAddFolders(TestDir, AddFolders);
            base.Dispose();
        }
    }
}
