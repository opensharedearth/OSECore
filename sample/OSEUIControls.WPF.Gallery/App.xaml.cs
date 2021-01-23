using OSECoreUI.Undo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OSEUIControls.WPF.Gallery
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static UndoRedoFramework UndoRedo { get; } = new UndoRedoFramework();

    }
}
