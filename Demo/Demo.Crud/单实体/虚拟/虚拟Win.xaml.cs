namespace Demo.Crud;

[View("虚拟")]
public partial class 虚拟Win : Win
{
    readonly 虚拟Form _form;

    public 虚拟Win()
    {
        InitializeComponent();
        _form = new 虚拟Form { OwnWin = this };
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