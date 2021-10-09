#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 后台任务处理基类，除 AtState、Stub、UnaryRpc、Kit.Toast 外，不可使用任何UI和外部变量，保证可独立运行！！！
    /// </summary>
    public abstract class BgTask
    {
        /// <summary>
        /// 后台任务入口
        /// </summary>
        public abstract Task Run();

        /// <summary>
        /// 存根
        /// </summary>
        public IStub Stub => Kit.Stub;

        /// <summary>
        /// 因后台独立运行，涉及验证身份的API，先确保已登录
        /// </summary>
        /// <returns></returns>
        protected async Task<bool> Login()
        {
            if (Kit.IsLogon)
            {
                //Kit.Toast("后台", "已登录");
                return true;
            }

            string phone = AtState.GetCookie("LoginPhone");
            string pwd = AtState.GetCookie("LoginPwd");
            if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(pwd))
            {
                // 自动登录
                var result = await new UnaryRpc(
                    "cm",
                    "Entry.LoginByPwd",
                    phone,
                    pwd
                ).Call<LoginResult>();

                // 登录成功
                if (result.IsSuc)
                {
                    //Kit.Toast("后台", "登录成功");
                    Kit.InitUser(result);
                    return true;
                }
            }
            return false;
        }
    }
}