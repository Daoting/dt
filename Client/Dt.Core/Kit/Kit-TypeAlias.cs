#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 类型别名
    /// </summary>
    public partial class Kit
    {
        /// <summary>
        /// 根据共享类型别名获取实例对象，通常用在两个无引用关系的dll之间的互相访问
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="p_alias">别名</param>
        /// <returns>返回共享类型的实例对象</returns>
        public static T GetShareObj<T>(string p_alias)
        {
            var tp = _typeAlias.GetTypeByAlias(typeof(ShareAttribute), p_alias);
            if (tp == null)
                Throw.Msg($"别名为{p_alias}的共享类型不存在！");
            return (T)Activator.CreateInstance(tp);
        }

        /// <summary>
        /// 根据共享类型别名获取实例对象，通常用在两个无引用关系的dll之间的互相访问
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="p_enumAlias">别名取枚举成员名称</param>
        /// <returns>返回共享类型的实例对象</returns>
        public static T GetShareObj<T>(Enum p_enumAlias)
        {
            return GetShareObj<T>(p_enumAlias.ToString());
        }

        /// <summary>
        /// 根据别名获取共享类型，通常用在两个无引用关系的dll之间的互相访问
        /// </summary>
        /// <param name="p_alias">类型别名</param>
        /// <returns>返回类型</returns>
        public static Type GetShareType(string p_alias)
        {
            return _typeAlias.GetTypeByAlias(typeof(ShareAttribute), p_alias);
        }

        /// <summary>
        /// 根据别名获取共享类型，通常用在两个无引用关系的dll之间的互相访问
        /// </summary>
        /// <param name="p_enumAlias">别名取枚举成员名称</param>
        /// <returns>返回类型</returns>
        public static Type GetShareType(Enum p_enumAlias)
        {
            return _typeAlias.GetTypeByAlias(typeof(ShareAttribute), p_enumAlias.ToString());
        }

        /// <summary>
        /// 根据类型别名获取类型
        /// </summary>
        /// <param name="p_attrType">标签类型</param>
        /// <param name="p_alias">别名</param>
        /// <returns>返回类型</returns>
        public static Type GetTypeByAlias(Type p_attrType, string p_alias)
        {
            return _typeAlias.GetTypeByAlias(p_attrType, p_alias);
        }

        /// <summary>
        /// 根据别名获取视图类型
        /// </summary>
        /// <param name="p_alias">类型别名</param>
        /// <returns>返回类型</returns>
        public static Type GetViewTypeByAlias(string p_alias)
        {
            return _typeAlias.GetTypeByAlias(typeof(ViewAttribute), p_alias);
        }

        /// <summary>
        /// 根据别名获取视图类型
        /// </summary>
        /// <param name="p_enumAlias">别名取枚举成员名称</param>
        /// <returns>返回类型</returns>
        public static Type GetViewTypeByAlias(Enum p_enumAlias)
        {
            return _typeAlias.GetTypeByAlias(typeof(ViewAttribute), p_enumAlias.ToString());
        }

        /// <summary>
        /// 根据别名获取视图参数编辑器类型
        /// </summary>
        /// <param name="p_alias">类型别名</param>
        /// <returns>返回类型</returns>
        public static Type GetViewParamsEditorByAlias(string p_alias)
        {
            return _typeAlias.GetTypeByAlias(typeof(ViewParamsEditorAttribute), p_alias);
        }
        
        /// <summary>
        /// 根据类型别名和方法名获取方法定义，取列表中第一个匹配项
        /// </summary>
        /// <param name="p_attrType">标签类型</param>
        /// <param name="p_alias">别名</param>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_flags">要包含BindingFlags.Public 或 BindingFlags.NonPublic，否则返回null，静态的方法还要有BindingFlags.Static</param>
        /// <returns></returns>
        public static MethodInfo GetMethodByAlias(Type p_attrType, string p_alias, string p_methodName, BindingFlags p_flags)
        {
            var tp = _typeAlias.GetTypeByAlias(p_attrType, p_alias);
            return tp.GetMethod(p_methodName, p_flags);
        }

        /// <summary>
        /// 根据类型别名获取所有类型列表，基础标签为 TypeListAliasAttribute 的类型
        /// </summary>
        /// <param name="p_attrType">标签类型</param>
        /// <param name="p_alias">别名</param>
        /// <returns>返回类型</returns>
        public static List<Type> GetAllTypesByAlias(Type p_attrType, string p_alias)
        {
            return _typeAlias.GetTypeListByAlias(p_attrType, p_alias);
        }

        /// <summary>
        /// 返回标签类型标记的所有类型列表
        /// </summary>
        /// <param name="p_attrType">标签类型</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllTypesByAttrType(Type p_attrType)
        {
            return _typeAlias.GetAllTypesByAttrType(p_attrType);
        }
        
        /// <summary>
        /// 发布本地事件
        /// </summary>
        /// <param name="p_event">事件对象，禁止事件为泛型</param>
        public static async Task PublishEvent(IEvent p_event)
        {
            if (p_event == null)
                return;

            var ls = _typeAlias.GetTypeListByAlias(typeof(EventHandlerAttribute), p_event.GetType().Name);
            if (ls.Count > 0)
            {
                foreach (var tp in ls)
                {
                    var mi = tp.GetMethod("Handle");
                    if (mi != null )
                    {
                        try
                        {
                            // 按顺序调用
                            var tgt = Activator.CreateInstance(tp);
                            await(Task)mi.Invoke(tgt, new object[] { p_event });
                        }
                        catch (Exception e)
                        {
                            Log.Warning(e, $"{tp.Name}处理本地事件时异常！");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 所有类型别名
        /// </summary>
        public static IReadOnlyDictionary<string, Type> AllAliasTypes => _typeAlias.AllAliasTypes;

        /// <summary>
        /// 所有类型别名的类型列表
        /// </summary>
        public static IReadOnlyDictionary<string, List<Type>> AllAliasTypeList => _typeAlias.AllAliasTypeList;
        
        /// <summary>
        /// 获取某本地库的结构信息
        /// </summary>
        /// <param name="p_dbName">库名</param>
        /// <returns></returns>
        public static SqliteTblsInfo GetSqliteDbInfo(string p_dbName)
        {
            return _typeAlias?.GetSqliteDbInfo(p_dbName);
        }
    }
}
