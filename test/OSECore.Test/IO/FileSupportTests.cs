using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using OSECore.IO;
using OSETesting;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CA1416

namespace OSECoreTest.IO
{
    public class FileSupportTests : IClassFixture<TestFileFixture>, IDisposable
    {
        ITestOutputHelper _output;
        private readonly TestFileFixture _fixture;
        public FileSupportTests(TestFileFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        public void Dispose()
        {
        }

        [Fact]
        public void IsWritableTest()
        {
            Assert.False(FileSupport.IsFileWritable(_fixture.UnwritableFilePath));
            Assert.True(FileSupport.IsFileWritable(_fixture.WritableFilePath));
            Assert.True(FileSupport.IsFileWritable(_fixture.NonExistentFilePath));
            Assert.False(FileSupport.IsFileWritable(_fixture.InvalidFilePath));
        }

        [Fact]
        public void IsFolderReadableTest()
        {
            Assert.False(FileSupport.IsFolderReadable(_fixture.UnreadableFolderPath));
            Assert.True(FileSupport.IsFolderReadable(_fixture.ReadableFolderPath));
            Assert.False(FileSupport.IsFolderReadable(_fixture.InvalidFilePath));
        }
        [Fact]
        public void IsFolderWritableTest()
        {
            Assert.False(FileSupport.IsFolderWritable(_fixture.UnwritableFolderPath));
            Assert.False(FileSupport.IsFolderWritable(_fixture.UnreadableFolderPath));
            Assert.True(FileSupport.IsFolderWritable(_fixture.ReadableFolderPath));
            Assert.False(FileSupport.IsFolderWritable(_fixture.InvalidFilePath));
        }

        [Fact]
        public void IsNormalFileTest()
        {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.False(FileSupport.IsNormalFile(_fixture.HiddenFilePath));
            }
            Assert.False(FileSupport.IsNormalFile(_fixture.TestDir));
            Assert.True(FileSupport.IsNormalFile(_fixture.WritableFilePath));
        }
        [Fact]
        public void IsReadableTest()
        {
            Assert.True(FileSupport.IsFileReadable(_fixture.UnwritableFilePath));
            Assert.True(FileSupport.IsFileReadable(_fixture.WritableFilePath));
            Assert.False(FileSupport.IsFileReadable(_fixture.UnreadableFilePath));
            Assert.False(FileSupport.IsFileReadable(_fixture.InvalidFilePath));
        }
        [Fact]
        public void HasFilePermissionTest()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.True(FileSupport.HasFilePermission(_fixture.WritableFilePath, FileSystemRights.Write));
                Assert.False(FileSupport.HasFilePermission(_fixture.UnwritableFilePath, FileSystemRights.Write));
            }
            else
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void HasDirectoryPermissionTest()
        {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.False(FileSupport.HasFolderPermission(_fixture.UnwritableFolderPath, FileSystemRights.Write));
                Assert.True(FileSupport.HasFolderPermission(_fixture.TestDir, FileSystemRights.ExecuteFile));
            }
            else
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void IsReadOnlyTest()
        {
            Assert.False(FileSupport.IsReadOnly(_fixture.WritableFilePath));
            Assert.True(FileSupport.IsReadOnly(_fixture.ReadOnlyFilePath));
        }

        [Fact]
        public void DoesDirectoryExistTest()
        {
            Assert.True(FileSupport.DoesFolderExist(_fixture.TestDir));
            Assert.False(FileSupport.DoesFolderExist(_fixture.UnwritableFilePath));
        }

        [Fact]
        public void DoesFileExistTest()
        {
            Assert.True(FileSupport.DoesFileExist(_fixture.UnwritableFilePath));
            Assert.False(FileSupport.DoesFileExist(_fixture.TestDir));
        }

