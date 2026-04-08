namespace Demo.Crud;

[View("父表")]
public partial class 父表Win : Win
{
    readonly 父表Form _parentForm;

    public 父表Win()
    {
        InitializeComponent();
        _parentForm = new 父表Form { OwnWin = this };
        Attach();
    }
        
    void Attach()
    {
        _query.Query += e =>
        {
            _parentList.Query(e);
            NaviTo(_parentList.Title);
        };
            
        _parentList.Msg += e => _ = _parentForm.Query(e);
        _parentList.Navi += () => NaviTo(_大儿List.Title + "," + _小儿List.Title);

        _parentForm.UpdateList += e => _ = _parentList.Refresh(e.ID);
        _parentForm.UpdateRelated += e => 
        {
            _大儿List.Query(e.ID);
            _小儿List.Query(e.ID);
            };
    }
}