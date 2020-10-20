#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public sealed partial class RptDemo : Win
    {
        public RptDemo()
        {
            InitializeComponent();

            Row row = new Row();
            //row.AddCell("type", "打开模板编辑窗口");
            row.AddCell("type", "使用RptView预览");
            _fv.Data = row;
            AttachEvent();
        }

        void AttachEvent()
        {
            foreach (var item in _fv.Items)
            {
                if (item is Panel pnl)
                {
                    foreach (Button btn in pnl.Children.OfType<Button>())
                    {
                        btn.Click += OnBtnClick;
                    }
                }
            }
        }

        void OnBtnClick(object sender, RoutedEventArgs e)
        {
            string name = ((Button)sender).Content.ToString();
            switch (_fv.Row.Str("type"))
            {
                case "使用RptView预览":
                    _rptView.LoadReport(new MyRptInfo { Name = name });
                    break;
                case "打开报表预览窗口":
                    AtRpt.Show(new MyRptInfo { Name = name });
                    break;
                case "打开模板编辑窗口":
                    _ = AtRpt.ShowDesign(new MyRptDesignInfo { Name = name });
                    break;
            }
        }

        void OnScript(object sender, RoutedEventArgs e)
        {
            string name = ((Button)sender).Content.ToString();
            switch (_fv.Row.Str("type"))
            {
                case "使用RptView预览":
                    _rptView.LoadReport(new MyRptInfo { Name = name, Params = new Dict { { "parentid", "" }, { "parentname", "根菜单" } } });
                    break;
                case "打开报表预览窗口":
                    AtRpt.Show(new MyRptInfo { Name = name, Params = new Dict { { "parentid", "" }, { "parentname", "根菜单" } } });
                    break;
                case "打开模板编辑窗口":
                    _ = AtRpt.ShowDesign(new MyRptDesignInfo { Name = name });
                    break;
            }
        }

        void OnRptGroup(object sender, RoutedEventArgs e)
        {
            List<RptInfo> ls = new List<RptInfo>
            {
                new MyRptInfo { Name = "默认查询面板" },
                new MyRptInfo { Name = "自定义查询面板" }
            };
            AtRpt.Show(ls, "报表组");
        }
    }
}
