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
using System.Linq.Dynamic.Core;
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

        void OnWhere1(object sender, RoutedEventArgs e)
        {
            SetWhere("Xm.StartsWith('李')");
        }

        void OnWhere2(object sender, RoutedEventArgs e)
        {
            SetWhere("Xm.Contains('涛') && Bh < 15");
        }

        void OnWhere3(object sender, RoutedEventArgs e)
        {
            SetWhere("it.Str(\"Xm\") == \"李全亮\"");
        }

        void OnWhere4(object sender, RoutedEventArgs e)
        {
            SetWhere("it.Str(\"Xm\").StartsWith('李')");
        }

        void OnMyWhere(object sender, RoutedEventArgs e)
        {
            SetWhere(_tb.Text.Trim());
        }

        void OnCombin(object sender, RoutedEventArgs e)
        {
            SetFilter(() =>
            {
                _lv.Where = "Xm.StartsWith('李')";
                _lv.Filter = FilterCallback;
            });
        }

        void OnFilter(object sender, RoutedEventArgs e)
        {
            SetFilter(() => _lv.Filter = FilterCallback);
        }

        void OnFilterCfg(object sender, RoutedEventArgs e)
        {
            _lv.FilterCfg = new FilterCfg();
        }

        void OnCustFilterCfg(object sender, RoutedEventArgs e)
        {
            _lv.FilterCfg = new FilterCfg
            {
                FilterCols = "xm,bh",
                EnablePinYin = true,
                IsRealtime = true,
            };
        }

        void OnMyFilterCfg(object sender, RoutedEventArgs e)
        {
            var cfg = new FilterCfg();
            cfg.MyFilter = (s) =>
            {
                _lv.Data = SampleData.CreatePersonXs(new Random().Next(30));
            };
            _lv.FilterCfg = cfg;
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

        void SetWhere(string p_where)
        {
            using (_lv.Defer())
            {
                _lv.Filter = null;
                _lv.FilterCfg = null;
                _lv.Where = p_where;
            }
        }

        void SetFilter(Action p_call)
        {
            using (_lv.Defer())
            {
                _lv.Where = null;
                _lv.Filter = null;
                _lv.FilterCfg = null;
                p_call();
            }
        }

        bool FilterCallback(object obj)
        {
            var per = obj as PersonX;
            return per.Bumen == "肾内科二";
        }
    }
}