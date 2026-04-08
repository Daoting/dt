namespace Demo.Crud;

using A = 用户X;

public partial class 用户List : List
{
    public 用户List()
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