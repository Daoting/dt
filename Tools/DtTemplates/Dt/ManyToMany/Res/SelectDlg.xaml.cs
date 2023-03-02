#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace $rootnamespace$
{
    public sealed partial class $selectdlg$ : Dlg
    {
        public $selectdlg$()
        {
            InitializeComponent();
        }
        
        public IEnumerable<Row> SelectedRows => _lv.SelectedRows;

        public async Task<bool> Show(long p_releatedID, FrameworkElement p_target)
        {
            _lv.Data = await $entity$.Query("$notexistclause$", new { ReleatedID = p_releatedID });
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
