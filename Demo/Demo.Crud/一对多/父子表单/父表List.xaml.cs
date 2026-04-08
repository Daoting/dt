namespace Demo.Crud;

using A = 父表X;

public partial class 父表List : List
{
    public 父表List()
    {
        InitializeComponent();
        Menu = CreateMenu(del: false);
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