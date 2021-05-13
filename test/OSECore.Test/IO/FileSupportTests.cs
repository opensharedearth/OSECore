using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using OSECore.IO;
using Xunit;

#pragma warning disable CA1416

namespace OSECoreTest.IO
{
    public class FileSupportTests : IClassFixture<TestFileFixture>, IDisposable
    {
        private readonly TestFileFixture _fixture;
        public FileSupportTests(TestFileFixture fixture)
        {
            _fixture = fixture;
        }


        public void Dispose()
        {
        }

        [Fact]
        public void IsWritableTest()
        {
            Assert.False(FileSupport.IsFileWritable(_fixture.UnwritableFilePath));
            Assert.True(FileSupport.IsFileWritable(_fixture.WritableFilePath));
            Assert.False(FileSupport.IsFileWritable(_fixture.NonExistentFilePath));
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
            Assert.True(FileSupport.IsFolderWritable(_fixture.UnreadableFolderPath));
            Assert.False(FileSupport.IsFolderWritable(_fixture.InvalidFilePath));
        }

        [Fact]
        public void IsNormalFileTest()
        {
            Assert.False(FileSupport.IsNormalFile(_fixture.HiddenFilePath));
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
            Assert.True(FileSupport.HasFilePermission(_fixture.WritableFilePath, FileSystemRights.Write));
            Assert.False(FileSupport.HasFilePermission(_fixture.UnwritableFilePath, FileSystemRights.Write));
        }

        [Fact]
        public void HasDirectoryPermissionTest()
        {
            Assert.False(FileSupport.HasFolderPermission(_fixture.TestDir, FileSystemRights.Write));
            Assert.True(FileSupport.HasFolderPermission(_fixture.TestDir,FileSystemRights.ExecuteFile));
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
        [Fact]
        public void MakeValidFileNameTest()
        {
            string invalidName = "file[]()<>|\"?*'name";
            string validName = "file[]()------'name";
            string name = FileSupport.MakeValidFileName(invalidName);
            Assert.Equal(name, validName);
            Assert.Equal(validName, FileSupport.MakeValidFileName("", "", validName));
        }
        [Theory]
        [InlineData(@"c:\a\b\c",true)]
        [InlineData(@"c:\a\b\c ", false)]
        [InlineData(@" c:\a\b\c ", false)]
        [InlineData(@"c:\a\b\c?",false)]
        [InlineData("c:\\a\\b\\c\"",false)]
        [InlineData(@"c:\a\b\c*",false)]
        [InlineData(@"", false)]
        public void IsvalidPathTest(string path, bool valid)
        {
            bool v1 = FileSupport.IsValidPath(path);
            Assert.Equal(valid, v1);
        }
        [Theory]
        [InlineData(@"c", true)]
        [InlineData(@"c ", false)]
        [InlineData(@"c?", false)]
        [InlineData("c\"", false)]
        [InlineData(@"c*", false)]
        [InlineData(@"", false)]
        public void IsvalidFilenameTest(string path, bool valid)
        {
            bool v1 = FileSupport.IsValidFilename(path);
            Assert.Equal(valid, v1);
        }
    }
}
