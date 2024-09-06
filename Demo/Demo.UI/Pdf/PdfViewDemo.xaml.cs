#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Charts;
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Windows.Storage;
#endregion

namespace Demo.UI
{
    public partial class PdfViewDemo : Win
    {
        public PdfViewDemo()
        {
            InitializeComponent();
            LoadPdf();
        }

        async void LoadPdf()
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Demo.UI/Files/Content/dt.pdf"));
            _pdf.Open(file);
        }

        void OnJsOpen(object sender, RoutedEventArgs e)
        {
            _pdf.Open();
        }

        void OnPrint(object sender, RoutedEventArgs e)
        {
            _pdf.Print();
        }

        void OnClear(object sender, RoutedEventArgs e)
        {
            _pdf.Clear();
        }
        
        async void OnOpenFile(object sender, RoutedEventArgs e)
        {
            var picker = Kit.GetFileOpenPicker();
            picker.FileTypeFilter.Add(".pdf");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                _pdf.Open(file);
            }
        }

        void OnDownload(object sender, RoutedEventArgs e)
        {
            _pdf.Download();
        }

        void OnFirstPage(object sender, RoutedEventArgs e)
        {
            _pdf.FirstPage();
        }

        void OnLastPage(object sender, RoutedEventArgs e)
        {
            _pdf.LastPage();
        }

        void OnNextPage(object sender, RoutedEventArgs e)
        {
            _pdf.NextPage();
        }

        void OnPrePage(object sender, RoutedEventArgs e)
        {
            _pdf.PreviousPage();
        }

        void OnGotoPage(object sender, RoutedEventArgs e)
        {
            _pdf.GotoPage(2);
        }
    }
}