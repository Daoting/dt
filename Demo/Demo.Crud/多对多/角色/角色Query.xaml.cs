namespace Demo.Crud;

public sealed partial class 角色Query : Tab
{
    public 角色Query()
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
        row.Add<string>("角色名称");
        row.Add<string>("角色描述");
        _fv.Data = row;
    }
}
