namespace Demo.Crud;

using A = 小儿X;

public partial class 父表小儿List : List
{
    public 父表小儿List()
    {
        InitializeComponent();
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
    }
}