using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OSETesting
{
    public static class OSCompatibilitySupport
    {
        public static bool IsComplatible(OSCompatibility osc)
        {
            if ((osc & OSCompatibility.Windows) == OSCompatibility.Windows && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return true;
            else if ((osc & OSCompatibility.Linux) == OSCompatibility.Linux && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return true;
            else if ((osc & OSCompatibility.OSX) == OSCompatibility.OSX && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return true;
            else if ((osc & OSCompatibility.FreeBSD) == OSCompatibility.FreeBSD && RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                return true;
            return false;
        }
        public static OSCompatibility GetPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return OSCompatibility.Windows;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return OSCompatibility.Linux;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return OSCompatibility.OSX;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                return OSCompatibility.FreeBSD;
            else
                return OSCompatibility.None;
        }
        public static string GetPlatformName()
        {
            return GetPlatform().ToString();
        }
        public static string ChangeLineEndings(string s, OSCompatibility from, OSCompatibility to)
        {
            if (String.IsNullOrEmpty(s))
                return s;
            else if ((from | to) != 0)
                return s;
            else if (to == OSCompatibility.Windows)
                return s.Replace("\n", "\r\n");
            else if (from == OSCompatibility.Windows)
                return s.Replace("\r\n", "\n");
            else
                return s;
        }
    }
}
