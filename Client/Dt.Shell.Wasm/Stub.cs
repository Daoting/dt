#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Dt.App;
using Dt.App.Chat;
using Dt.App.Home;
using Dt.App.Model;
using Dt.Base;
using Dt.Base.Docking;
using Dt.Base.FormView;
using Dt.Core;
using Dt.Core.Model;
using Dt.Sample;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        public string ServerUrl => "http://localhost/baisui";

        /// <summary>
        /// 系统标题
        /// </summary>
        public string Title => "福祉堂";

        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        public UIElement LoginPage => new Login { Desc = "吉林省现代信息技术有限公司\r\nCopyright 2017-2020 版权所有" };

        /// <summary>
        /// 系统启动
        /// </summary>
        /// <param name="p_info">提示信息</param>
        public void OnStartup(StartupInfo p_info)
        {
            //Grid _rootGrid = Window.Current.Content as Grid;
            //if (_rootGrid.Children.Count > 2)
            //    _rootGrid.Children.RemoveAt(0);

            //Desktop desktop = new Desktop();
            //var win = new SingleViewWin();
            //desktop.SetValue(Desktop.MainWinProperty, win);

            //_rootGrid.Children.Insert(0, desktop);
            //Console.WriteLine("宽" + Window.Current.Bounds.Width.ToString());
            //Console.WriteLine("宽" + Window.Current.CoreWindow.Bounds.Width.ToString());

            //var win = new SingleViewWin();
            //_rootGrid.Children.Insert(0, win);

            AtApp.LoadRootUI();
        }

        /// <summary>
        /// 系统注销时的处理
        /// </summary>
        public Task OnLogout()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 挂起时的处理，必须耗时小！
        /// 手机或PC平板模式下不占据屏幕时触发，此时不确定被终止还是可恢复
        /// </summary>
        /// <returns></returns>
        public Task OnSuspending()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 恢复会话时的处理，手机或PC平板模式下再次占据屏幕时触发
        /// </summary>
        public void OnResuming()
        {
        }

        void CreateFixedMenus()
        {
            MenuKit.FixedMenus = new List<OmMenu>
            {
                new OmMenu
                {
                    ID = 1110,
                    Name = "通讯录",
                    Icon = "留言",
                    ViewName = "通讯录"
                },
                new OmMenu
                {
                    ID = 2000,
                    Name = "福祉主页",
                    Icon = "主页",
                    ViewName = "福祉主页"
                },
            };
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
            { "主页", typeof(SamplesMain) },
            { "通讯录", typeof(ChatHome) },
            { "控件样例", typeof(SamplesMain) },
            { "文章管理", typeof(Fz.PostMgr) },
            { "福祉主页", typeof(Fz.ClientHome) },
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
            { "产品", typeof(Product) },
            { "学生", typeof(Student) },
            { "部门", typeof(Department) },
        };

        /// <summary>
        /// 获取状态库表类型
        /// </summary>
        public Dictionary<string, Type> StateTbls => new Dictionary<string, Type>
        {
            { "menufav", typeof(MenuFav) },
            { "chatmember", typeof(ChatMember) },
            { "letter", typeof(Letter) },
            { "docklayout", typeof(DockLayout) },
            { "celllastval", typeof(CellLastVal) },
            { "searchfvhis", typeof(SearchFvHis) },
            { "clientlog", typeof(ClientLog) },
            { "clientcookie", typeof(ClientCookie) },
        };

        /// <summary>
        /// 获取状态库版本号，和本地不同时自动更新
        /// </summary>
        public string StateDbVer => "cfe061dec371ded2b7692c2c00bc9a40";
        #endregion
    }
}
