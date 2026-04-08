namespace Demo.Crud;

using A = 小儿X;

public sealed partial class 普通小儿Form : Form
{
    public 普通小儿Form()
    {
        InitializeComponent();
        Menu = CreateMenu();
    }

    protected override async Task OnAdd()
    {
        _fv.Data = await A.New(GroupID: _args.ParentID.Value);
    }

    protected override async Task OnGet()
    {
        _fv.Data = await A.GetByID(_args.ID);
    }
}