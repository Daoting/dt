#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace $ext_safeprojectname$
{
    public partial class App : BaseApp
    {
        public App()
        {
            InitializeComponent();
        }

        public override Type Stub => typeof(AppStub);
    }
}