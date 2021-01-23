using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using Microsoft.Win32;
using OSECore.IO;
using OSECore.Logging;
using OSECoreUI.Annotations;
using OSECoreUI.Document;
using OSEUI.WPF.App;

namespace OSEUI.WPF.Document
{
    public class DesktopDocument : BaseDocument
    {
        public DesktopDocument()
        {

        }

        public DesktopDocument(SerializationInfo info, StreamingContext context)
        : base(info, context)
        {

        }

        public static bool Close(Window window, DesktopDocument document)
        {
            if (document != null)
            {
                if (document.IsDirty)
                {
                    MessageBoxResult result = MessageBox.Show(window, "Save '" + document.Title + "'?",
                        "Save before closing", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                    if (result == MessageBoxResult.Yes)
                    {
                        return Save(window, document);

                    }
                    else if (result == MessageBoxResult.Cancel)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool SaveAs(Window window, DesktopDocument document)
        {
            string filename = FileSupport.MakeValidFileName(document.Title);
            SaveFileDialog fd = new SaveFileDialog
            {
                InitialDirectory = DesktopApp.Instance.Settings.DocumentFolder,
                FileName = filename,
                Filter = BaseDocument.GetFilterString(true, document.DocType),
                FilterIndex = 0,
            };
            if (fd.ShowDialog(window) == true)
            {
                string path = fd.FileName;
                if (File.Exists(path) && path != document.Path)
                {
                    if (MessageBox.Show(window, "File '" + path + "' exists.  Overwrite?", "Save Document",
                            MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK)
                    {
                        return document.Save(path, true);
                    }

                    return false;
                }
                else if (path == document.Path)
                {
                    return document.Save();
                }
                else
                {
                    return document.Save(path, false);
                }
            }

            return false;

        }

        public static bool Save(Window window, DesktopDocument document)
        {
            if (document != null)
            {
                if (document.IsDirty)
                {
                    if (String.IsNullOrEmpty(document.Path))
                    {
                        return SaveAs(window, document);
                    }

                    return document.Save();
                }

                return true;
            }

            return false;
        }

        public static DesktopDocument Open(Window window, DocType docType, ResultLog log = null)
        {
            OpenFileDialog fd = new OpenFileDialog
            {
                InitialDirectory = DesktopApp.Instance.Settings.DocumentFolder,
                DefaultExt = docType.Extension,
                Filter = GetFilterString(true, docType),
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (fd.ShowDialog(window) == true)
            {
                return BaseDocument.Open(fd.FileName, log) as DesktopDocument;
            }

            return null;
        }

    }
}