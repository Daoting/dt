namespace Demo.Crud;

public sealed partial class 权限Query : Tab
{
    public 权限Query()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 查询事件
    /// </summary>
    public event Action<QueryClause> Query
    {
        add { _fv.Query += value; }
        remove { _fv.Query -= value; }
    }

    protected override void OnFirstLoaded()
    {
        var row = new Row();
        row.Add<string>("权限名称");
        _fv.Data = row;
    }
}
