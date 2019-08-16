#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统存根接口
    /// </summary>
    public interface IStub
    {
        /// <summary>
        /// 获取系统标题
        /// </summary>
        string Title { get; }

        /// <summary>
        /// 获取系统描述信息
        /// </summary>
        string Desc { get; }

        /// <summary>
        /// 获取服务器地址
        /// </summary>
        string ServerUrl { get; }

        /// <summary>
        /// 获取系统是否为单机模式
        /// </summary>
        bool IsLocalMode { get; }

        /// <summary>
        /// 获取是否允许延迟登录
        /// </summary>
        bool AllowDelayLogin { get; }

        /// <summary>
        /// 获取固定菜单项
        /// </summary>
        List<OmMenu> FixedMenus { get; }

        /// <summary>
        /// 获取外部处理后台任务的类型
        /// </summary>
        Type BgTaskType { get; }

        /// <summary>
        /// 获取视图字典
        /// </summary>
        Dictionary<string, Type> ViewTypes { get; }

        /// <summary>
        /// 获取流程表单字典
        /// </summary>
        Dictionary<string, Type> FormTypes { get; }

        /// <summary>
        /// 获取流程Sheet字典
        /// </summary>
        Dictionary<string, Type> SheetTypes { get; }

        /// <summary>
        /// 获取本地服务类型字典
        /// </summary>
        Dictionary<string, Type> ServiceTypes { get; }

        /// <summary>
        /// 获取自定义可序列化类型字典
        /// </summary>
        Dictionary<string, Type> SerializeTypes { get; }

        /// <summary>
        /// 获取状态库版本号，和本地不同时自动更新
        /// </summary>
        string StateDbVer { get; }

        /// <summary>
        /// 获取状态库表类型，键值为小写类型名
        /// </summary>
        Dictionary<string, Type> StateTbls { get; }

        /// <summary>
        /// 启动处理，登录成功后未加载桌面前调用
        /// </summary>
        /// <returns></returns>
        Task Startup();

        /// <summary>
        /// 系统关闭或挂起时调用，必须耗时小！
        /// </summary>
        /// <returns></returns>
        Task ShutDown();
    }
}