#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-12-20 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 类型别名
    /// </summary>
    public interface ITypeAlias
    {
        /// <summary>
        /// 获取某本地库的结构信息
        /// </summary>
        /// <param name="p_dbName">库名</param>
        /// <returns></returns>
        SqliteTblsInfo GetSqliteDbInfo(string p_dbName);
        
        /// <summary>
        /// 根据类型别名获取类型
        /// </summary>
        /// <param name="p_attrType">标签类型</param>
        /// <param name="p_alias">别名</param>
        /// <returns>返回类型</returns>
        Type GetTypeByAlias(Type p_attrType, string p_alias);

        /// <summary>
        /// 根据类型别名获取类型列表
        /// </summary>
        /// <param name="p_attrType">标签类型</param>
        /// <param name="p_alias">别名</param>
        /// <returns>返回类型</returns>
        List<Type> GetTypeListByAlias(Type p_attrType, string p_alias);

        /// <summary>
        /// 返回标签类型标记的所有类型列表
        /// </summary>
        /// <param name="p_attrType">标签类型</param>
        /// <returns></returns>
        IEnumerable<Type> GetAllTypesByAttrType(Type p_attrType);
        
        /// <summary>
        /// 所有类型别名
        /// </summary>
        IReadOnlyDictionary<string, Type> AllAliasTypes { get; }

        /// <summary>
        /// 所有类型别名的类型列表
        /// </summary>
        IReadOnlyDictionary<string, List<Type>> AllAliasTypeList { get; }
    }
}