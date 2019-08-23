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
using Windows.UI.Xaml;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统存根接口
    /// </summary>
    public interface IStub
    {
        /// <summary>
        /// 启动页面
        /// </summary>
        /// <returns></returns>
        UIElement StartPage { get; }

        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        UIElement LoginPage { get; }

        /// <summary>
        /// 系统标题
        /// </summary>
        string Title { get; }

        /// <summary>
        /// 系统描述信息
        /// </summary>
        string Desc { get; }

        /// <summary>
        /// 服务器地址
        /// </summary>
        string ServerUrl { get; }

        /// <summary>
        /// 系统是否为单机模式
        /// </summary>
        bool IsLocalMode { get; }

        /// <summary>
        /// 是否允许延迟登录
        /// </summary>
        bool AllowDelayLogin { get; }

        /// <summary>
        /// 视图字典
        /// </summary>
        Dictionary<string, Type> ViewTypes { get; }

        /// <summary>
        /// 自定义可序列化类型字典
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
        /// 系统注销时的处理
        /// </summary>
        Task OnLogout();

        /// <summary>
        /// 系统关闭时的处理，必须耗时小！
        /// </summary>
        /// <returns></returns>
        Task OnShutDown();
    }
}