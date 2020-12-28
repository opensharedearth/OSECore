using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace OSECoreTest.IO
{
    public class TestFileFixture : FileFixture
    {
        public string UnwritableFilePath;
        public string UnreadableFilePath;
        public string WritableFilePath;
        public string HiddenFilePath;
        public string ReadOnlyFilePath;
        public const string UnwritableFileName = "Unwritable.txt";
        public const string UnreadableFileName = "Unreadable.txt";
        public const string WritableFileName = "Writable.txt";
        public const string HiddenFileName = "HiddenFile.txt";
        public const string ReadOnlyFileName = "ReadOnly.txt";
        public TestFileFixture()
        {
            UnwritableFilePath = CreateUnwritableFile(TestDir);
            UnreadableFilePath = CreateUnreadableFile(TestDir);
            WritableFilePath = CreateWritableFile(TestDir);
            HiddenFilePath = CreateHiddenFile(TestDir);
            ReadOnlyFilePath = CreateReadOnlyFile(TestDir);
            SetDirectoryReadOnly(TestDir);

    }
        private void SetDirectoryReadOnly(string path)
        {
            DirectorySecurity ds = new DirectorySecurity(path, AccessControlSections.Access);
            SecurityIdentifier user = WindowsIdentity.GetCurrent().User;
            FileSystemAccessRule r = new FileSystemAccessRule(user, FileSystemRights.Write, AccessControlType.Deny);
            ds.AddAccessRule(r);
            DirectoryInfo di = new DirectoryInfo(path);
            di.SetAccessControl(ds);

        }

        private string CreateReadOnlyFile(string testDir)
        {
            string path = Path.Combine(testDir, ReadOnlyFileName);
            CreateFile(path);
            FileAttributes fa = File.GetAttributes(path);
            File.SetAttributes(path, fa | FileAttributes.ReadOnly);
            return path;
        }

        private void ClearReadOnlyAttribute(string path)
        {
            FileAttributes fa = File.GetAttributes(path);
            File.SetAttributes(path, fa & (~FileAttributes.ReadOnly));
        }

        private string CreateHiddenFile(string testDir)
        {
            string path = Path.Combine(testDir, HiddenFileName);
            CreateFile(path);
            FileAttributes fa = File.GetAttributes(path);
            File.SetAttributes(path, fa | FileAttributes.Hidden);
            return path;
        }

        private string CreateWritableFile(string testDir)
        {
            string path = Path.Combine(testDir, WritableFileName);
            CreateFile(path);
            return path;
        }

        private string CreateUnreadableFile(string testDir)
        {
            string path = Path.Combine(testDir, UnreadableFileName);
            CreateFile(path);
            FileSecurity fs = new FileSecurity(path, AccessControlSections.Access);
            SecurityIdentifier user = WindowsIdentity.GetCurrent().User;
            FileSystemAccessRule r = new FileSystemAccessRule(user, FileSystemRights.Read, AccessControlType.Deny);
            fs.AddAccessRule(r);
            FileInfo fi = new FileInfo(path);
            fi.SetAccessControl(fs);
            return path;
        }

        private string CreateUnwritableFile(string testDir)
        {
            string path = Path.Combine(testDir, UnwritableFileName);
            CreateFile(path);
            FileSecurity fs = new FileSecurity(path, AccessControlSections.Access);
            SecurityIdentifier user = WindowsIdentity.GetCurrent().User;
            FileSystemAccessRule r = new FileSystemAccessRule(user, FileSystemRights.Write, AccessControlType.Deny);
            fs.AddAccessRule(r);
            FileInfo fi = new FileInfo(path);
            fi.SetAccessControl(fs);
            return path;
        }


        public override void Dispose()
        {
            File.Delete(UnwritableFilePath);
            File.Delete(UnreadableFilePath);
            File.Delete(WritableFilePath);
            File.Delete(HiddenFilePath);
            ClearReadOnlyAttribute(ReadOnlyFilePath);
            File.Delete(ReadOnlyFilePath);
            base.Dispose();
        }

    }
}
