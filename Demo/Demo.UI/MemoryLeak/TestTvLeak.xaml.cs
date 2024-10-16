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
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.UI
{
    public partial class TestTvLeak : Win
    {
        public TestTvLeak()
        {
            InitializeComponent();
            OnLoadTbl(null, null);
        }

        void OnLoadTbl(object sender, RoutedEventArgs e)
        {
            _tv.Data = TvData.GetTbl();
        }

        void OnLoadData(object sender, RoutedEventArgs e)
        {
            _tv.Data = TvData.GetTreeData();
        }
        
        void OnFilter(object sender, RoutedEventArgs e)
        {
            _tv.FilterCfg = new FilterCfg();
        }

        void OnCustFilter(object sender, RoutedEventArgs e)
        {
            _tv.FilterCfg = new FilterCfg
            {
                FilterCols = "name",
                EnablePinYin = true,
                IsRealtime = true,
            };
        }

        void OnMyFilter(object sender, RoutedEventArgs e)
        {
            var cfg = new FilterCfg();
            cfg.MyFilter = (o, txt) =>
            {
                return true;
            };
            _tv.FilterCfg = cfg;
        }

        void OnDelFilter(object sender, RoutedEventArgs e)
        {
            _tv.FilterCfg = null;
        }
    }
}