#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-10-25 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Frozen;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// auto-generated代码中用
    /// </summary>
    public class TypeLoader : IDisposable
    {
        readonly Dictionary<string, Type> _alias = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<string, List<Type>> _list = new Dictionary<string, List<Type>>(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, SqliteTblsInfo> _dbs;

        public void AddAlias(string p_key, Type p_type)
        {
            _alias[p_key] = p_type;
        }

        public void AddList(string p_key, Type p_type)
        {
            List<Type> ls;
            if (!_list.TryGetValue(p_key, out ls))
            {
                ls = new List<Type>();
                ls.Add(p_type);
                _list[p_key] = ls;
            }
            else
            {
                // 插入头部
                ls.Insert(0, p_type);
            }
        }

        public void AddDbs(Dictionary<string, SqliteTblsInfo> p_dbs)
        {
            _dbs = p_dbs;
        }

        public void Dispose()
        {
            DefTypeAlias._sqliteDbs = _dbs.ToFrozenDictionary();
            DefTypeAlias._aliasTypes = _alias.ToFrozenDictionary();
            DefTypeAlias._aliasTypeList = _list.ToFrozenDictionary();
        }
    }
}