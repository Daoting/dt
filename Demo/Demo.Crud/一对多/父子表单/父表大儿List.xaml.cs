namespace Demo.Crud;

using A = 大儿X;

public partial class 父表大儿List : List
{
    public 父表大儿List()
    {
        InitializeComponent();
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
    }
}