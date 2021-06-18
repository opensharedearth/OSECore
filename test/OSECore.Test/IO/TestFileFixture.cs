using Mono.Unix;
using OSETesting;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Xunit.Abstractions;
using Mono.Unix.Native;

#pragma warning disable CA1416

namespace OSECoreTest.IO
{
    public class TestFileFixture : FileFixture
    {
        public string UnwritableFilePath;
        public string UnreadableFilePath;
        public string WritableFilePath;
        public string HiddenFilePath;
        public string ReadOnlyFilePath;
        public string InvalidFilePath = " <|>";
        public string NonExistentFilePath;
        public string UnreadableFolderPath;
        public string UnwritableFolderPath;
        public string ReadableFolderPath;
        public const string UnwritableFileName = "Unwritable.txt";
        public const string UnreadableFileName = "Unreadable.txt";
        public const string WritableFileName = "Writable.txt";
        public const string HiddenFileName = "HiddenFile.txt";
        public const string ReadOnlyFileName = "ReadOnly.txt";
        public const string NonExistentFileName = "NonExistent.txt";
        public const string UnreadableFolderName = "UnreadableFolder";
        public const string UnwritableFolderName = "UnwritableFolder";
        public const string ReadableFolderName = "ReadableFolder";
        public TestFileFixture()
            : base("OSECoreTest.IO")
        {
            SetTestFilePaths();
            DeleteTestFiles();
            CreateTestFiles();
        }

        public void SetTestFilePaths()
        {
            UnwritableFilePath = Path.Combine(TestDir, UnwritableFileName);
            UnreadableFilePath = Path.Combine(TestDir, UnreadableFileName);
            WritableFilePath = Path.Combine(TestDir, WritableFileName);
            HiddenFilePath = Path.Combine(TestDir, HiddenFileName);
            ReadOnlyFilePath = Path.Combine(TestDir, ReadOnlyFileName);
            NonExistentFilePath = Path.Combine(TestDir, NonExistentFileName);
            UnreadableFolderPath = Path.Combine(TestDir, UnreadableFolderName);
            UnwritableFolderPath = Path.Combine(TestDir, UnwritableFolderName);
            ReadableFolderPath = Path.Combine(TestDir, ReadableFolderName);
        }

        public void CreateTestFiles()
        {
            CreateUnwritableFile();
            CreateUnreadableFile();
            CreateWritableFile();
            CreateHiddenFile();
            CreateReadOnlyFile();
            CreateUnreadableFolder();
            CreateUnwritableFolder();
            CreateReadableFolder();
        }

        private void SetDirectoryReadOnly(string path)
        {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                DirectorySecurity ds = new DirectorySecurity(path, AccessControlSections.Access);
                SecurityIdentifier user = WindowsIdentity.GetCurrent().User;
                FileSystemAccessRule r = new FileSystemAccessRule(user, FileSystemRights.Write, AccessControlType.Deny);
                ds.AddAccessRule(r);
                DirectoryInfo di = new DirectoryInfo(path);
                di.SetAccessControl(ds);
            }
            else
            {
                FileInfo fi = new FileInfo(path);
                fi.IsReadOnly = true;
            }

        }

