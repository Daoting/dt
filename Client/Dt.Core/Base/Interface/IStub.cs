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
        /// 服务器地址，末尾不包含'/'
        /// </summary>
        string ServerUrl { get; }

        /// <summary>
        /// 系统标题
        /// </summary>
        string Title { get; }

        /// <summary>
        /// 系统描述信息
        /// </summary>
        string Desc { get; }

        /// <summary>
        /// 默认主页(DefaultHome)的固定菜单项
        /// </summary>
        IList<OmMenu> FixedMenus { get; }

        /// <summary>
        /// 后台任务类型，继承BgTask
        /// </summary>
        Type BgTaskType { get; }

        /// <summary>
        /// 系统启动
        /// </summary>
        Task OnStartup();

        /// <summary>
        /// 接收分享内容
        /// </summary>
        /// <param name="p_info">分享内容描述</param>
        void ReceiveShare(ShareInfo p_info);

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
        /// 本地库的结构信息，键为小写的库文件名(不含扩展名)，值为该库信息，包括版本号和表结构的映射类型
        /// </summary>
        Dictionary<string, SqliteTblsInfo> SqliteDb { get; }
    }
}