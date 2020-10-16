#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class GlobalParamDlg : Dlg
    {
        public GlobalParamDlg()
        {
            InitializeComponent();
            LoadItems();
        }

        internal async Task<bool> Show(FrameworkElement p_target)
        {
            if (!AtSys.IsPhoneUI)
            {
                WinPlacement = DlgPlacement.TargetOuterLeftTop;
                PlacementTarget = p_target;
                ClipElement = p_target;
                Height = 400;
                Width = 300;
            }
            return await ShowAsync();
        }

        public string GetExpression()
        {
            return $"Global({_lv.SelectedRow.Str("id")})";
        }

        void OnSave(object sender, Mi e)
        {
            if (_lv.SelectedItem == null)
            {
                AtKit.Warn("请选择变量名！");
            }
            else
            {
                Close(true);
            }
        }

        void LoadItems()
        {
            Table tbl = new Table { { "id" } };
            tbl.AddRow(new { id = "页号" });
            tbl.AddRow(new { id = "总页数" });
            tbl.AddRow(new { id = "水平页号" });
            tbl.AddRow(new { id = "垂直页号" });
            tbl.AddRow(new { id = "报表名称" });
            tbl.AddRow(new { id = "日期" });
            tbl.AddRow(new { id = "时间" });
            tbl.AddRow(new { id = "日期时间" });
            _lv.Data = tbl;
        }

        void OnDoubleClick(object sender, object e)
        {
            OnSave(null, null);
        }
    }
}
