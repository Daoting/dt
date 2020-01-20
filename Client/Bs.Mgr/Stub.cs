#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Dt.App;
using Dt.App.Home;
using Dt.App.Model;
using Dt.Base;
using Dt.Base.Docking;
using Dt.Base.FormView;
using Dt.Core;
using Dt.Core.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
#endregion

namespace Dt.Shell
{
    /// <summary>
    /// 系统存根
    /// </summary>
    public class Stub : IStub
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServerUrl => "https://10.10.1.16/baisui";

        /// <summary>
        /// 系统标题
        /// </summary>
        public string Title => "百岁管理";

        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        public UIElement LoginPage => new Login { Desc = "吉林省现代信息技术有限公司\r\nCopyright 2017-2020 版权所有" };

        /// <summary>
        /// 系统启动
        /// </summary>
        /// <param name="p_info">提示信息</param>
        public async void OnStartup(StartupInfo p_info)
        {
            // 更新打开模型库
            string error = await AtApp.OpenModelDb("cm");
            if (!string.IsNullOrEmpty(error))
            {
                p_info.SetMessage(error);
                return;
            }

            string phone = AtLocal.GetCookie("LoginPhone");
            string pwd = AtLocal.GetCookie("LoginPwd");
            if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(pwd))
            {
                // 自动登录
                Dict dt = await AtCm.LoginByPwd(phone, pwd);
                if (dt.Bool("valid"))
                {
                    // 登录成功
                    MenuKit.InitRoles(dt.Str("roles"));
                    AtApp.LoginSuccess(dt.Long("userid"), phone, dt.Str("name"));
                    return;
                }
            }

            // 未登录或登录失败
            AtSys.Login(false);
        }

        /// <summary>
        /// 系统注销时的处理
        /// </summary>
        public Task OnLogout()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 系统关闭时的处理，必须耗时小！
        /// </summary>
        /// <returns></returns>
        public Task OnShutDown()
        {
            return Task.CompletedTask;
        }

        #region 自动生成
        /// <summary>
        /// 获取视图字典
        /// </summary>
        public Dictionary<string, Type> ViewTypes => new Dictionary<string, Type>
        {
            { "基础代码", typeof(BaseCode) },
            { "菜单管理", typeof(SysMenu) },
            { "基础权限", typeof(BasePrivilege) },
            { "系统角色", typeof(SysRole) },
            { "参数定义", typeof(UserParams) },
            { "用户账号", typeof(UserAccount) },
            { "主页", typeof(DefaultHome) },
        };

        /// <summary>
        /// 处理服务器推送的类型字典
        /// </summary>
        public Dictionary<string, Type> PushHandlers => new Dictionary<string, Type>
        {
            { "syspushapi", typeof(SysPushApi) },
        };

        /// <summary>
        /// 获取自定义可序列化类型字典
        /// </summary>
        public Dictionary<string, Type> SerializeTypes => new Dictionary<string, Type>
        {
        };

        /// <summary>
        /// 获取状态库表类型
        /// </summary>
        public Dictionary<string, Type> StateTbls => new Dictionary<string, Type>
        {
            { "menufav", typeof(MenuFav) },
            { "filereadlog", typeof(FileReadLog) },
            { "docklayout", typeof(DockLayout) },
            { "celllastval", typeof(CellLastVal) },
            { "searchfvhis", typeof(SearchFvHis) },
            { "clientlog", typeof(ClientLog) },
            { "clientcookie", typeof(ClientCookie) },
        };

        /// <summary>
        /// 获取状态库版本号，和本地不同时自动更新
        /// </summary>
        public string StateDbVer => "a9d6f73f1508bf8a2e8f00486b1a6d77";
        #endregion
    }
}
