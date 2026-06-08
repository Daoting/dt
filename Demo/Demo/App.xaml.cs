namespace Demo;

public partial class App : AppBase
{
    public App()
    {
        InitializeComponent();
        Init();
    }

    protected override Stub NewStub() => new AppStub();
}