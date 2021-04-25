using System;
using System.Collections.Generic;
using System.Text;
using OSECore.Text;

namespace OSELogic.Command
{
    public class CommandResult
    {
        List<String> _messages = new List<string>();
        public bool Succeeded { get; private set; } = true;
        public bool Continue { get; private set; } = true;
        public bool HasMessages => _messages.Count > 0;
        public int ErrorCode { get; private set; } = 0;
        public CommandResult(bool succeeded = true, string message = null, Exception ex = null)
        {
            Succeeded = succeeded;
            Continue = succeeded;
            AddMessage(message);
            AddException(ex);
        }
        public CommandResult(bool b)
        {
            Continue = b;
        }
        public CommandResult(int error)
        {
            ErrorCode = error;
        }
        public CommandResult(bool succeeded, bool cont, int error, string[] message)
        {
            Succeeded = succeeded;
            Continue = cont;
            ErrorCode = error;
            _messages.AddRange(message);
        }
        public string GetMessages()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in _messages) sb.AppendLine(s);
            return sb.ToString();
        }
        public void AddMessage(string message = null)
        {
            if(message != null)
            {
                _messages.AddRange(message.SplitLines());
            }
            else
            {
                _messages.Add("");
            }
        }
        public void AddException(Exception ex)
        {
            if(ex != null)
            {
                AddMessage(ex.Message);
                AddException(ex.InnerException);
            }
        }
        public static CommandResult Aggregate(CommandResult[] results)
        {
            bool success = true;
            bool cont = true;
            int error = 0;
            foreach (var result in results)
            {
                if (!result.Succeeded)
                    success = false;
                if (!result.Continue)
                    cont = false;
                if (result.ErrorCode != 0)
                    error = result.ErrorCode;
            }
            List<string> messages = new List<string>();
            foreach (var result in results)
                if (result.Succeeded == success)
                    messages.AddRange(result._messages);
            return new CommandResult(success, cont, error, messages.ToArray());
        }
        public void Append(CommandResult r)
        {

            if(Succeeded == r.Succeeded)
            {
                _messages.AddRange(r._messages);
            }
            else if(!r.Succeeded)
            {
                Succeeded = false;
                _messages.Clear();
                _messages.AddRange(r._messages);
            }
            if (!r.Continue)
                Continue = false;
            if (r.ErrorCode != 0)
                ErrorCode = r.ErrorCode;
        }
        public override string ToString()
        {
            return GetMessages();
        }
    }
}
