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
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class RptDesignHome : Win
    {
        RptDesignInfo _info;
        RptDesignWin _design;
        TextBox _tbXaml;

        public RptDesignHome(RptDesignInfo p_info)
        {
            InitializeComponent();
            _info = p_info;
            _design = new RptDesignWin(_info);
            LoadCenter(_design);
        }

        void OnEditTemplate(object sender, RoutedEventArgs e)
        {
            LoadCenter(_design);
        }

        void OnExport(object sender, RoutedEventArgs e)
        {
            if (_tbXaml == null)
            {
                _tbXaml = new TextBox { AcceptsReturn = true, BorderThickness = new Thickness(0) };
                ScrollViewer.SetHorizontalScrollBarVisibility(_tbXaml, ScrollBarVisibility.Auto);
                ScrollViewer.SetVerticalScrollBarVisibility(_tbXaml, ScrollBarVisibility.Auto);
            }
                
            _tbXaml.Text = AtRpt.SerializeTemplate(_info.Root);
            LoadCenter(_tbXaml);
        }

        void OnImport(object sender, RoutedEventArgs e)
        {
            AtRpt.Show(new List<RptInfo> { new RptInfo { Name = "a" }, new RptInfo { Name = "b" } });
        }

        void OnPreview(object sender, RoutedEventArgs e)
        {
            // 比较窗口类型和初始参数，关闭旧窗口
            var info = new RptInfo { Name = _info.Name, Root = _info.Root };
            Win win;
            if (!AtSys.IsPhoneUI
                && (win = Desktop.Inst.ActiveWin(typeof(RptViewWin), info)) != null)
            {
                win.Close();
            }

            AtRpt.Show(info);
        }
    }
}
