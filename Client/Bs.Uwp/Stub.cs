#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Model;
using Dt.Sample;
using Dt.App;
using Dt.App.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Shell
{
    /// <summary>
    /// 系统存根
    /// </summary>
    public class Stub : IStub
    {
        /// <summary>
        /// 获取系统标题
        /// </summary>
        public string Title
        {
            get { return "百岁"; }
        }

        /// <summary>
        /// 获取系统描述信息
        /// </summary>
        public string Desc
        {
            get { return "吉林省现代信息技术有限公司\r\nCopyright 2017-2020 版权所有"; }
        }

        /// <summary>
        /// 获取服务器地址
        /// </summary>
        public string ServerUrl
        {
            get { return "https://10.10.1.16/baisui"; }
        }

        /// <summary>
        /// 获取系统是否为单机模式
        /// </summary>
        public bool IsLocalMode
        {
            get { return false; }
        }

        /// <summary>
        /// 获取是否允许延迟登录
        /// </summary>
        public bool AllowDelayLogin
        {
            get { return false; }
        }

        /// <summary>
        /// 获取固定菜单项，可使用默认或自定义
        /// </summary>
        public List<OmMenu> FixedMenus
        {
            get { return AtUser.DefaultFixedMenus; }
        }

        /// <summary>
        /// 获取外部处理后台任务的类型
        /// </summary>
        public Type BgTaskType
        {
            get { return null; }
        }

        /// <summary>
        /// 获取视图字典
        /// </summary>
        public Dictionary<string, Type> ViewTypes
        {
            get { return _viewTypes; }
        }

        /// <summary>
        /// 获取流程表单字典
        /// </summary>
        public Dictionary<string, Type> FormTypes
        {
            get { return _formTypes; }
        }

        /// <summary>
        /// 获取流程Sheet字典
        /// </summary>
        public Dictionary<string, Type> SheetTypes
        {
            get { return _sheetTypes; }
        }

        /// <summary>
        /// 获取本地服务类型字典
        /// </summary>
        public Dictionary<string, Type> ServiceTypes
        {
            get { return _serviceTypes; }
        }

        /// <summary>
        /// 获取自定义可序列化类型字典
        /// </summary>
        public Dictionary<string, Type> SerializeTypes
        {
            get { return _serializeTypes; }
        }

        /// <summary>
        /// 获取状态库表类型
        /// </summary>
        public Dictionary<string, Type> StateTbls
        {
            get { return _stateTbls; }
        }

        /// <summary>
        /// 获取状态库版本号，和本地不同时自动更新
        /// </summary>
        public string StateDbVer
        {
            get { return _stateDbVer; }
        }

        /// <summary>
        /// 启动处理，登录成功后未加载桌面前调用
        /// </summary>
        /// <returns></returns>
        public Task Startup()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 系统关闭或挂起时调用，必须耗时小！
        /// </summary>
        /// <returns></returns>
        public Task ShutDown()
        {
            return Task.CompletedTask;
        }

        #region 自动生成
        readonly Dictionary<string, Type> _viewTypes = new Dictionary<string, Type>
        {
            { "主页", typeof(DefaultHome) },
            { "菜单管理", typeof(SysMenu) },
            { "系统模块", typeof(SysModule) },
            { "参数定义", typeof(SysParams) },
            { "基础权限", typeof(SysPrivilege) },
            { "控件样例", typeof(SamplesMain) },
        };

        readonly Dictionary<string, Type> _formTypes = new Dictionary<string, Type>
        {
        };

        readonly Dictionary<string, Type> _sheetTypes = new Dictionary<string, Type>
        {
        };

        readonly Dictionary<string, Type> _serviceTypes = new Dictionary<string, Type>
        {

        };

        readonly Dictionary<string, Type> _serializeTypes = null;

        readonly Dictionary<string, Type> _stateTbls = new Dictionary<string, Type>
        {
            { "clientcookie", typeof(ClientCookie) },
            { "clientlog", typeof(ClientLog) },
            { "menufav", typeof(MenuFav) },
            { "docklayout", typeof(DockLayout) },
            { "celllastval", typeof(CellLastVal) },
            { "letter", typeof(Letter) },
            { "filereadlog", typeof(FileReadLog) },
        };

        const string _stateDbVer = "83975e59c4e069a15333a2d0ada18e2e";
        #endregion
    }
}
