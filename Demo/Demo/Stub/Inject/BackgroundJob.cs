#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-07-28 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo
{
    class BackgroundJob : IBackgroundJob
    {
        /// <summary>
        /// 后台任务处理，除 BgJobKit类、本地库(如AtState CookieX) 外，不可使用任何UI和外部变量，保证可独立运行！！！
        /// </summary>
        public async Task Run()
        {
            string phone = await CookieX.Get("LoginID");
            BgJobKit.Toast(
                "后台任务样例",
                $"用户标识：{phone}",
                new AutoStartInfo { WinType = typeof(UI.LvHome).AssemblyQualifiedName, Title = "列表" });
            BgJobKit.Log("后台运行中");
        }
    }
}
