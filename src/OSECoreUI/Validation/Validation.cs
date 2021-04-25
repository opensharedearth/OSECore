using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace OSECoreUI.Validation
{
    public static class BasicValidation
    {
        public static object ValidateObject(object d, string ex = "Invalid object")
        {
            if (d != null) return d;
            throw new ArgumentException(ex);
        }
        public static string ValidateName(string name, string ex = "Invalid name", int maxLength = 511, string namePattern = "^[a-zA-Z_][0-9a-zA-Z_]*$")
        {
            if(!String.IsNullOrEmpty(name))
            {
                name = name.Trim();
                if (name.Length <= maxLength && Regex.IsMatch(name, namePattern))
                {
                    return name;
                }
            }
            throw new ArgumentException(ex);
        }
        public static int ValidateInt(string iv, string ex = "Invalid integer")
        {
            if(!String.IsNullOrEmpty(iv))
            {
                iv = iv.Trim();
                if(int.TryParse(iv, out int i))
                {
                    return i;
                }
            }
            throw new ArgumentException(ex);
        }
        public static T ValidateEnum<T>(string enums, string ex = "Invalid Enum") where T : struct, System.Enum
        {
            if(!String.IsNullOrEmpty(enums))
            {
                if(Enum.TryParse<T>(enums, true, out T result))
                {
                    return result;
                }
            }
            throw new ArgumentException(ex);
        }
        public static Type ValidateType(string typeString, string ex)
        {
            try
            {
                if (!String.IsNullOrEmpty(typeString))
                {
                    return Type.GetType(typeString.Trim(), true);
                }
            }
            catch(Exception e)
            {
                throw new ArgumentException(ex, e);
            }
            throw new ArgumentException(ex);
        }
    }
}
