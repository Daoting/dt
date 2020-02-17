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
            Type tp;
            if (_typeCache.TryGetValue(p_alias, out tp))
                return tp;
            throw new Exception($"未找到 {p_alias} 映射的类型！");
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
            throw new Exception($"类“{p_type.FullName}”不存在可序列化别名！");
        }

        /// <summary>
        /// 合并可序列化类型字典
        /// </summary>
        /// <param name="p_dict"></param>
        public static void Merge(Dictionary<string, Type> p_dict)
        {
            foreach (var item in p_dict)
            {
                _typeCache[item.Key] = item.Value;
            }
        }

        /// <summary>
        /// 增加自定义序列化类型
        /// </summary>
        /// <param name="p_alias">类型别名</param>
        /// <param name="p_type">类型</param>
        public static void Add(string p_alias, Type p_type)
        {
            if (!string.IsNullOrEmpty(p_alias))
                _typeCache[p_alias] = p_type;
        }
    }
}
