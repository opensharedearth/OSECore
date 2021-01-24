using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using OSECoreUI.Document;
using OSEUI.WPF.App;

namespace OSEUIDesktop.WPF.Sample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : DesktopApp
    {
        public App()
            : base(new SampleSettings(), new DocType(
                "Sample", typeof(SampleDocument), ".osedoc", "OSE Sample Document", "(Untitled)"
                ))
        {
            Instance = this;
        }
        public new static App Instance { get; private set; }

        bool _abort = false;
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            if (!Config.EvaluateArguments(e.Args))
            {
                _abort = true;
                Application.Current.Shutdown();
            }
            Config.Load();
        }
        private void App_OnExit(object sender, ExitEventArgs e)
        {
            if (!_abort)
            {
                DesktopApp.Instance.Config.Save();
            }
        }
    }
}
