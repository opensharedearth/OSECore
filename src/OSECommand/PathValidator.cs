using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSECore.IO;
using OSEConfig;

namespace OSECommand
{
    public class PathValidator : ArgValidator
    {
        [Flags]
        public enum Disposition
        {
            None = 0,
            Exists = 1,
            Readable = 3,
            Writable = 4,
            Folder = 9,
        }
        private bool Exists => (PathDisposition & Disposition.Exists) == Disposition.Exists;
        private bool Readable => (PathDisposition & Disposition.Readable) == Disposition.Readable;
        private bool Writable => (PathDisposition & Disposition.Writable) == Disposition.Writable;
        private bool Folder => (PathDisposition & Disposition.Folder) == Disposition.Folder;
        private bool HasDefaultExt => !String.IsNullOrEmpty(DefaultExt);
        private string DefaultExt { get; } = "";
        private Disposition PathDisposition { get; } = Disposition.None;
        public PathValidator(string defaultExt = "", Disposition disposition = Disposition.None)
        {
            if (!String.IsNullOrEmpty(defaultExt) && defaultExt[0] != '.')
                throw new ArgumentException("Default extension must begin with a '.'");
            DefaultExt = defaultExt;
            PathDisposition = disposition;
        }

        public override CommandResult Validate(CommandArg arg)
        {
            CommandResult result = base.Validate(arg);
            string path = arg.Value;
            if (HasDefaultExt && Path.GetExtension(path) == "")
            {
                path += DefaultExt;
            }
            if (!Path.IsPathFullyQualified(path))
            {
                try
                {
                    path = Path.Combine(GeneralParameters.Instance.WorkingFolder, path);
                }
                catch (Exception ex)
                {
                    result.Append(new CommandResult(false, "Invalid path:", ex));
                }
            }
            if (result.Succeeded)
            {
                if(Exists && !File.Exists(path))
                {
                    return new CommandResult(false, $"File '{path}' does not exist.");
                }
                if(Exists && Folder && !Directory.Exists(path))
                {
                    return new CommandResult(false, $"Folder '{path}' does not exist.");
                }
                if(Readable && !FileSupport.IsFileReadable(path))
                {
                    return new CommandResult(false, $"File '{path}' is not readable.");
                }
                if(Writable && !FileSupport.IsFileWritable(path))
                {
                    return new CommandResult(false, $"File '{path}' is not writable.");
                }

                arg.ResolvedValue = path;
            }
            return result;
        }
    }
}
