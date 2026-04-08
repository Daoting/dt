namespace Demo.Crud;

[View("基础")]
public partial class 基础Win : Win
{
    readonly 基础Form _form;

    public 基础Win()
    {
        InitializeComponent();
        _form = new 基础Form { OwnWin = this };
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