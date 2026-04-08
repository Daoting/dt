namespace Demo.Crud;

using A = 基础X;

[Share("Crud基础Form")]
public sealed partial class 基础Form : Form
{
    public 基础Form()
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