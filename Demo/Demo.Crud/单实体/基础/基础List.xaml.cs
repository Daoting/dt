namespace Demo.Crud;

using A = 基础X;

[Share]
public partial class 基础List : List
{
    public 基础List()
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