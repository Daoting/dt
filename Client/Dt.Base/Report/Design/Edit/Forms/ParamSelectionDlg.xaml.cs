#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class ParamSelectionDlg : Dlg
    {
        public ParamSelectionDlg()
        {
            InitializeComponent();
        }

        internal async Task<bool> Show(FrameworkElement p_target, RptText p_item)
        {
            _lv.Data = p_item.Root.Params.Data;
            if (!Kit.IsPhoneUI)
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
            return $"Param({_lv.SelectedRow.Str("name")})";
        }

        void OnSave(object sender, Mi e)
        {
            if (_lv.SelectedItem == null)
            {
                Kit.Warn("请选择参数名！");
            }
            else
            {
                Close(true);
            }
        }

        void OnDoubleClick(object sender, object e)
        {
            OnSave(null, null);
        }
    }
}
