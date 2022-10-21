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
        /// 合并本地库的结构信息，键为小写的库文件名(不含扩展名)，值为该库信息，包括版本号和表结构的映射类型
        /// 先调用base.MergeSqliteDbs，不可覆盖上级的同名本地库
        /// </summary>
        /// <returns></returns>
        protected virtual void MergeSqliteDbs(Dictionary<string, SqliteTblsInfo> p_sqliteDbs) { }
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
                    _sqliteDbs = new Dictionary<string, SqliteTblsInfo>(StringComparer.OrdinalIgnoreCase);
                    MergeSqliteDbs(_sqliteDbs);
                }

                if (_sqliteDbs.TryGetValue(p_dbName, out var info))
                    return info;
            }
            return null;
        }

        Dictionary<string, SqliteTblsInfo> _sqliteDbs;
        #endregion
    }
}