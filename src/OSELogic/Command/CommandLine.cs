using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using OSECore.Text;

namespace OSELogic.Command
{
    public class CommandLine : IList<CommandArg>
    {
        List<CommandArg> _list = new List<CommandArg>();
        public CommandLine()
        {

        }
        public CommandLine(string line)
        {
            var result = ParseArguments(line);
            if (!result.Succeeded) throw new ArgumentException(result.ToString());
        }
        public CommandLine(string[] fields)
        {
            var result = ParseArguments(fields);
            if (!result.Succeeded) throw new ArgumentException(result.ToString());
        }
        public CommandLine(IEnumerable<CommandArg> args)
        {
            _list.AddRange(args);
        }
        public CommandArg this[int index] { get => _list[index]; set => throw new NotImplementedException(); }

        public CommandArg this[string name] 
        {
            get
            {
                if (_list.Count == 0) return CommandArg.Null;
                var arg = (from d in _list where d.Name == name select d).FirstOrDefault();
                return arg == null ? CommandArg.Null : arg;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public int Count => _list.Count;

        public bool IsReadOnly => true;

        public void Add(CommandArg item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(CommandArg item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(CommandArg[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public T GetValue<T>(string name, T defaultValue = default(T)) where T : struct
        {
            var arg = this[name];
            if(arg.Value == null)
            {
                return defaultValue;
            }
            else
            {
                return arg.Value.GetValue<T>();
            }
        }

        public IEnumerator<CommandArg> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(CommandArg item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, CommandArg item)
        {
            _list.Insert(index, item);
        }

        public bool Remove(CommandArg item)
        {
            return _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }
        public static string[] ParseFields(string line)
        {
            List<string> fields = new List<string>();
            List<char> field = new List<char>();
            char quote = '\0';
            char escape = '\0';
            foreach (char c in line)
            {
                switch (c)
                {
                    case '\'':
                    case '"':
                        if (escape != 0)
                        {
                            field.Add(c);
                            escape = '\0';
                        }
                        else if (quote == 0)
                            quote = c;
                        else if (quote == c)
                            quote = '\0';
                        else
                            field.Add(c);
                        break;
                    case '\\':
                        if (escape == 0)
                            escape = c;
                        else
                        {
                            field.Add(c);
                            escape = '\0';
                        }
                        break;
                    case ' ':
                    case '\t':
                        if (quote != 0)
                        {
                            field.Add(c);
                            escape = '\0';
                        }
                        else if (escape != 0)
                        {
                            field.Add(escape);
                            field.Add(c);
                            escape = '\0';
                        }
                        else if (field.Count > 0)
                        {
                            fields.Add(new string(field.ToArray()));
                            field.Clear();
                        }
                        break;
                    default:
                        if (escape != 0)
                        {
                            field.Add(escape);
                            escape = '\0';
                        }
                        field.Add(c);
                        break;
                }
            }
            if (escape != 0)
            {
                field.Add(escape);
            }
            if (field.Count > 0)
            {
                fields.Add(new string(field.ToArray()));
            }
            return fields.ToArray();
        }
        public CommandResult ParseArguments(string commandLine)
        {
            string[] fields = ParseFields(commandLine);
            return ParseArguments(fields);
        }
        public CommandResult ParseArguments(string[] fields)
        {
            int pos = 0;
            CommandResult result = new CommandResult();
            foreach (string field in fields)
            {
                if (field.StartsWith("--"))
                {
                    string name = field.Substring(2);
                    string value = null;
                    int eq = field.IndexOf('=');
                    if (eq < 0)
                    {
                        name = field.Substring(2, eq - 2);
                        value = field.Substring(eq + 1);
                    }
                    var arg = new CommandArg(name);
                    arg.Value = value;
                    this.Add(arg);
                }
                else if (field.StartsWith("-"))
                {
                    foreach (char c in field.Substring(1))
                    {
                        this.Add(new CommandArg(new string(c, 1)));
                    }
                }
                else
                {
                    Add(new CommandArg($"Arg{pos++}", field));
                }
            }
            return result;
        }
        public bool HasSwitch(string name)
        {
            return (from arg in this where arg.IsSwitch && arg.Name == name select arg).Count() > 0;
        }
        public CommandArg GetSwitch(string name)
        {
            return (from arg in this where arg.IsSwitch && arg.Name == name select arg).FirstOrDefault();
        }
    }
}
