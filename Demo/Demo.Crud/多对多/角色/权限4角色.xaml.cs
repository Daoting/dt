using Microsoft.UI.Xaml;

namespace Demo.Crud;

public sealed partial class 权限4角色 : Dlg
{
    public 权限4角色()
    {
        InitializeComponent();
        Menu = Menu.New(Mi.确定(OnOK));
    }
        
    public IEnumerable<Row> SelectedRows => _lv.SelectedRows;

    public async Task<bool> Show(long p_releatedID, FrameworkElement p_target)
    {
        _lv.Data = await 权限X.Query($"where not exists ( select prv_id from crud_角色权限 b where a.ID = b.prv_id and role_id={p_releatedID} )");
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
