using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace OSECore.Object
{
    public class ObjectConverters : Dictionary<Type, ObjectConverter>
    {
        static ObjectConverters s_instance = new ObjectConverters();
        public static ObjectConverters Instance => s_instance;

        public static ObjectConverter Find(Type t)
        {
            if(s_instance.TryGetValue(t, out ObjectConverter dc))
            {
                return dc;
            }
            return null;
        }

        public static void Add(ObjectConverter dc)
        {
            Instance[dc.Foundation] = dc;
        }

        public static string Format(string format, params object[] dd)
        {
            if (!String.IsNullOrEmpty(format))
            {
                List<object> ss = new List<object>();
                foreach (object d in dd)
                {
                    if (d == null)
                    {
                        ss.Add("(null)");
                    }
                    else
                    {
                        ObjectConverter oc = Find(d.GetType());
                        if (oc != null)
                        {
                            ss.Add(oc.Format(d));
                        }
                        else
                        {
                            ss.Add(d.ToString());
                        }
                    }
                }

                return String.Format(format, ss.ToArray());
            }

            return "";
        }
    }
}
