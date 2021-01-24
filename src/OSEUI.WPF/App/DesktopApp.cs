using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using OSECore.Object;
using OSECoreUI.App;
using OSECoreUI.Document;
using OSECoreUI.Undo;
using OSEUI.WPF.Document;

namespace OSEUI.WPF.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public class DesktopApp : Application
    {
        public DesktopDocument CurrentDocument { get; set; }
        public BaseAppConfig Config { get; }
        public UndoRedoFramework UndoRedo { get; } = new UndoRedoFramework();
        public DocType DefaultDocType { get; }

        public DesktopAppSettings Settings => Config.Settings as DesktopAppSettings;


        public static DesktopApp Instance { get; private set; }

        protected DesktopApp(DesktopAppSettings settings, DocType defaultDocType)
        {
            Config = new BaseAppConfig(settings);
            DefaultDocType = defaultDocType;
            DocTypeRegistry.Instance.Add(defaultDocType);
            Instance = this;
            NewDocument();
        }

        public void NewDocument()
        {
            ConstructorInfo ci = DefaultDocType.Type.GetConstructor(new Type[0]);
            if (ci != null)
            {
                CurrentDocument = ci.Invoke(null) as DesktopDocument;
            }
        }

        public void CloseDocument()
        {
            if (CurrentDocument != null)
            {
                if (DesktopDocument.Close(MainWindow, CurrentDocument))
                {
                    CurrentDocument = null;
                }
            }
        }

        public void UpdateDocumentFolder()
        {
            if (CurrentDocument != null)
            {
                string folder = Path.GetDirectoryName(CurrentDocument.Path);
                if (!String.IsNullOrEmpty(folder))
                {
                    Settings.DocumentFolder = folder;
                }
            }
        }
    }
}
