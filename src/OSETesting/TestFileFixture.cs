﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace OSETesting
{
    public class TestFileFixture : FileFixture
    {
        public TestFileFixture(string testDir = "")
            : base(testDir)
        {

        }
        public const string InputFolder = "Input";
        public const string OutputFolder = "Output";
        public string GetInputFilePath(string file)
        {
            return Path.Combine(GetInputPath(), file);
        }
        public string GetInputPath()
        {
            return Path.Combine(GetBasePath(), InputFolder);
        }
        public string GetBasePath()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }
        public string GetOutputPath()
        {
            return Path.Combine(GetBasePath(), OutputFolder);
        }
        public string GetOutputFilePath(string file)
        {
            return Path.Combine(GetOutputPath(), file);
        }
        public string GetPlatformOutputFilePath(string file)
        {
            return Path.Combine(GetOutputPath(), OSCompatibilitySupport.GetPlatformName(), file);
        }
        public string GetTempFilePath(string file)
        {
            return Path.Combine(TestDir, file);
        }
        public bool CompareFiles(string path0, string path1)
        {
            if(File.Exists(path0) && File.Exists(path1))
            {
                string s0 = File.ReadAllText(path0);
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    s0 = s0.Replace("\r\n", Environment.NewLine);
                }
                string s1 = File.ReadAllText(path1);
                return String.Compare(s0, s1) == 0;
            }
            return false;
        }
    }
}
