namespace Demo.Crud;

using A = 小儿X;

public partial class 普通小儿List : List
{
    public 普通小儿List()
    {
        InitializeComponent();
        Menu = CreateMenu();
        _lv.AddMultiSelMenu(Menu);
        _lv.SetMenu(CreateContextMenu());
    }
        
    protected override async Task OnQuery()
    {
        if (_parentID > 0)
        {
            _lv.Data = await A.Query($"where group_id={_parentID}");
        }
        else
        {
            _lv.Data = null;
        }
        Menu["增加"].IsEnabled = _parentID > 0;
    }
}