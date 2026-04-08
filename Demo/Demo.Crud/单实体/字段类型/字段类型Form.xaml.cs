namespace Demo.Crud;

using A = 字段类型X;

public sealed partial class 字段类型Form : Form
{
    public 字段类型Form()
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