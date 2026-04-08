namespace Demo.Crud;

public sealed partial class 用户Query : Tab
{
    public 用户Query()
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
        row.Add<string>("手机号");
        row.Add<string>("姓名");
        row.Add<string>("密码");
        _fv.Data = row;
    }
}
