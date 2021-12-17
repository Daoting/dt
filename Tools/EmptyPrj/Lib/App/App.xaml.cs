#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-12-03
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Lib
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