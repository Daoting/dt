﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System.Collections.Frozen;
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
                throw new Exception("Stub 为单例对象！若重启请使用 Reboot 方法");
            Inst = this;

            var svc = new ServiceCollection();
            ConfigureServices(svc);
            SvcProvider = svc.BuildServiceProvider();

            // 后台任务启动时可能为null
            var app = Application.Current;
            if (app != null)
            {
                var mi = app.GetType().GetMethod("InitDtDictionary", BindingFlags.Public | BindingFlags.Instance);
                if (mi == null)
                    throw new Exception(app.GetType().Name + " 中不包括 InitDtDictionary 方法！");
                mi.Invoke(app, new object[0]);
            }
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
        internal Type GetTypeByAlias(Type p_attrType, string p_alias)
        {
            if (p_attrType != null && !string.IsNullOrEmpty(p_alias))
            {
                // 键规则：标签类名去掉尾部的Attribute-别名，如：View-主页
                var name = p_attrType.Name;
                var key = $"{name.Substring(0, name.Length - 9)}-{p_alias}";
                if (_aliasTypes.TryGetValue(key, out var tp))
                    return tp;
            }
            return null;
        }

        /// <summary>
        /// 根据类型别名获取类型列表
        /// </summary>
        /// <param name="p_attrType">标签类型</param>
        /// <param name="p_alias">别名</param>
        /// <returns>返回类型</returns>
        internal List<Type> GetTypeListByAlias(Type p_attrType, string p_alias)
        {
            if (p_attrType != null && !string.IsNullOrEmpty(p_alias))
            {
                // 键规则：标签类名去掉尾部的Attribute-别名，如：View-主页
                var name = p_attrType.Name;
                var key = $"{name.Substring(0, name.Length - 9)}-{p_alias}";
                if (_aliasTypeList.TryGetValue(key, out var ls))
                    return ls;
            }
            return new List<Type>();
        }

        internal FrozenDictionary<string, SqliteTblsInfo> _sqliteDbs;
        internal FrozenDictionary<string, Type> _aliasTypes;
        internal FrozenDictionary<string, List<Type>> _aliasTypeList;
        #endregion

    }
}