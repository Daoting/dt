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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.UIDemo
{
    public partial class LvFilter : Win
    {
        public LvFilter()
        {
            InitializeComponent();
            _lv.Data = SampleData.CreatePersonXs(100);
        }

        void OnWhere(object sender, RoutedEventArgs e)
        {
            _lv.Where = "it.Xm.Contains('李')";
        }

        void OnFilter(object sender, RoutedEventArgs e)
        {
            _lv.Filter = FilterCallback;
        }

        void OnFilterCfg(object sender, RoutedEventArgs e)
        {
            _lv.FilterCfg = new FilterCfg();
        }

        void OnClearFilter(object sender, RoutedEventArgs e)
        {
            using (_lv.Defer())
            {
                _lv.Where = null;
                _lv.Filter = null;
                _lv.FilterCfg = null;
            }
        }

        void OnMyWhere(object sender, RoutedEventArgs e)
        {
            _lv.Where = _tb.Text.Trim();
        }

        bool FilterCallback(object obj)
        {
            var per = obj as PersonX;
            return per.Bumen == "肾内科二";
        }
    }
}