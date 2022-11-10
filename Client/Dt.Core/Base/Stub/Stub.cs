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
        public Stub()
        {
            if (Inst != null)
                throw new Exception("Stub 为单例对象！若重启请使用 Reboot 方法");
            Inst = this;

            var svc = new ServiceCollection();
            ConfigureServices(svc);
            SvcProvider = svc.BuildServiceProvider();

            MergeSqliteDbs(_sqliteDbs);
            MergeTypeAlias(_typeAlias);
        }

        /// <summary>
        /// 系统标题
        /// </summary>
        public string Title { get; protected set; }

        /// <summary>
        /// 注入全局服务
        /// </summary>
        /// <param name="p_svcs"></param>
        protected virtual void ConfigureServices(IServiceCollection p_svcs) { }

        /// <summary>
        /// 初始化完毕，系统启动
        /// </summary>
        protected virtual Task OnStartup() { return Task.CompletedTask; }

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
                if (_sqliteDbs.TryGetValue(p_dbName, out var info))
                    return info;
            }
            return null;
        }

        /// <summary>
        /// 根据类型别名获取类型
        /// </summary>
        /// <param name="p_attrType">标签类型</param>
        /// <param name="p_alias">别名</param>
        /// <returns>返回类型</returns>
        internal List<Type> GetTypesByAlias(Type p_attrType, string p_alias)
        {
            if (p_attrType != null && !string.IsNullOrEmpty(p_alias))
            {
                // 键规则：标签类名去掉尾部的Attribute-别名，如：View-主页
                var name = p_attrType.Name;
                var key = $"{name.Substring(0, name.Length - 9)}-{p_alias}";
                if (_typeAlias.TryGetValue(key, out var ls))
                    return ls;
            }
            return new List<Type>();
        }

        readonly Dictionary<string, SqliteTblsInfo> _sqliteDbs = new Dictionary<string, SqliteTblsInfo>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<string, List<Type>> _typeAlias = new Dictionary<string, List<Type>>(StringComparer.OrdinalIgnoreCase);
        #endregion

        #region 自动生成
        // 以下方法内容由 Dt.BuildTools 在编译前自动生成

        /// <summary>
        /// 合并本地库的结构信息，键为小写的库文件名(不含扩展名)，值为该库信息，包括版本号和表结构的映射类型
        /// 先调用base.MergeSqliteDbs，不可覆盖上级的同名本地库
        /// </summary>
        /// <param name="p_dict"></param>
        protected virtual void MergeSqliteDbs(Dictionary<string, SqliteTblsInfo> p_dict) { }

        /// <summary>
        /// 合并类型别名字典，程序集中所有贴有 TypeAliasAttribute 子标签的类型都会收集到字典，如视图类型、工作流表单类型等
        /// 先调用 base.MergeTypeAlias，后添加的别名相同的项放在列表的头部
        /// 键规则：标签类名去掉尾部的Attribute-别名，如：View-主页
        /// </summary>
        /// <param name="p_dict"></param>
        protected virtual void MergeTypeAlias(Dictionary<string, List<Type>> p_dict) { }

        /// <summary>
        /// 合并新类型，若存在相同别名的列表则插入头部，不存在则创建新列表
        /// </summary>
        /// <param name="p_dict"></param>
        /// <param name="p_key"></param>
        /// <param name="p_type"></param>
        protected void DoMergeTypeAlias(Dictionary<string, List<Type>> p_dict, string p_key, Type p_type)
        {
            List<Type> ls;
            if (!p_dict.TryGetValue(p_key, out ls))
            {
                ls = new List<Type>();
                ls.Add(p_type);
                p_dict[p_key] = ls;
            }
            else
            {
                // 插入头部
                ls.Insert(0, p_type);
            }
        }
        #endregion
    }
}