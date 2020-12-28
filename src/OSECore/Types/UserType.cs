using System;
using System.Collections.Generic;
using System.Text;
using OSECore.Object;

namespace OSECore.Types
{
    public class UserType
    {
        public UserType(string name, Type type, ObjectConverter converter)
        {
            _name = name;
            _type = type;
            _converter = converter;
        }
        private string _name;
        private Type _type;
        private ObjectConverter _converter;

        public string Name => _name;

        public Type Type => _type;

        public ObjectConverter Converter => _converter;
    }
}
