#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.UIDemo
{
    public partial class TvDynamicLoading : Win
    {
        public TvDynamicLoading()
        {
            InitializeComponent();
            _tv.Data = TvData.GetRootTbl();
            _tv.LoadingChild += OnLoadingChild;
        }

        async void OnLoadingChild(LoadingChildArgs e)
        {
            using (e.Wait())
            {
                // 模拟等待
                await Task.Delay(400);
                e.Children = ((ITreeData)TvData.GetTbl()).GetTreeItemChildren(e.CurrentItem.Data);
            }
        }

        void OnCollapseAll(object sender, RoutedEventArgs e)
        {
            _tv.CollapseAll();
        }
    }
}