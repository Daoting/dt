#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-04-16 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Sqlite
{
    /// <summary>
    /// 实体结构定义
    /// </summary>
    class SqliteEntitySchema
    {
        public SqliteEntitySchema(Type p_type)
        {
            OnSaving = GetMethod(p_type, "OnSaving");
            OnDeleting = GetMethod(p_type, "OnDeleting");
        }

        /// <summary>
        /// 保存前的处理，抛出异常时取消保存，实体中的方法规范：私有方法OnSaving，无入参，返回值void 或 Task
        /// </summary>
        public MethodInfo OnSaving { get; }

        /// <summary>
        /// 删除前的处理，抛出异常时取消删除，实体中的方法规范：私有方法OnDeleting，无入参，返回值void 或 Task
        /// </summary>
        public MethodInfo OnDeleting { get; }

        #region 静态内容
        static readonly ConcurrentDictionary<Type, SqliteEntitySchema> _models = new ConcurrentDictionary<Type, SqliteEntitySchema>();

        /// <summary>
        /// 获取实体类型的定义
        /// </summary>
        /// <param name="p_type">实体类型</param>
        /// <returns></returns>
        public static SqliteEntitySchema Get(Type p_type)
        {
            if (_models.TryGetValue(p_type, out var m))
                return m;

            var model = new SqliteEntitySchema(p_type);
            _models[p_type] = model;
            return model;
        }
        
        static MethodInfo GetMethod(Type p_type, string p_name)
        {
            var mi = p_type.GetMethod(p_name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly);
            if (mi != null
                && (mi.ReturnType == typeof(void) || mi.ReturnType == typeof(Task))
                && mi.GetParameters().Length == 0)
            {
                return mi;
            }
            return null;
        }
        #endregion
    }
}