        [Fact]
        public void IsValidPathTest()
        {
            Assert.True(FileSupport.IsValidPath(_fixture.WritableFilePath));
            Assert.True(FileSupport.IsValidPath(Path.GetFileName(_fixture.WritableFilePath)));
            Assert.True(FileSupport.IsValidPath(".."));
            Assert.False(FileSupport.IsValidPath("  "));
            Assert.False(FileSupport.IsValidPath(""));
            Assert.False(FileSupport.IsValidPath(_fixture.InvalidFilePath));
        }
        [Fact]
        public void GetFullPathTest()
        {
            string current = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(_fixture.TestDir);
            string name = "Writable.txt";
            Assert.Equal(FileSupport.GetFullPath(name), _fixture.WritableFilePath);
            Directory.SetCurrentDirectory(current);
        }
        [Theory]
        [InlineData("file/[]()<>|?*'name", "file-[]()-----'name", OSCompatibility.Windows)]
        [InlineData("file/[]()<>|?*'name", "file-[]()<>|?*'name", OSCompatibility.AllUnix)]
        public void MakeValidFileNameTest(string invalidName, string validName, OSCompatibility platform)
        {
            if(OSCompatibilitySupport.IsComplatible(platform))
            {
                string name = FileSupport.MakeValidFileName(invalidName);
                Assert.Equal(validName, name);
                Assert.Equal(validName, FileSupport.MakeValidFileName("", "", validName));
            }
            else
            {
                Assert.True(true);
            }
        }
        [Theory]
        [InlineData(@"c:\a\b\c",true, OSCompatibility.Windows)]
        [InlineData(@"c:\a\b\c ", false, OSCompatibility.Windows)]
        [InlineData(@" c:\a\b\c ", false, OSCompatibility.Windows)]
        [InlineData(@"c:\a\b\c?",false, OSCompatibility.Windows)]
        [InlineData("c:\\a\\b\\c\"",false, OSCompatibility.Windows)]
        [InlineData(@"c:\a\b\c*",false, OSCompatibility.Windows)]
        [InlineData(@"/c/a/b/c", true, OSCompatibility.AllUnix)]
        [InlineData(@"/c/a/b/c ", false, OSCompatibility.AllUnix)]
        [InlineData(@" /c/a/b/c ", false, OSCompatibility.AllUnix)]
        [InlineData(@"/c/a/b/c?", true, OSCompatibility.AllUnix)]
        [InlineData("/c/a/b/c\"", true, OSCompatibility.AllUnix)]
        [InlineData(@"/c/a/b/c*", true, OSCompatibility.AllUnix)]
        [InlineData(@"", false, OSCompatibility.Any)]
        public void IsvalidPathTest(string path, bool valid, OSCompatibility platform)
        {
            if(OSCompatibilitySupport.IsComplatible(platform))
            {
                bool v1 = FileSupport.IsValidPath(path);
                Assert.Equal(valid, v1);
            }
            else
            {
                Assert.True(true);
            }
        }
        [Theory]
        [InlineData(@"c", true, OSCompatibility.Any)]
        [InlineData(@"c ", false, OSCompatibility.Windows)]
        [InlineData(@"c?", false, OSCompatibility.Windows)]
        [InlineData("c\"", false, OSCompatibility.Windows)]
        [InlineData(@"c*", false, OSCompatibility.Windows)]
        [InlineData(@"c ", false, OSCompatibility.AllUnix)]
        [InlineData(@"c?", true, OSCompatibility.AllUnix)]
        [InlineData("c\"", true, OSCompatibility.AllUnix)]
        [InlineData(@"c*", true, OSCompatibility.AllUnix)]
        [InlineData(@"", false, OSCompatibility.Any)]
        public void IsvalidFilenameTest(string path, bool valid, OSCompatibility platform)
        {
            if(OSCompatibilitySupport.IsComplatible(platform))
            {
                bool v1 = FileSupport.IsValidFilename(path);
                Assert.Equal(valid, v1);
            }
            else
            {
                Assert.True(true);
            }
        }
    }
}
