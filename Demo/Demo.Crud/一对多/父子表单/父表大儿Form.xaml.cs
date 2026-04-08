namespace Demo.Crud;

using A = 大儿X;

public sealed partial class 父表大儿Form : Form
{
    public 父表大儿Form()
    {
        InitializeComponent();
        Menu = CreateMenu();
    }

    protected override async Task OnAdd()
    {
        _fv.Data = await A.New(ParentID: _args.ParentID.Value);
    }

    protected override async Task OnGet()
    {
        _fv.Data = await A.GetByID(_args.ID);
    }
}