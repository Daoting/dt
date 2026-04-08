namespace Demo.Crud;

using A = 角色X;

public partial class 角色List : List
{
    public 角色List()
    {
        InitializeComponent();
        Menu = CreateMenu();
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