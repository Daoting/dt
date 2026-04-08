namespace Demo.Crud;

using A = 用户X;

public sealed partial class 用户Form : Form
{
    public 用户Form()
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