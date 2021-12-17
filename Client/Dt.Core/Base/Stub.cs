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
    public abstract class Stub
    {
        string _serverUrl;

        public Stub()
        {
            Init();
        }

        /// <summary>
        /// 服务器地址，如：https://10.10.1.16/fz
        /// </summary>
        public string ServerUrl
        {
            get { return _serverUrl; }
            protected set
            {
#if WASM
                _serverUrl = Kit.GetServerUrl(value);
#else
                if (!string.IsNullOrEmpty(value))
                    _serverUrl = value.TrimEnd('\\');
#endif
            }
        }

        /// <summary>
        /// 系统标题
        /// </summary>
        public string Title { get; protected set; }

        /// <summary>
        /// 系统描述信息
        /// </summary>
        public string Desc { get; protected set; }

        /// <summary>
        /// 后台任务类型，继承BgTask
        /// </summary>
        public Type BgTaskType { get; protected set; }

        /// <summary>
        /// 默认主页(DefaultHome)的固定菜单项
        /// </summary>
        public IList<OmMenu> FixedMenus { get; protected set; }

        /// <summary>
        /// 系统启动
        /// </summary>
        public abstract Task OnStartup();

        /// <summary>
        /// 接收分享内容
        /// </summary>
        /// <param name="p_info">分享内容描述</param>
        public virtual void ReceiveShare(ShareInfo p_info) { }

        /// <summary>
        /// 系统注销时的处理
        /// </summary>
        public virtual Task OnLogout()  => Task.CompletedTask;

        /// <summary>
        /// 挂起时的处理，必须耗时小！
        /// 手机或PC平板模式下不占据屏幕时触发，此时不确定被终止还是可恢复
        /// </summary>
        /// <returns></returns>
        public virtual Task OnSuspending() => Task.CompletedTask;

        /// <summary>
        /// 恢复会话时的处理，手机或PC平板模式下再次占据屏幕时触发
        /// </summary>
        public virtual void OnResuming() { }


        //--------------------以下内容自动生成----------------------------------
        protected abstract void Init();
        
        /// <summary>
        /// 视图字典
        /// </summary>
        public Dictionary<string, Type> ViewTypes { get; protected set; }

        /// <summary>
        /// 处理服务器推送的类型字典
        /// </summary>
        public Dictionary<string, Type> PushHandlers { get; protected set; }

        /// <summary>
        /// 自定义可序列化类型字典
        /// </summary>
        public Dictionary<string, Type> SerializeTypes { get; protected set; }

        /// <summary>
        /// 本地库的结构信息，键为小写的库文件名(不含扩展名)，值为该库信息，包括版本号和表结构的映射类型
        /// </summary>
        public Dictionary<string, SqliteTblsInfo> SqliteDb { get; protected set; }
    }
}