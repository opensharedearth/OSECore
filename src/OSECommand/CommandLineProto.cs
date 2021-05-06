﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OSECommand
{
    public class CommandLineProto : CommandLine, IEquatable<CommandLineProto>, IComparable<CommandLineProto>
    {
        public CommandLineProto(string name, Usage usage, Func<CommandLine, CommandResult> func, params CommandArgProto[] args)
            : base(args)
        {
            CommandArgProto proto = new CommandArgProto(name, 0, usage);
            Insert(0, proto);
            _command = func;
        }
        private Func<CommandLine, CommandResult> _command = null;
        public CommandResult Execute(string[] fields)
        {
            return Execute(new CommandLine(fields));
        }
        public CommandResult Execute(CommandLine argList)
        {
            CommandResult result = new CommandResult();
            CommandLine args = Resolve(argList, result);
            if(result.Succeeded)
            {
                return _command(args);
            }
            return result;
        }

        public CommandLine CheckSwitches(CommandLine args, CommandResult result)
        {
            int iPos = 0;
            int nArg = 0;
            CommandLine args1 = new CommandLine();
            foreach(var arg in args)
            {
                if (arg.IsPositional)
                {
                    if(arg.PositionIndex == iPos)
                    {
                        iPos++;
                        args1.Add(new CommandArg(arg.PositionIndex - nArg, arg.Value));
                    }
                }
                else
                {
                    var proto = GetSwitch(arg.Name, arg.Mnemonic) as CommandArgProto;
                    if(proto == null)
                    {
                        result.Append(new CommandResult(false, $"{arg.ToString()} is not a valid option"));
                    }
                    else if(proto.HasArgument)
                    {
                        var switchArg = args.GetPositional(iPos++);
                        if(switchArg != null)
                        {
                            args1.Add(new CommandArg(arg.Name, arg.Mnemonic, switchArg.Value));
                            ++nArg;
                        }
                        else
                        {
                            result.Append(new CommandResult(false, $"No argument for option {arg.Name}"));
                        }
                    }
                    else
                    {
                        args1.Add(arg);
                    }
                }
            }
            return args1;
        }
        public CommandLine CheckPositionals(CommandLine args, CommandResult result)
        {
            CommandLine args1 = new CommandLine();
            int pos = 0;
            foreach(CommandArg arg in args)
            {
                if(arg.IsPositional)
                {
                    var proto = GetPositional(pos) as CommandArgProto;
                    if(proto == null)
                    {
                        result.Append(new CommandResult(false, $"Too many arguments for command"));
                        break;
                    }
                    else
                    {
                        args1.Add(arg);
                        if(!proto.HasMultiple)
                            pos++;
                    }
                }
                else
                {
                    args1.Add(arg);
                }
            }
            return args1;
        }
        public void CheckRequired(CommandLine args, CommandResult result)
        {
            foreach(var proto in this)
            {
                if((proto as CommandArgProto).IsRequired)
                {
                    if(proto.IsSwitch && args.GetSwitch(proto.Name) == null)
                    {
                        result.Append(new CommandResult(false, $"Required option {proto.Name} missing"));
                    }
                    else if(proto.IsPositional && args.GetPositional(proto.PositionIndex) == null)
                    {
                        result.Append(new CommandResult(false, $"Required argument {proto.Name} missing"));
                    }
                }
            }
        }
        public CommandLine Resolve(CommandLine args, CommandResult result)
        {
            args = CheckSwitches(args, result);
            args = CheckPositionals(args, result);
            CheckRequired(args, result);
            foreach(var arg in args)
            {
                if(arg.IsPositional)
                {
                    result.Append((GetPositional(arg.PositionIndex) as CommandArgProto).Validate(arg));
                }
                else if(arg.IsSwitch)
                {
                    result.Append((GetSwitch(arg.Name, arg.Mnemonic) as CommandArgProto).Validate(arg));
                }
            }
            return args;
        }
        public string GetProto()
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach(CommandArgProto proto in this)
            {
                if(proto.IsPositional)
                {
                    if (!first) sb.Append(' ');
                    first = false;
                    sb.Append(proto.Value);
                }
            }
            return sb.ToString();
        }
        public string GetName()
        {
            var arg = GetPositional(0);
            if (arg != null)
                return arg.Value;
            else
                return "";
        }
        public string GetDescription()
        {
            var arg = GetPositional(0) as CommandArgProto;
            if (arg != null)
                return arg.Usage.Description;
            else
                return "";
        }

        public bool Equals(CommandLineProto other)
        {
            if(other != null)
            {
                return GetProto().Equals(other.GetProto());
            }
            return false;
        }

        public int CompareTo(CommandLineProto other)
        {
            if(other != null)
            {
                return GetProto().CompareTo(other.GetProto());
            }
            return 1;
        }
        public override bool Equals(object obj)
        {
            if(obj is CommandLineProto proto)
            {
                return Equals(proto);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return GetProto().GetHashCode();
        }
        public static bool operator==(CommandLineProto a, CommandLineProto b)
        {
            if ((object)a == null)
                return (object)b == null;
            else if ((object)b == null)
                return false;
            else
                return a.Equals(b);
        }
        public static bool operator!=(CommandLineProto a, CommandLineProto b)
        {
            if ((object)a == null)
                return (object)b != null;
            else if ((object)b == null)
                return true;
            else
                return !a.Equals(b);
        }
        public static bool operator<(CommandLineProto a, CommandLineProto b)
        {
            if ((object)a == null)
                return (object)b != null;
            else if ((object)b == null)
                return false;
            else
                return a.CompareTo(b) < 0;
        }
        public static bool operator>(CommandLineProto a, CommandLineProto b)
        {
            if ((object)a == null)
                return (object)b != null;
            else if ((object)b == null)
                return true;
            else
                return a.CompareTo(b) > 0;
        }
        public static bool operator<=(CommandLineProto a, CommandLineProto b)
        {
            if ((object)a == null)
                return true;
            else if ((object)b == null)
                return false;
            else
                return a.CompareTo(b) <= 0;
        }
        public static bool operator >=(CommandLineProto a, CommandLineProto b)
        {
            if ((object)a == null)
                return (object)b == null;
            else if ((object)b == null)
                return true;
            else
                return a.CompareTo(b) >= 0;
        }
    }
}
