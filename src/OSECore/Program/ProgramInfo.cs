using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace OSECore.Program
{
    public static class ProgramInfo
    {
        public static Assembly TopLevelAssembly { get; set; } = Assembly.GetEntryAssembly();
        public static string GetProgramName()
        {
            Assembly a = TopLevelAssembly;
            var name = a.GetName();
            return name.Name;
        }
        public static string GetProgramVersion()
        {
            Assembly a = TopLevelAssembly;
            var name = a.GetName();
            return name.Version.ToString();

        }
        public static string GetProgramDescription()
        {
            Assembly a = TopLevelAssembly;
            var description = a.GetCustomAttribute<AssemblyDescriptionAttribute>();
            return description?.Description ?? "";
        }
        public static string GetCopyright()
        {
            Assembly a = TopLevelAssembly;
            var copyright = a.GetCustomAttribute<AssemblyCopyrightAttribute>();
            return copyright?.Copyright ?? "";
        }
        public static string GetCompany()
        {
            Assembly a = TopLevelAssembly;
            var company = a.GetCustomAttribute<AssemblyCompanyAttribute>();
            return company?.Company ?? "";
        }
    }
}
