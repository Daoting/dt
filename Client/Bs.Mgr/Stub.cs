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
using Dt.Base.Docking;
using Dt.Base.FormView;
using Dt.Core;
using Dt.Core.Model;
using Dt.Sample;
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
        /// 启动页面
        /// </summary>
        /// <returns></returns>
        public UIElement StartPage => new Startup();

        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        public UIElement LoginPage => new Login();

        /// <summary>
        /// 系统标题
        /// </summary>
        public string Title => "百岁";

        /// <summary>
        /// 系统描述信息
        /// </summary>
        public string Desc => "吉林省现代信息技术有限公司\r\nCopyright 2017-2020 版权所有";

        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServerUrl => "https://10.10.1.16/baisui";

        /// <summary>
        /// 是否为单机模式
        /// </summary>
        public bool IsLocalMode => false;

        /// <summary>
        /// 是否允许延迟登录
        /// </summary>
        public bool AllowDelayLogin => false;

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
            { "主页", typeof(DefaultHome) },
            { "菜单管理", typeof(SysMenu) },
            { "系统模块", typeof(SysModule) },
            { "参数定义", typeof(SysParams) },
            { "基础权限", typeof(SysPrivilege) },
            { "控件样例", typeof(SamplesMain) },
        };

        /// <summary>
        /// 获取自定义可序列化类型字典
        /// </summary>
        public Dictionary<string, Type> SerializeTypes => new Dictionary<string, Type>
        {
            { "产品", typeof(Product) },
            { "学生", typeof(Student) },
            { "部门", typeof(Department) },
        };

        /// <summary>
        /// 获取状态库表类型
        /// </summary>
        public Dictionary<string, Type> StateTbls => new Dictionary<string, Type>
        {
            { "clientcookie", typeof(ClientCookie) },
            { "clientlog", typeof(ClientLog) },
            { "menufav", typeof(MenuFav) },
            { "docklayout", typeof(DockLayout) },
            { "celllastval", typeof(CellLastVal) },
        };

        /// <summary>
        /// 获取状态库版本号，和本地不同时自动更新
        /// </summary>
        public string StateDbVer => "83975e59c4e069a15333a2d0ada18e2e";
        #endregion
    }
}
