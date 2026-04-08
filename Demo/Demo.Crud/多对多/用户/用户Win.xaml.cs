namespace Demo.Crud;

[View("用户")]
public partial class 用户Win : Win
{
    readonly 用户Form _mainForm;

    public 用户Win()
    {
        InitializeComponent();
        _mainForm = new 用户Form { OwnWin = this };
        Attach();
    }

    void Attach()
    {
        _query.Query += e =>
        {
            _mainList.Query(e);
            NaviTo(_mainList.Title);
        };

        _mainList.Msg += e => _ = _mainForm.Query(e);
        _mainList.Navi += () => NaviTo(_角色List.Title);

        _mainForm.UpdateList += e => _ = _mainList.Refresh(e.ID);
        _mainForm.UpdateRelated += e => _角色List.Query(e.ID);
    }
}