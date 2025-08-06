#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-07-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr.Rbac;
#endregion

namespace Demo
{
    class BackgroundJob : IBackgroundJob
    {
        /// <summary>
        /// 后台任务进程外运行，除 At BgJobKit AtState CookieX 类外，不可使用任何UI或外部变量，保证可独立运行！！！
        /// </summary>
        public async Task Run()
        {
            BgJobKit.Log("后台运行中");

            string last = await CookieX.Get("LastNotify");
            int now = Environment.TickCount;
            if (last != null
                && int.TryParse(last, out int lastTime)
                && now - lastTime < 1440000)
            {
                // 24小时通知一次
                return;
            }
            await CookieX.Save("LastNotify", now.ToString());

            string msg = null;
            string id = await CookieX.Get("LoginID");
            if (id != "")
            {
                var user = await At.First<UserX>($"select * from cm_user where id={id}");
                if (user != null)
                {
                    msg += $"用户标识：{user.ID}\n" +
                       $"账号：{user.Acc}\n" +
                       $"姓名：{user.Name}\n" +
                       $"手机号：{user.Phone}\n";
                }
            }
            if (msg == null)
            {
                msg += "用户未登录\n";
            }

            BgJobKit.Toast(
                "后台样例 > 带启动参数",
                msg,
                new AutoStartInfo { WinType = typeof(UI.LvHome).AssemblyQualifiedName, Title = "列表" });
        }
    }
}
