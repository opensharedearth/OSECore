using System;

namespace OSEUIForms.WPF.App
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AboutBoxImageAttribute : Attribute
    {
        public string Path { get; set; }
        public AboutBoxImageAttribute(string path)
        {
            Path = path;
        }
    }
}