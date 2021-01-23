using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using OSEUI.WPF.Resources;

namespace OSEUI.WPF.Printing
{
    public class PageLayoutPaginator : DocumentPaginator
    {
        Size _pageSize;
        Thickness _margin;
        DocumentPaginator _paginator;
        Rect _contentBox;
        Rect _headerBox;
        Rect _footerBox;
        Rect _pageBox;
        int _pageNumber = 0;
        bool _hasHeader = true;
        bool _hasFooter = true;
        public string Title { get; set; }
        public Thickness Margin
        {
            get
            {
                return _margin;
            }
        }
        public Rect ContentBox
        {
            get
            {
                return _contentBox;
            }
        }
        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }
        }
        public DateTime PrintedDate
        {
            get
            {
                return DateTime.Now;
            }
        }
        public string PageNumberText
        {
            get
            {
                return "Page " + PageNumber + " of " + PageCount;
            }
        }
        public string PrintedDateText
        {
            get
            {
                return "Printed " + PrintedDate.ToString();
            }
        }
        public PageLayoutPaginator(DocumentPaginator paginator, Size pageSize, Thickness margin)
        {
            _pageSize = pageSize;
            _margin = margin;
            _paginator = paginator;
            _pageBox = new Rect(0, 0, pageSize.Width, pageSize.Height);
            _contentBox = new Rect(margin.Left, margin.Top, pageSize.Width - margin.Left - margin.Right, PageSize.Height - margin.Top - margin.Bottom);
            _headerBox = new Rect(margin.Left, 0.0, pageSize.Width - margin.Left - margin.Right, margin.Top);
            _footerBox = new Rect(margin.Left, pageSize.Height - margin.Bottom, pageSize.Width - margin.Left - margin.Right, margin.Bottom);
            _paginator.PageSize = new Size(_pageSize.Width - margin.Left - margin.Right,
                                            _pageSize.Height - margin.Top - margin.Bottom);
            _paginator.ComputePageCount();
        }
        public override DocumentPage GetPage(int pageNumber)
        {
            _pageNumber = pageNumber + 1;
            DocumentPage page = _paginator.GetPage(pageNumber);
            ContainerVisual printedPage = new ContainerVisual();
            ContainerVisual pageContainer = new ContainerVisual();
            pageContainer.Children.Add(page.Visual);
            pageContainer.Transform = new TranslateTransform(_margin.Left, _margin.Top);
            printedPage.Children.Add(pageContainer);
            AddHeader(printedPage);
            AddFooter(printedPage);
            return new DocumentPage(printedPage, _pageSize, _pageBox, _contentBox);
        }

        private void AddFooter(ContainerVisual printedPage)
        {
            if(HasFooter)
            {
                DataTemplate dtFooter = OSEUIResources.Resources["DocumentFooter"] as DataTemplate;
                FrameworkElement ffe = dtFooter.LoadContent() as FrameworkElement;
                ffe.DataContext = this;
                ffe.Measure(_footerBox.Size);
                ffe.Arrange(_footerBox);
                ffe.UpdateLayout();
                ContainerVisual footerContainer = new ContainerVisual();
                footerContainer.Offset = new Vector(_footerBox.Left, _footerBox.Top);
                footerContainer.Children.Add(ffe);
                printedPage.Children.Add(footerContainer);
            }
        }

        private void AddHeader(ContainerVisual printedPage)
        {
            if(HasHeader)
            {
                DataTemplate dtHeader = OSEUIResources.Resources["DocumentHeader"] as DataTemplate;
                FrameworkElement hfe = dtHeader.LoadContent() as FrameworkElement;
                hfe.DataContext = this;
                hfe.Measure(_headerBox.Size);
                hfe.Arrange(_headerBox);
                hfe.UpdateLayout();
                ContainerVisual headerContainer = new ContainerVisual();
                headerContainer.Offset = new Vector(_headerBox.Left, _headerBox.Top);
                headerContainer.Children.Add(hfe);
                printedPage.Children.Add(headerContainer);
            }
        }

        public override bool IsPageCountValid
        {
            get
            {
                return _paginator.IsPageCountValid;
            }
        }

        public override int PageCount
        {
            get
            {
                return _paginator.PageCount;
            }
        }
        public override Size PageSize
        {
            get
            {
                return _paginator.PageSize;
            }
            set
            {
                _paginator.PageSize = value;
            }
        }

        public override IDocumentPaginatorSource Source
        {
            get
            {
                return _paginator.Source;
            }
        }

        public bool HasHeader
        {
            get
            {
                return _hasHeader;
            }

            set
            {
                _hasHeader = value;
            }
        }

        public bool HasFooter
        {
            get
            {
                return _hasFooter;
            }

            set
            {
                _hasFooter = value;
            }
        }
    }
}
