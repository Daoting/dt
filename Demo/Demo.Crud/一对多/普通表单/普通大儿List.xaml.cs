namespace Demo.Crud;

using A = 大儿X;

public partial class 普通大儿List : List
{
    public 普通大儿List()
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
            _lv.Data = await A.Query($"where parent_id={_parentID}");
        }
        else
        {
            _lv.Data = null;
        }
        Menu["增加"].IsEnabled = _parentID > 0;
    }
}