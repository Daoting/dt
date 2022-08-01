#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统存根
    /// </summary>
    public abstract partial class Stub
    {
        string _cmSvcUrl;

        public Stub()
        {
            if (Inst != null)
                throw new Exception("Stub为单例对象！");

            Inst = this;
            Init();

            var svc = new ServiceCollection();
            ConfigureServices(svc);
            SvcProvider = svc.BuildServiceProvider();
        }
        
        /// <summary>
        /// 系统标题
        /// </summary>
        public string Title { get; protected set; }

        /// <summary>
        /// 设置cm服务地址，如：https://10.10.1.16/fz-cm
        /// <para>单机版无需设置</para>
        /// </summary>
        public string SvcUrl
        {
            get { return _cmSvcUrl; }
            protected set
            {
                _cmSvcUrl = string.IsNullOrEmpty(value) ? null : value.TrimEnd('/');
            }
        }

        /// <summary>
        /// 日志设置，在AppStub构造方法设置有效，默认输出到：Console和trace
        /// </summary>
        public LogSetting LogSetting { get; } = new LogSetting();

        /// <summary>
        /// 注入全局服务
        /// </summary>
        /// <param name="p_svcs"></param>
        protected virtual void ConfigureServices(IServiceCollection p_svcs) { }

        /// <summary>
        /// 初始化完毕，系统启动
        /// </summary>
        protected abstract Task OnStartup();

        /// <summary>
        /// 系统注销时的处理
        /// </summary>
        protected virtual Task OnLogout() => Task.CompletedTask;

        /// <summary>
        /// 内部访问存根实例
        /// </summary>
        internal static Stub Inst { get; private set; }

        /// <summary>
        /// 依赖注入的全局服务对象提供者
        /// </summary>
        internal readonly IServiceProvider SvcProvider;

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
        /// 本地库的结构信息，键为小写的库文件名(不含扩展名)，值为该库信息，包括版本号和表结构的映射类型
        /// </summary>
        public Dictionary<string, SqliteTblsInfo> SqliteDb { get; protected set; }
    }
}