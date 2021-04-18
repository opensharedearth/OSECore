using OSELogic.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSELogic.Config
{
    class GeneralParameters
    {
        private static GeneralParameters _instance;
        public static GeneralParameters Instance
        {
            get => _instance;
            set => _instance = value;
        }
        private string _workingFolder = null;
        public string WorkingFolder
        {
            get => _workingFolder;
            set => _workingFolder = value;
        }
        public GeneralParameters()
        {
            Load();
        }

        private void Load()
        {
            ConfigSection section = ConfigFile.Instance.FindOrCreate(StandardCommands.Names.GeneralSection);
            _workingFolder = section.GetValue<string>(StandardCommands.Names.WorkingFolder, Directory.GetCurrentDirectory());
        }
        public void Save()
        {
            ConfigSection section = ConfigFile.Instance.FindOrCreate(StandardCommands.Names.GeneralSection);
            section.SetValue(StandardCommands.Names.WorkingFolder, _workingFolder);
        }
    }
}
