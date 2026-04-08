namespace Demo.Crud;

public sealed partial class 父表Query : Tab
{
    public 父表Query()
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
        row.Add<string>("父名");
        _fv.Data = row;
    }
}
