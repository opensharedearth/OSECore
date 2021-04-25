using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace OSECore.Program
{
    public static class ProgramInfo
    {
        public static string GetProgramName()
        {
            Assembly a = Assembly.GetEntryAssembly();
            var name = a.GetName();
            return name.Name;
        }
        public static string GetProgramVersion()
        {
            Assembly a = Assembly.GetEntryAssembly();
            var name = a.GetName();
            return name.Version.ToString();

        }
        public static string GetProgramDescription()
        {
            Assembly a = Assembly.GetEntryAssembly();
            var description = a.GetCustomAttribute<AssemblyDescriptionAttribute>();
            return description?.Description ?? "";
        }
    }
}
