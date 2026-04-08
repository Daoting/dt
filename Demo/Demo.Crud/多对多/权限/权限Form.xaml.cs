namespace Demo.Crud;

using A = 权限X;

public sealed partial class 权限Form : Form
{
    public 权限Form()
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