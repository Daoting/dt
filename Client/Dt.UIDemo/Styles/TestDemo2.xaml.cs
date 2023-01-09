#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using Windows.Foundation;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System.Collections.ObjectModel;
#endregion

namespace Dt.Sample
{
    public sealed partial class TestDemo2 : Win
    {
        public ObservableCollection<ListItemData> Items1 { get; set; }
        = new ObservableCollection<ListItemData>();

        public TestDemo2()
        {
            InitializeComponent();

            ListView1.ItemsSource = Items1;
            ListView1.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainPage_Loaded;
            RefreshContainer.RefreshRequested += RefreshContainer_RefreshRequested;

            // Add some initial content to the list.
            await FetchAndInsertItemsAsync(2);
        }

        private void RefreshButtonClick(object sender, RoutedEventArgs e)
        {
            RefreshContainer.RequestRefresh();
        }

        private async void RefreshContainer_RefreshRequested(RefreshContainer sender, RefreshRequestedEventArgs args)
        {
            // Respond to a request by performing a refresh and using the deferral object.
            using (var RefreshCompletionDeferral = args.GetDeferral())
            {
                // Do some async operation to refresh the content

                await FetchAndInsertItemsAsync(3);

                // The 'using' statement ensures the deferral is marked as complete.
                // Otherwise, you'd call
                // RefreshCompletionDeferral.Complete();
                // RefreshCompletionDeferral.Dispose();
            }
        }

        void OnTest(object sender, RoutedEventArgs e)
        {
            //var btn = (BtnItem)XamlReader.Load("<a:BtnItem xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:a=\"using:Dt.Base\" Icon=\"个人信息\" Title=\"个人选项\" Desc=\"包括自定义参数设置，查看修改个人信息及密码等功能。\" />");
            //Grid.SetRow(btn, 1);
            //_grid.Children.Add(btn);
            //Kit.ShowLogin(false);
            RefreshContainer.RequestRefresh();
        }

        private async Task FetchAndInsertItemsAsync(int updateCount)
        {
            for (int i = 0; i < updateCount; ++i)
            {
                // Simulate delay while we go fetch new items.
                await Task.Delay(100);
                Items1.Insert(0, GetNextItem());
            }
        }

        private ListItemData GetNextItem()
        {
            return new ListItemData()
            {
                Header = "Header " + DateTime.Now.Second.ToString(),
                Date = DateTime.Now.ToLongDateString(),
                Body = DateTime.Now.ToLongTimeString()
            };
        }
    }

    public class ListItemData
    {
        public string Header { get; set; }
        public string Date { get; set; }
        public string Body { get; set; }
    }
}
