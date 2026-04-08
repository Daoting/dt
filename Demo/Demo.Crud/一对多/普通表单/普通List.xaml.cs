namespace Demo.Crud;

using A = 父表X;

public partial class 普通List : List
{
    public 普通List()
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