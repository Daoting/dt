#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
#endregion

namespace Dt.Core
{
    
    [View("主页")]
    public class MainPage
    {

    }

    [View(LobViews.登录页)]
    public class LoginPage
    {

    }

    public enum LobViews
    {
        主页,
        登录页,
        通讯录,

    }
}
