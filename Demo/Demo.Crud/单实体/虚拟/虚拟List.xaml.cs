namespace Demo.Crud;

using A = VirX<主表X, 扩展1X, 扩展2X>;

public partial class 虚拟List : List
{
    public 虚拟List()
    {
        InitializeComponent();
        Menu = CreateMenu();
        _lv.AddMultiSelMenu(Menu);
        _lv.SetMenu(CreateContextMenu());
    }

    protected override async Task OnQuery()
    {
        if (_clause == null)
        {
            _lv.Data = await A.Query(null);
        }
        else
        {
            var par = await _clause.Build<A>();
            _lv.Data = await A.Query(par.Sql, par.Params);
        }
    }
}