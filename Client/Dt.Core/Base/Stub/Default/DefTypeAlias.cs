#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-12-20 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Frozen;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 类型别名
    /// </summary>
    class DefTypeAlias : ITypeAlias
    {
        public SqliteTblsInfo GetSqliteDbInfo(string p_dbName)
        {
            if (!string.IsNullOrEmpty(p_dbName) && _sqliteDbs != null)
            {
                if (_sqliteDbs.TryGetValue(p_dbName, out var info))
                    return info;
            }
            return null;
        }

        public Type GetTypeByAlias(Type p_attrType, string p_alias)
        {
            if (p_attrType != null
                && !string.IsNullOrEmpty(p_alias)
                && _aliasTypes != null)
            {
                // 键规则：标签类名去掉尾部的Attribute-别名，如：View-主页
                var name = p_attrType.Name;
                var key = $"{name.Substring(0, name.Length - 9)}-{p_alias}";
                if (_aliasTypes.TryGetValue(key, out var tp))
                    return tp;
            }
            return null;
        }

        public List<Type> GetTypeListByAlias(Type p_attrType, string p_alias)
        {
            if (p_attrType != null
                && !string.IsNullOrEmpty(p_alias)
                && _aliasTypeList != null)
            {
                // 键规则：标签类名去掉尾部的Attribute-别名，如：View-主页
                var name = p_attrType.Name;
                var key = $"{name.Substring(0, name.Length - 9)}-{p_alias}";
                if (_aliasTypeList.TryGetValue(key, out var ls))
                    return ls;
            }
            return new List<Type>();
        }

        public IEnumerable<Type> GetAllTypesByAttrType(Type p_attrType)
        {
            if (p_attrType != null
                && _aliasTypes != null)
            {
                // 键规则：标签类名去掉尾部的Attribute-别名，如：View-主页
                var name = p_attrType.Name;
                var key = $"{name.Substring(0, name.Length - 9)}-";
                return from item in _aliasTypes
                       where item.Key.StartsWith(key)
                       select item.Value;
            }
            return null;
        }

        public IReadOnlyDictionary<string, Type> AllAliasTypes => _aliasTypes;

        public IReadOnlyDictionary<string, List<Type>> AllAliasTypeList => _aliasTypeList;

        internal static FrozenDictionary<string, SqliteTblsInfo> _sqliteDbs;
        internal static FrozenDictionary<string, Type> _aliasTypes;
        internal static FrozenDictionary<string, List<Type>> _aliasTypeList;
    }
}