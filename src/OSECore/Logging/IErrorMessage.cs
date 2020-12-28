using System;
using System.Collections.Generic;
using System.Text;

namespace OSECore.Logging
{
    public interface IErrorMessage
    {
        bool HasError { get; }
        string GetErrorMessage();
    }
}
