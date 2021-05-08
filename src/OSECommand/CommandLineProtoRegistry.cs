using System;
using System.Collections.Generic;
using System.Text;

namespace OSECommand
{
    public class CommandLineProtoRegistry
    {
        private Dictionary<string, List<CommandLineProto>> _map = new Dictionary<string, List<CommandLineProto>>(StringComparer.CurrentCultureIgnoreCase);
        public static CommandLineProtoRegistry Instance { get; } = new CommandLineProtoRegistry();
        public CommandLineProtoRegistry()
        {
        }

        public void Register(CommandLineProto proto)
        {
            if(proto != null)
            {
                CommandArg name = proto.GetPositional(0);
                if(name != null)
                {
                    Register(name.Value, proto);
                }
                else
                    throw new ArgumentException($"Command cannot be registered");
            }
            else
                throw new ArgumentNullException("Proto argument cannot be null");
        }
        private void Register(string name, CommandLineProto proto)
        {
            if(_map.TryGetValue(name, out List<CommandLineProto> list))
            {
                list.Add(proto);
            }
            else
            {
                List<CommandLineProto> newList = new List<CommandLineProto>();
                newList.Add(proto);
                _map.Add(name, newList);
            }
        }
        public CommandLineProto Find(CommandLine args)
        {
            if(args != null)
            {
                CommandArg name = args.GetPositional(0);
                if(name != null)
                {
                    CommandLineProto[] protos = Find(name.Value);
                    if (protos.Length == 0)
                        return null;
                    else if (protos.Length == 1)
                        return protos[0];
                    else
                        return Find(protos, 1, args);
                }
            }
            return null;
        }
        public CommandLineProto[] GetAll()
        {
            List<CommandLineProto> allproto = new List<CommandLineProto>();
            foreach(var list in _map.Values)
            {
                foreach(var proto in list)
                {
                    allproto.Add(proto);
                }
            }
            allproto.Sort();
            return allproto.ToArray();
        }
        private CommandLineProto[] Find(CommandLineProto[] protos, CommandLine args)
        {
            List<CommandLineProto> protos1 = new List<CommandLineProto>();
            foreach(var proto in protos)
            {
                if(proto.HasRequired(args))
                {
                    protos1.Add(proto);
                }
            }
            return protos1.ToArray();
        }

        private CommandLineProto Find(CommandLineProto[] protos, int pos, CommandLine args)
        {
            CommandArg name = args.GetPositional(pos);
            if (name == null)
            {
                foreach(var proto in protos)
                {
                    if (proto.HasRequired(args))
                        return proto;
                }
                return null;
            }
            else
            {
                List<CommandLineProto> matches = new List<CommandLineProto>();
                foreach(var proto in protos)
                {
                    var protoArg = proto.GetPositional(pos);
                    if (protoArg != null && string.Compare(protoArg.Value, name.Value, true) == 0)
                        matches.Add(proto);
                }
                if (matches.Count == 0)
                    return null;
                else if (matches.Count == 1)
                    return matches[0];
                else
                    return Find(matches.ToArray(), pos + 1, args);
            }
        }

        public CommandLineProto[] Find(string name)
        {
            if(_map.TryGetValue(name, out List<CommandLineProto> list))
            {
                return list.ToArray();
            }
            return new CommandLineProto[0];
        }
    }
}
