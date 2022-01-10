#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 序列化时类型/名称映射表，只针对枚举、数组、自定义类型
    /// </summary>
    internal static class SerializeTypeAlias
    {
        /// <summary>
        /// 内置类型缓存
        /// </summary>
        static readonly Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();

        static SerializeTypeAlias()
        {
            // 可序列化类型
            _typeCache = new Dictionary<string, Type>();
            _typeCache["tbl"] = typeof(Table);
            _typeCache["row"] = typeof(Row);
            _typeCache["dict"] = typeof(Dict);
            _typeCache["ss"] = typeof(List<string>);
            _typeCache["bs"] = typeof(List<bool>);
            _typeCache["is"] = typeof(List<int>);
            _typeCache["ls"] = typeof(List<long>);
            _typeCache["ds"] = typeof(List<double>);
            _typeCache["dates"] = typeof(List<DateTime>);
            _typeCache["objs"] = typeof(List<object>);
            _typeCache["tbls"] = typeof(List<Table>);
            _typeCache["dicts"] = typeof(List<Dict>);
            // 客户端UWP Release版无法反序列化，因Native无法调用ToArray方法！！！
            //_typeCache["objarr"] = typeof(object[]);
            _typeCache["msg"] = typeof(MsgInfo);
            _typeCache["letter"] = typeof(LetterInfo);
        }

        /// <summary>
        /// 查询可序列化类型，未找到时自动抛出异常
        /// </summary>
        /// <param name="p_alias">类型别名</param>
        /// <returns></returns>
        public static Type GetType(string p_alias)
        {
            if (_typeCache.TryGetValue(p_alias, out var tp))
                return tp;

            // 非内置类型
            return typeof(object);
        }

        /// <summary>
        /// 查询某类型序列化时的别名
        /// </summary>
        /// <param name="p_type">指定类型</param>
        /// <returns></returns>
        public static string GetAlias(Type p_type)
        {
            foreach (var item in _typeCache)
            {
                if (item.Value == p_type)
                    return item.Key;
            }

            // 非内置类型
            return "object";
        }

        /// <summary>
        /// 是否为内置类型
        /// </summary>
        /// <param name="p_type"></param>
        /// <returns></returns>
        public static bool IsInternal(Type p_type)
        {
            return p_type.IsValueType
                || p_type == typeof(string)
                || p_type == typeof(byte[])
                || _typeCache.Values.Contains(p_type);
        }
    }
}
