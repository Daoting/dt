namespace Demo.Crud;

using A = 基础X;

public sealed partial class 树形Form : Form
{
    public 树形Form()
    {
        InitializeComponent();
        Menu = CreateMenu();
    }
        
    protected override async Task OnAdd()
    {
        _fv.Data = await A.New(ParentID: _args.ParentID);
    }

    protected override async Task OnGet()
    {
        _fv.Data = await A.GetByID(_args.ID);
    }
}