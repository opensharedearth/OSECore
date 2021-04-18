using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FMTool
{
    public static class StringExtensions
    {
        [SupportedOSPlatform("windows")]
        public static string EncryptValue(this string s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return s;
            }
            else
            {
                var encoding = new UTF8Encoding();
                byte[] plain = encoding.GetBytes(s);
                byte[] secret = ProtectedData.Protect(plain, null, DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(secret);
            }
        }
        [SupportedOSPlatform("windows")]
        public static string DecryptValue(this string s)
        {
            if(String.IsNullOrEmpty(s))
            {
                return s;
            }
            else
            {
                byte[] secret = Convert.FromBase64String(s);
                byte[] plain = ProtectedData.Unprotect(secret, null, DataProtectionScope.CurrentUser);
                var encoding = new UTF8Encoding();
                return encoding.GetString(plain);
            }
        }
    }
}

