#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-12-03
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
#endregion

namespace Demo
{
    public partial class App : AppBase
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Stub NewStub() => new AppStub();
    }
}