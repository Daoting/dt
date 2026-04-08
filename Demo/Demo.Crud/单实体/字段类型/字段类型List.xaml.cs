namespace Demo.Crud;

using A = 字段类型X;

public partial class 字段类型List : List
{
    public 字段类型List()
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