using System;
using System.Reflection;

namespace OSECore.Object
{
    public class ObjectSupport
    {
        public static object CloneFromCopyConstructor(System.Object d)
        {
            if (d != null)
            {
                Type t = d.GetType();
                foreach (ConstructorInfo ci in t.GetConstructors())
                {
                    ParameterInfo[] pi = ci.GetParameters();
                    if (pi.Length == 1 && pi[0].ParameterType == t)
                    {
                        return ci.Invoke(new object[] {d});
                    }
                }
            }

            return null;
        }
    }
}