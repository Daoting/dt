namespace Demo.Crud;

using A = VirX<主表X, 扩展1X, 扩展2X>;

public sealed partial class 虚拟Form : Form
{
    public 虚拟Form()
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