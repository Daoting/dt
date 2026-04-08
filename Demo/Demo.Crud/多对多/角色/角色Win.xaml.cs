namespace Demo.Crud;

[View("角色")]
public partial class 角色Win : Win
{
    readonly 角色Form _mainForm;

    public 角色Win()
    {
        InitializeComponent();
        _mainForm = new 角色Form { OwnWin = this };
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
        _mainList.Navi += () => NaviTo(_用户List.Title + "," + _权限List.Title);

        _mainForm.UpdateList += e => _ = _mainList.Refresh(e.ID);
        _mainForm.UpdateRelated += e => 
        {
            _用户List.Query(e.ID);
            _权限List.Query(e.ID);
        };
    }
}