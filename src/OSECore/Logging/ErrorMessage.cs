using System;
using System.Collections.Generic;
using System.Text;

namespace OSECore.Logging
{
    public class ErrorMessage : IErrorMessage
    {
        public ErrorMessage(string errorMessage = "")
        {
            _errorMessage = errorMessage;
        }
        private string _errorMessage = "";
        public bool HasError => !String.IsNullOrEmpty(_errorMessage);

        public string Message
        {
            get { return _errorMessage; }
            set
            {
                if (value != null)
                {
                    _errorMessage = value;
                }
            }
        }

        public string GetErrorMessage()
        {
            return _errorMessage;
        }

        public void Clear()
        {
            _errorMessage = "";
        }
    }
}
