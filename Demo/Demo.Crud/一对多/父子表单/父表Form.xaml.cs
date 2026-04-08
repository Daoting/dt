namespace Demo.Crud;

using A = 父表X;

public sealed partial class 父表Form : Form
{
    public 父表Form()
    {
        InitializeComponent();
        ShowVeil = true;
        Menu = CreateMenu();
    }

    protected override async Task OnAdd()
    {
        _fv.Data = await A.New();
    }

    protected override async Task OnGet()
    {
        _fv.Data = await A.GetByID(_args.ID);
        _lv大儿.Data = await 大儿X.Query($"where parent_id={_args.ID}");
        _lv小儿.Data = await 小儿X.Query($"where group_id={_args.ID}");
    }

    protected override async Task OnAddChild(Fv p_fv)
    {
        if (p_fv == _fv大儿)
        {
            p_fv.Data = await 大儿X.New(ParentID: _fv.Row.ID);
        }
        else if (p_fv == _fv小儿)
        {
            p_fv.Data = await 小儿X.New(GroupID: _fv.Row.ID);
        }
    }
}