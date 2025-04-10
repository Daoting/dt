#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Base.Views
{
    public sealed partial class AddRelationDlg : Dlg
    {
        Lv _lv;
        
        public IEnumerable<Row> SelectedRows => _lv.SelectedRows;

        public async Task<bool> Show(RelatedEntityCfg p_cfg, long p_releatedID, FrameworkElement p_target)
        {
            Title = p_cfg.SelectDlgTitle;
            
            _lv = RelatedEntityList.CreateLv(p_cfg);
            _lv.SelectionMode = SelectionMode.Multiple;
            Content = _lv;
            Menu = Menu.New(Mi.确定(OnOK));

            _lv.Data = await p_cfg.QueryUnrelated(p_releatedID);
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
