namespace Demo.Crud;

[View("字段类型")]
public partial class 字段类型Win : Win
{
    readonly 字段类型Form _form;

    public 字段类型Win()
    {
        InitializeComponent();
        _form = new 字段类型Form { OwnWin = this };
        Attach();
    }

    void Attach()
    {
        _query.Query += e =>
        {
            _list.Query(e);
            NaviTo(_list.Title);
        };
        _list.Msg += e => _ = _form.Query(e);
        _form.UpdateList += e => _ = _list.Refresh(e.ID);
    }
}