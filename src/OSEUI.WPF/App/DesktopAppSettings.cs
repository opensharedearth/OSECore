using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OSECoreUI.App;
using BaseAppSettings = OSECoreUI.App.BaseAppSettings;

namespace OSEUI.WPF.App
{
    [Serializable]
    public class DesktopAppSettings : BaseAppSettings
    {
        private const string MainWindowBoundsKey = "MainWindowBounds";
        private const string RecentFilesKey = "RecentFiles";
        public DesktopAppSettings()
        {

        }

        public DesktopAppSettings(SerializationInfo info, StreamingContext context)
        : base(info,context)
        {
            try
            {
                _mainWindowBounds = (Rect)info.GetValue(MainWindowBoundsKey, typeof(Rect));
                _recentFiles = info.GetValue(RecentFilesKey, typeof(RecentFiles)) as RecentFiles;
            }
            catch (Exception e)
            {
                Trace.WriteLine("Unable to deserialize desktop app settings: " + e.Message);
            }

        }
        private Rect _mainWindowBounds = new Rect();
        private RecentFiles _recentFiles = new RecentFiles();

        public Rect MainWindowBounds
        {
            get =>_mainWindowBounds;
            set
            {
                if (_mainWindowBounds != value)
                {
                    _mainWindowBounds = value;
                    Dirty();
                }
            }
        }

        public override bool IsDirty => base.IsDirty || RecentFiles.IsDirty;

        public RecentFiles RecentFiles => _recentFiles;

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(MainWindowBoundsKey, MainWindowBounds);
            info.AddValue(RecentFilesKey, RecentFiles);
            base.GetObjectData(info, context);
        }

        public void ApplyMainWindowBounds(Window mainWindow)
        {
            Rect r = MainWindowBounds;
            Rect desktop = new Rect(SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenTop, SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);
            if (desktop.Contains(r) && r.Width > 0.0 && r.Height > 0.0)
            {
                mainWindow.Left = r.Left;
                mainWindow.Top = r.Top;
                mainWindow.Height = r.Height;
                mainWindow.Width = r.Width;
            }

        }

        public void UpdateMainWindowBounds(Window mainWindow)
        {
            MainWindowBounds = new Rect(mainWindow.Left, mainWindow.Top, mainWindow.Width, mainWindow.Height);
        }
    }
}
