namespace Demo.Crud;

using A = 基础X;

public partial class 树形Tree : Tree
{
    public 树形Tree()
    {
        InitializeComponent();
        Menu = CreateMenu();
        _tv.FilterCfg = new FilterCfg
        {
            FilterCols = "name",
            EnablePinYin = true,
            Placeholder = "文字或拼音简码",
            IsRealtime = true,
        };
    }
        
    public void SelectByID(long p_id)
    {
        _tv.SelectByID(p_id);
    }
        
    protected override void OnFirstLoaded()
    {
        _tv.FixedRoot = new A(ID: 0);
        _ = Refresh();
    }

    protected override async Task OnQuery()
    {
        _tv.Data = await A.Query("where parent_id is null");
    }
}