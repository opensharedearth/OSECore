using OSECoreUI.App;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSEConfig
{
    public class GeneralParameters : IDirty
    {
        static GeneralParameters()
        {
            _instance = new GeneralParameters();
        }
        public struct Names
        {
            public const string GeneralSection = "General";
            public const string WorkingFolder = "working-folder";
        }
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
            set
            {
                if(_workingFolder != value)
                {
                    _workingFolder = value;
                    Dirty();
                }
            }
        }

        public bool IsDirty => _isDirty;

        public GeneralParameters()
        {
            Load();
        }

        private void Load()
        {
            ConfigSection section = ConfigFile.Instance.FindOrCreate(Names.GeneralSection);
            _workingFolder = section.GetValue<string>(Names.WorkingFolder, Directory.GetCurrentDirectory());
        }
        public void Save()
        {
            ConfigSection section = ConfigFile.Instance.FindOrCreate(Names.GeneralSection);
            section.SetValue(Names.WorkingFolder, _workingFolder);
        }
        private bool _isDirty = false;
        public void Dirty()
        {
            _isDirty = true;
        }

        public void Undirty()
        {
            if (_isDirty)
            {
                Save();
                _isDirty = false;
            }
        }
    }
}
