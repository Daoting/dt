#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    public sealed partial class 部门4人员 : Dlg
    {
        public 部门4人员()
        {
            InitializeComponent();
            Menu = Menu.New(Mi.确定(OnOK));
        }
        
        public Row SelectedRow => _tv.SelectedRow;

        public async Task<bool> Show(long p_releatedID, FrameworkElement p_target)
        {
            _tv.Data = await 部门X.View1.Query($"where not exists ( select 部门id from 部门人员 b where a.ID = b.部门id and 人员id={p_releatedID} )");
            if (!Kit.IsPhoneUI)
            {
                WinPlacement = DlgPlacement.TargetBottomLeft;
                PlacementTarget = p_target;
                ClipElement = p_target;
                Height = Kit.ViewHeight / 2;
                Width = Kit.ViewWidth / 4;
            }
            return await ShowAsync();
        }
    }
}
