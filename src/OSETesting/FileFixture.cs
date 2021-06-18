using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace OSETesting
{
    public class FileFixture : IDisposable
    {
        public string TestDir { get; }
        private bool _deleteOnDispose = true;
        public FileFixture(string testFolder = "OSETesting")
        {
            System.Diagnostics.Trace.WriteLine("In file fixture constructor");
            TestDir = CreateTestDirectory(testFolder);

        }
        public virtual void Dispose()
        {
            if(_deleteOnDispose && Directory.Exists(TestDir))
            {
                Directory.Delete(TestDir, true);
            }
        }
        protected string CreateTestDirectory(string testFolder = "OSETesting")
        {
            string testbasedir = Path.GetTempPath();
            string scratchpath = Environment.GetEnvironmentVariable("SCRATCH");
            if(!String.IsNullOrEmpty(scratchpath))
            {
                testbasedir = scratchpath;
                _deleteOnDispose = false;
            }
            string testDirPath = testFolder;
            if(!Path.IsPathFullyQualified(testDirPath))
            {
                testDirPath = Path.Combine(testbasedir, testFolder);
            }

            if (!Directory.Exists(testDirPath))
            {
                Directory.CreateDirectory(testDirPath);
            }
            return testDirPath;
        }
        protected void CreateFile(string path)
        {
            using (FileStream fs = File.Create(path))
            {

            }
        }
    }
}
