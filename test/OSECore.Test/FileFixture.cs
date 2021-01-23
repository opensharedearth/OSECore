using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OSECoreTest
{
    public class FileFixture : IDisposable
    {
        public string TestDir;
        public FileFixture()
        {
            TestDir = CreateTestDirectory();

        }
        public virtual void Dispose()
        {
            Directory.Delete(TestDir);
        }
        protected string CreateTestDirectory()
        {
            string temppath = Path.GetTempPath();
            string testdir = Path.Combine(temppath, Path.GetRandomFileName());
            Directory.CreateDirectory(testdir);
            return testdir;
        }
        protected void CreateFile(string path)
        {
            using (FileStream fs = File.Create(path))
            {

            }
        }
    }
}
