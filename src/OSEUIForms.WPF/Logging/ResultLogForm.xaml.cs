using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OSECore.Logging;
using OSECoreUI.Logging;
using OSEUI.WPF;
using OSEUI.WPF.Converters;
using OSEUI.WPF.Printing;

namespace OSEUIForms.WPF.Logging
{
    /// <summary>
    /// Interaction logic for ResultLogForm.xaml
    /// </summary>
    public partial class ResultLogForm : Window
    {
        ResultLog _resultLog = null;

        public ResultLogForm(ResultLog log = null)
        {
            _resultLog = log ?? new ResultLog();
            InitializeComponent();
            UpdateForm();
            DataContext = new ResultLogView(_resultLog);
        }

        public ResultLogForm(ResultLogView resultLogView)
        {
            _resultLog = resultLogView.ResultLog;
            InitializeComponent();
            UpdateForm();
            DataContext = resultLogView;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }
        public ResultLog ResultLog
        {
            get
            {
                return _resultLog;
            }
            set
            {
                if (value != null)
                {
                    _resultLog = value;
                    UpdateForm();
                }
            }
        }

        private void UpdateForm()
        {
            Title = String.Format("{0} ({1} errors, {2} warnings)", _resultLog.Caption, _resultLog.BadCount, _resultLog.SuspectCount);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CanPrint(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = listBoxResultLog.Items.Count > 0;
        }

        private void Print(object sender, ExecutedRoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if(pd.ShowDialog() == true)
            {
                FlowDocument fd = GetFlowDocument();
                fd.ColumnWidth = pd.PrintableAreaWidth;
                DocumentPaginator dp = (fd as IDocumentPaginatorSource).DocumentPaginator;
                PageLayoutPaginator plp = new PageLayoutPaginator(dp, new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight), new Thickness(96));
                plp.Title = Title;
                pd.PrintDocument(plp, Title);
            }
        }

        private FlowDocument GetFlowDocument()
        {
            FlowDocument fd = new FlowDocument();
            fd.FontFamily = new FontFamily("Arial");
            fd.FontSize = 10;
            fd.PagePadding = new Thickness(0);
            Table t = new Table();
            t.CellSpacing = 0;
            t.Margin = new Thickness(0);
            TableColumn tcType = new TableColumn();
            tcType.Width = new GridLength(10.0, GridUnitType.Star);
            TableColumn tcDescription = new TableColumn();
            tcDescription.Width = new GridLength(90.0, GridUnitType.Star);
            t.Columns.Add(tcType);
            t.Columns.Add(tcDescription);
            TableRowGroup trg = new TableRowGroup();
            t.RowGroups.Add(trg);  
            foreach(Result r in listBoxResultLog.Items.OfType<Result>())
            {
                TableRow row = new TableRow();
                AddResultType(row, r);
                AddResultDescription(row, r);
                trg.Rows.Add(row);
            }
            fd.Blocks.Add(t);
            return fd;
        }

        private static void AddResultDescription(TableRow row, Result r)
        {
            Run run = new Run(r.Description);
            run.Foreground = Brushes.Black;
            TableCell cellDescription = new TableCell(new Paragraph(run));
            cellDescription.Padding = new Thickness(5);
            cellDescription.BorderBrush = Brushes.Black;
            cellDescription.BorderThickness = new Thickness(0.25);
            row.Cells.Add(cellDescription);
        }

        private static void AddResultType(TableRow row, Result r)
        {
            TableCell cellType = new TableCell();
            cellType.BorderBrush = Brushes.Black;
            cellType.BorderThickness = new Thickness(0.25);
            cellType.Padding = new Thickness(5);
            string iconPath = ResultConverter.GetIconPath(r.Type);
            Uri uri = new Uri(iconPath);
            BitmapImage bi = new BitmapImage(uri);
            Image icon = new Image();
            icon.Height = 10;
            icon.Width = 10;
            icon.Source = bi;
            BlockUIContainer buic = new BlockUIContainer(icon);
            cellType.Blocks.Add(buic);
            row.Cells.Add(cellType);
        }

        private void CanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = listBoxResultLog.SelectedItems.Count > 0;
        }

        private void Copy(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (Result r in listBoxResultLog.SelectedItems)
                {
                    sb.AppendLine(r.Type.ToString() + "\t" + r.Description);
                }
                Clipboard.SetText(sb.ToString());
            }
            catch(Exception ex)
            {
                Trace.WriteLine("Unable to copy to clipboard: " + ex.Message);
            }
        }

        private void CanSelectAll(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = listBoxResultLog.Items.Count > 0;
        }

        private void SelectAll(object sender, ExecutedRoutedEventArgs e)
        {
            foreach(Result r in listBoxResultLog.Items)
            {
                listBoxResultLog.SelectedItems.Add(r);
            }
        }

        private void Clear_OnClick(object sender, RoutedEventArgs e)
        {
            _resultLog?.Clear();
        }
    }
}
