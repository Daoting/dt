#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统存根接口
    /// </summary>
    public interface IStub
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        string ServerUrl { get; }

        /// <summary>
        /// 系统标题
        /// </summary>
        string Title { get; }

        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        UIElement LoginPage { get; }

        /// <summary>
        /// 系统启动
        /// </summary>
        /// <param name="p_info">提示信息</param>
        void OnStartup(StartupInfo p_info);

        /// <summary>
        /// 系统注销时的处理
        /// </summary>
        Task OnLogout();

        /// <summary>
        /// 挂起时的处理，必须耗时小！
        /// 手机或PC平板模式下不占据屏幕时触发，此时不确定被终止还是可恢复
        /// </summary>
        /// <returns></returns>
        Task OnSuspending();

        /// <summary>
        /// 恢复会话时的处理，手机或PC平板模式下再次占据屏幕时触发
        /// </summary>
        void OnResuming();

        //--------------------以下内容自动生成----------------------------------
        /// <summary>
        /// 视图字典
        /// </summary>
        Dictionary<string, Type> ViewTypes { get; }

        /// <summary>
        /// 处理服务器推送的类型字典
        /// </summary>
        Dictionary<string, Type> PushHandlers { get; }

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
    }
}