        private void CreateReadOnlyFile()
        {
            string path = ReadOnlyFilePath;
            CreateFile(path);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                FileAttributes fa = File.GetAttributes(path);
                File.SetAttributes(path, fa | FileAttributes.ReadOnly);
            }
            else
            {
                FileInfo fi = new FileInfo(path);
                fi.IsReadOnly = true;
            }
        }
        private void ClearReadOnlyAttribute(string path)
        {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                FileAttributes fa = File.GetAttributes(path);
                File.SetAttributes(path, fa & (~FileAttributes.ReadOnly));
            }
            else
            {
                FileInfo fi = new FileInfo(path);
                fi.IsReadOnly = false;
            }
        }

        private void CreateHiddenFile()
        {
            string path = HiddenFilePath;
            CreateFile(path);
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                FileAttributes fa = File.GetAttributes(path);
                File.SetAttributes(path, fa | FileAttributes.Hidden);
            }
        }

        private void CreateWritableFile()
        {
            string path = WritableFilePath;
            CreateFile(path);
        }

        private void CreateReadableFolder()
        {
            string path = ReadableFolderPath;
            Directory.CreateDirectory(path);
        }

        private void CreateUnreadableFile()
        {
            string path = UnreadableFilePath;
            CreateFile(path);
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                FileSecurity fs = new FileSecurity(path, AccessControlSections.Access);
                SecurityIdentifier user = WindowsIdentity.GetCurrent().User;
                FileSystemAccessRule r = new FileSystemAccessRule(user, FileSystemRights.Read, AccessControlType.Deny);
                fs.AddAccessRule(r);
                FileInfo fi = new FileInfo(path);
                fi.SetAccessControl(fs);
            }
            else
            {
                var fi = new UnixFileInfo(path);
                fi.FileAccessPermissions = 0;
            }
        }

        private void CreateUnwritableFile()
        {
            string path = UnwritableFilePath;
            CreateFile(path);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {

                FileSecurity fs = new FileSecurity(path, AccessControlSections.Access);
                SecurityIdentifier user = WindowsIdentity.GetCurrent().User;
                FileSystemAccessRule r = new FileSystemAccessRule(user, FileSystemRights.Write, AccessControlType.Deny);
                fs.AddAccessRule(r);
                FileInfo fi = new FileInfo(path);
                fi.SetAccessControl(fs);
            }
            else
            {
                var fi = new UnixFileInfo(path);
                fi.FileAccessPermissions = FileAccessPermissions.OtherRead | FileAccessPermissions.GroupRead | FileAccessPermissions.UserRead;
            }
        }

        private void CreateUnwritableFolder()
        {
            string path = UnwritableFolderPath;
            Directory.CreateDirectory(path);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                FileSecurity fs = new FileSecurity(path, AccessControlSections.Access);
                SecurityIdentifier user = WindowsIdentity.GetCurrent().User;
                FileSystemAccessRule r = new FileSystemAccessRule(user, FileSystemRights.Write | FileSystemRights.Modify, AccessControlType.Deny);
                fs.AddAccessRule(r);
                FileInfo fi = new FileInfo(path);
                fi.SetAccessControl(fs);
            }
            else
            {
                var fi = new UnixFileInfo(path);
                fi.FileAccessPermissions = FileAccessPermissions.OtherRead | FileAccessPermissions.GroupRead | FileAccessPermissions.UserRead;
            }
        }

        private void CreateUnreadableFolder()
        {
            string path = UnreadableFolderPath;
            Directory.CreateDirectory(path);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                FileSecurity fs = new FileSecurity(path, AccessControlSections.Access);
                SecurityIdentifier user = WindowsIdentity.GetCurrent().User;
                FileSystemAccessRule r = new FileSystemAccessRule(user, FileSystemRights.Read, AccessControlType.Deny);
                fs.AddAccessRule(r);
                FileInfo fi = new FileInfo(path);
                fi.SetAccessControl(fs);
            }
            else
            {
                var fi = new UnixFileInfo(path);
                fi.FileAccessPermissions = 0;
            }
        }
        public void ClearDenyACEs(string path)
        {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                FileSecurity fs = new FileSecurity(path, AccessControlSections.Access);
                AuthorizationRuleCollection rules = fs.GetAccessRules(true, true, typeof(SecurityIdentifier));
                SecurityIdentifier user = WindowsIdentity.GetCurrent().User;
                AuthorizationRuleCollection newRules = new AuthorizationRuleCollection();
                FileSystemSecurity fssNew = new FileSecurity();
                foreach (FileSystemAccessRule rule in rules.OfType<FileSystemAccessRule>())
                {
                    if(rule.IdentityReference == user && rule.AccessControlType == AccessControlType.Deny)
                    {
                        fs.RemoveAccessRule(rule);
                        FileInfo fi = new FileInfo(path);
                        fi.SetAccessControl(fs);

                    }
                }
            }
        }

        public void DeleteTestFiles()
        {
            if(File.Exists(UnwritableFilePath))File.Delete(UnwritableFilePath);
            if(File.Exists(UnreadableFilePath))File.Delete(UnreadableFilePath);
            if(File.Exists(WritableFilePath))File.Delete(WritableFilePath);
            if (File.Exists(HiddenFilePath))File.Delete(HiddenFilePath);
            if (File.Exists(ReadOnlyFilePath))
            {
                ClearReadOnlyAttribute(ReadOnlyFilePath);
                File.Delete(ReadOnlyFilePath);
            }
            if (Directory.Exists(UnwritableFolderPath))
            {
                ClearDenyACEs(UnwritableFolderPath);
                Directory.Delete(UnwritableFolderPath);
            }
            if (Directory.Exists(ReadableFolderPath))Directory.Delete(ReadableFolderPath);
            if (Directory.Exists(UnreadableFolderPath))Directory.Delete(UnreadableFolderPath);
        }
        public bool NeedsCleanup()
        {
            var flag = Environment.GetEnvironmentVariable("NOCLEANUP");
            return flag != "1";
        }
        public override void Dispose()
        {
            if(NeedsCleanup())
            {
                DeleteTestFiles();
            }
            base.Dispose();
        }

    }
}
