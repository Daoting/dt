namespace Demo.Crud;

using A = 角色X;

public sealed partial class 角色Form : Form
{
    public 角色Form()
    {
        InitializeComponent();
        Menu = CreateMenu();
    }

    protected override async Task OnAdd()
    {
        _fv.Data = await A.New();
    }

    protected override async Task OnGet()
    {
        _fv.Data = await A.GetByID(_args.ID);
    }
}