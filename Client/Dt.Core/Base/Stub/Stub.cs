#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统存根
    /// </summary>
    public abstract partial class Stub
    {
        public Stub()
        {
            if (Inst != null)
                throw new Exception("Stub为单例对象！");
            Inst = this;

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

        #region 自动生成
        /// <summary>
        /// 视图名称与窗口类型的映射字典，主要菜单项用，同名时覆盖内置的视图类型
        /// </summary>
        /// <returns></returns>
        protected abstract Dictionary<string, Type> GetViewTypes();

        /// <summary>
        /// 内置视图字典
        /// </summary>
        /// <returns></returns>
        protected abstract Dictionary<string, Type> GetInternalViewTypes();

        /// <summary>
        /// 本地库的结构信息，键为小写的库文件名(不含扩展名)，值为该库信息，包括版本号和表结构的映射类型
        /// </summary>
        /// <returns></returns>
        protected abstract Dictionary<string, SqliteTblsInfo> GetSqliteDbs();

        /// <summary>
        /// 内置本地库的结构信息
        /// </summary>
        /// <returns></returns>
        protected abstract Dictionary<string, SqliteTblsInfo> GetInternalSqliteDbs();
        #endregion

        #region 内部成员
        /// <summary>
        /// 内部访问存根实例
        /// </summary>
        internal static Stub Inst { get; private set; }

        /// <summary>
        /// 依赖注入的全局服务对象提供者
        /// </summary>
        internal readonly IServiceProvider SvcProvider;

        /// <summary>
        /// 获取视图类型
        /// </summary>
        /// <param name="p_typeName">类型名称</param>
        /// <returns>返回类型</returns>
        internal Type GetViewType(string p_typeName)
        {
            if (!string.IsNullOrEmpty(p_typeName))
            {
                if (_viewTypes == null)
                {
                    _viewTypes = GetInternalViewTypes();
                    if (_viewTypes == null)
                        _viewTypes = new Dictionary<string, Type>();

                    var dict = GetViewTypes();
                    if (dict != null && dict.Count > 0)
                    {
                        foreach (var item in dict)
                        {
                            // 可覆盖内置的视图类型
                            _viewTypes[item.Key] = item.Value;
                        }
                    }
                }

                if (_viewTypes.TryGetValue(p_typeName, out var tp))
                    return tp;
            }
            return null;
        }

        /// <summary>
        /// 获取某本地库的结构信息
        /// </summary>
        /// <param name="p_dbName">库名</param>
        /// <returns></returns>
        internal SqliteTblsInfo GetSqliteDbInfo(string p_dbName)
        {
            if (!string.IsNullOrEmpty(p_dbName))
            {
                if (_sqliteDbs == null)
                {
                    _sqliteDbs = GetInternalSqliteDbs();
                    if (_sqliteDbs == null)
                        _sqliteDbs = new Dictionary<string, SqliteTblsInfo>();

                    var dict = GetSqliteDbs();
                    if (dict != null && dict.Count > 0)
                    {
                        foreach (var item in dict)
                        {
                            // 不可覆盖内置的本地库
                            if (!_sqliteDbs.ContainsKey(item.Key))
                                _sqliteDbs[item.Key] = item.Value;
                        }
                    }
                }

                if (_sqliteDbs.TryGetValue(p_dbName, out var info))
                    return info;
            }
            return null;
        }

        string _cmSvcUrl;
        Dictionary<string, Type> _viewTypes;
        Dictionary<string, SqliteTblsInfo> _sqliteDbs;
        #endregion
    }
}