
// 模拟命令行输入参数，支持绝对路径和相对路径
Launcher.Run(
    new string[]
    {
        "path=D:\\Dt\\Master\\Service\\src\\Cos\\bin\\Debug\\net10.0",
        "proxy=nginx"
    },
    new Dictionary<string, Stub>() { { "cm", new Dt.Cm.SvcStub() } }
    );