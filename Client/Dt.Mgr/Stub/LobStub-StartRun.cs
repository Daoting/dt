#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Mgr
{
    public partial class LobStub
    {
        /// <summary>
        /// 按默认流程开始运行
        /// <para>1. 已登录过，先自动登录</para>
        /// <para>2. 未登录或登录失败时，根据 p_loginFirst 显示登录页或主页</para>
        /// </summary>
        /// <param name="p_loginFirst">是否强制先登录，默认true</param>
        /// <returns></returns>
        public async Task StartRun(bool p_loginFirst = true)
        {
            string phone = AtState.GetCookie("LoginPhone");
            string pwd = AtState.GetCookie("LoginPwd");
            if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(pwd))
            {
                // 自动登录
                var result = await AtCm.LoginByPwd<LoginResult>(phone, pwd);

                // 登录成功
                if (result.IsSuc)
                {
                    Lob.InitUser(result);
                    // 切换到主页
                    Lob.ShowHome();
                    // 接收服务器推送
                    PushHandler.Register();
                    return;
                }
            }

            // 未登录或登录失败
            if (p_loginFirst)
            {
                // 强制先登录
                Lob.ShowLogin(false);
            }
            else
            {
                // 未登录先显示主页
                Lob.ShowHome();
            }
        }
    }
}