#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-28 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections;
#endregion

namespace Dt.Core;

/// <summary>
/// 数据访问基类，为方便使用所有方法为静态，通过 TAgentInfo 提供实际的数据查询对象
/// </summary>
/// <typeparam name="TAgentInfo">数据访问的描述信息</typeparam>
public abstract class AccessAgent<TAgentInfo>
    where TAgentInfo : AgentInfo, new()
{
    #region 查询
    /// <summary>
    /// 以参数值方式执行Sql语句，返回结果集
    /// </summary>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回Table数据</returns>
    public static Task<Table> Query(string p_sqlOrKey, object p_params = null)
    {
        return _info.GetDa().Query(p_sqlOrKey, p_params);
    }

    /// <summary>
    /// 以参数值方式执行Sql语句，返回实体列表
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回实体列表</returns>
    public static Task<Table<TEntity>> Query<TEntity>(string p_sqlOrKey, object p_params = null)
        where TEntity : Entity
    {
        return _info.GetDa().Query<TEntity>(p_sqlOrKey, p_params);
    }

    /// <summary>
    /// 按页查询数据
    /// </summary>
    /// <param name="p_starRow">起始序号：第一行的序号统一为0</param>
    /// <param name="p_pageSize">每页显示行数</param>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回Table数据</returns>
    public static Task<Table> Page(int p_starRow, int p_pageSize, string p_sqlOrKey, object p_params = null)
    {
        return _info.GetDa().Page(
            p_starRow,
            p_pageSize,
            p_sqlOrKey,
            p_params
        );
    }

    /// <summary>
    /// 按页查询数据
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="p_starRow">起始序号：第一行的序号统一为0</param>
    /// <param name="p_pageSize">每页显示行数</param>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回Table数据集</returns>
    public static Task<Table<TEntity>> Page<TEntity>(int p_starRow, int p_pageSize, string p_sqlOrKey, object p_params = null)
        where TEntity : Entity
    {
        return _info.GetDa().Page<TEntity>(
            p_starRow,
            p_pageSize,
            p_sqlOrKey,
            p_params
        );
    }

    /// <summary>
    /// 以参数值方式执行Sql语句，只返回第一行数据
    /// </summary>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回第一行Row或null</returns>
    public static Task<Row> First(string p_sqlOrKey, object p_params = null)
    {
        return _info.GetDa().First(p_sqlOrKey, p_params);
    }

    /// <summary>
    /// 以参数值方式执行Sql语句，返回第一个实体对象，实体属性由Sql决定，不存在时返回null
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回实体对象或null</returns>
    public static Task<TEntity> First<TEntity>(string p_sqlOrKey, object p_params = null)
        where TEntity : Entity
    {
        return _info.GetDa().First<TEntity>(p_sqlOrKey, p_params);
    }

    /// <summary>
    /// 以参数值方式执行Sql语句，返回符合条件的第一列数据，并转换为指定类型
    /// </summary>
    /// <typeparam name="T">第一列数据类型</typeparam>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回第一列数据的泛型列表</returns>
    public static Task<List<T>> FirstCol<T>(string p_sqlOrKey, object p_params = null)
    {
        return _info.GetDa().FirstCol<T>(p_sqlOrKey, p_params);
    }

    /// <summary>
    /// 以参数值方式执行Sql语句，只返回第一个单元格数据
    /// </summary>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回第一个单元格数据</returns>
    public static Task<T> GetScalar<T>(string p_sqlOrKey, object p_params = null)
    {
        return _info.GetDa().GetScalar<T>(p_sqlOrKey, p_params);
    }

    /// <summary>
    /// 以参数值方式执行Sql语句，返回Row枚举，高性能，客户端代理不支持！
    /// </summary>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回Row枚举</returns>
    public static Task<IEnumerable<Row>> Each(string p_sqlOrKey, object p_params = null)
    {
        return _info.GetDa().Each(p_sqlOrKey, p_params);
    }

    /// <summary>
    /// 以参数值方式执行Sql语句，返回实体枚举，高性能，客户端代理不支持！
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回实体枚举</returns>
    public static Task<IEnumerable<TEntity>> Each<TEntity>(string p_sqlOrKey, object p_params = null)
        where TEntity : Entity
    {
        return _info.GetDa().Each<TEntity>(p_sqlOrKey, p_params);
    }

    /// <summary>
    /// 以参数值方式执行Sql语句，返回第一列枚举，高性能，客户端代理不支持！
    /// </summary>
    /// <typeparam name="T">第一列数据类型</typeparam>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    /// <returns>返回第一列数据的泛型枚举</returns>
    public static Task<IEnumerable<T>> EachFirstCol<T>(string p_sqlOrKey, object p_params = null)
    {
        return _info.GetDa().EachFirstCol<T>(p_sqlOrKey, p_params);
    }
    #endregion

    #region 增删改
    /// <summary>
    /// 以参数值方式执行Sql语句，返回影响的行数，底层方法万不得已慎用！
    /// </summary>
    /// <param name="p_sqlOrKey">Sql语句 或 Sql键名(#前缀)</param>
    /// <param name="p_params">参数值，支持Dict或匿名对象</param>
    /// <returns>执行后影响的行数</returns>
    public static Task<int> Exec(string p_sqlOrKey, object p_params = null)
    {
        return _info.GetDa().Exec(p_sqlOrKey, p_params);
    }

    /// <summary>
    /// 一个事务内执行多个Sql，底层方法万不得已慎用！
    /// </summary>
    /// <param name="p_dts">参数列表，每个Dict中包含两个键：text,params，text为sql语句params类型为Dict或List{Dict}</param>
    /// <returns>返回执行后影响的行数</returns>
    public static Task<int> BatchExec(List<Dict> p_dts)
    {
        return _info.GetDa().BatchExec(p_dts);
    }

    /// <summary>
    /// 一个事务内批量保存列表中所有需要新增、修改、删除的数据
    /// <para>1. SaveItem项中的Data只支持：有表名的Table、有表名的Row、Entity、Table{TEntity}</para>
    /// <para>2. Entity、Table{TEntity}支持实体的各种回调和领域事件</para>
    /// <para>3. 反序列化时根据表名确定是否有对应Entity，没有时按Row、Table处理</para>
    /// </summary>
    /// <param name="p_datas">保存项列表</param>
    /// <returns>是否成功</returns>
    public static Task<bool> Save(List<SaveItem> p_datas)
    {
        return _info.GetDa().Save(p_datas);
    }

    /// <summary>
    /// 一个事务内批量保存需要新增、修改、删除的数据
    /// </summary>
    /// <param name="p_item">保存项，Table{TEntity} 或 有表名的Table</param>
    /// <returns>是否成功</returns>
    public static Task<bool> Save(SaveItem p_item)
    {
        return _info.GetDa().Save(new List<SaveItem> { p_item });
    }
    #endregion

    #region 新ID和序列
    /// <summary>
    /// 获取新ID
    /// </summary>
    /// <returns></returns>
    public static Task<long> NewID()
    {
        return _info.GetDa().NewID();
    }

    /// <summary>
    /// 获取新序列值
    /// </summary>
    /// <param name="p_seqName">序列名称，不可为空</param>
    /// <returns>新序列值</returns>
    public static Task<long> NewSeq(string p_seqName)
    {
        return _info.GetDa().NewSeq(p_seqName);
    }
    #endregion

    #region 缓存
    /// <summary>
    /// 根据键查询缓存值，获取过程：
    /// <para>1. 首先从缓存中获取，有则直接返回</para>
    /// <para>2. 无且p_persist 为true，则从数据库获取，并按照rpc json规则反序列化，将结果添加到缓存以备下次使用</para>
    /// </summary>
    /// <param name="p_key">缓存键，字符串或简单值类型，推荐分段式串如：大类1:分组2:标识</param>
    /// <param name="p_persist">缓存中无时是否从数据库获取，默认false</param>
    /// <returns>返回缓存值，自动转换为指定类型</returns>
    public static Task<T> GetCache<T>(object p_key, bool p_persist = false)
    {
        return _info.GetDa().GetCache<T>(p_key, p_persist);
    }

    /// <summary>
    /// 将键值添加到缓存，并根据p_persist控制是否同时存放在数据库
    /// </summary>
    /// <param name="p_key">缓存键，字符串或简单值类型，推荐分段式串如：大类1:分组2:标识</param>
    /// <param name="p_value">缓存值，持久化时按照rpc json规则序列化键值</param>
    /// <param name="p_persist">是否同时存放在数据库，默认false</param>
    /// <returns>是否成功</returns>
    public static Task<bool> SetCache(object p_key, object p_value, bool p_persist = false)
    {
        return _info.GetDa().SetCache(p_key, p_value, p_persist);
    }

    /// <summary>
    /// 将键值添加到缓存，并设置过期时间，始终不持久化
    /// </summary>
    /// <param name="p_key">缓存键，字符串或简单值类型，推荐分段式串如：大类1:分组2:标识</param>
    /// <param name="p_value">缓存值</param>
    /// <param name="p_seconds">过期秒数，0表示永久缓存</param>
    /// <returns>是否成功</returns>
    public static Task<bool> SetExpireCache(object p_key, object p_value, double p_seconds)
    {
        return _info.GetDa().SetExpireCache(p_key, p_value, p_seconds);
    }

    /// <summary>
    /// 删除缓存值，并根据p_persist控制是否同时删除数据库中的值
    /// </summary>
    /// <param name="p_key">缓存键，字符串或简单值类型，推荐分段式串如：大类1:分组2:标识</param>
    /// <param name="p_persist">是否同时删除数据库中的值，默认false</param>
    /// <returns>是否成功</returns>
    public static Task<bool> DeleteCache(object p_key, bool p_persist = false)
    {
        return _info.GetDa().DeleteCache(p_key, p_persist);
    }

    /// <summary>
    /// 批量删除缓存值，并根据p_persist控制是否同时删除数据库中的值
    /// </summary>
    /// <param name="p_keys">缓存键列表，字符串或简单值类型，推荐分段式串如：大类1:分组2:标识</param>
    /// <param name="p_persist">是否同时删除数据库中的值，默认false</param>
    /// <returns>是否成功</returns>
    public static Task<bool> BatchDeleteCache(IList p_keys, bool p_persist = false)
    {
        return _info.GetDa().BatchDeleteCache(p_keys, p_persist);
    }

    /// <summary>
    /// 按照匹配模式批量删除缓存值，并根据p_persist控制是否同时删除数据库中的值
    /// </summary>
    /// <param name="p_keyStr">待匹配的键字符串</param>
    /// <param name="p_pattern">匹配模式：0以开头，1以结尾，2包含</param>
    /// <param name="p_persist">是否同时删除数据库中的值，默认false</param>
    /// <returns>是否成功</returns>
    public static Task<bool> DelCacheByPattern(string p_keyStr, int p_pattern, bool p_persist = false)
    {
        return _info.GetDa().DelCacheByPattern(p_keyStr, p_pattern, p_persist);
    }

    /// <summary>
    /// 按照匹配模式获取一级缓存(内存)的所有键列表，p_keyStr空时返回所有键
    /// </summary>
    /// <param name="p_keyStr">待匹配的键字符串，空时表示匹配所有键</param>
    /// <param name="p_pattern">匹配模式：0以开头，1以结尾，2包含</param>
    /// <returns>缓存键列表</returns>
    public static Task<List<string>> GetCacheKeys(string p_keyStr, int p_pattern)
    {
        return _info.GetDa().GetCacheKeys(p_keyStr, p_pattern);
    }
    #endregion

    #region 库信息
    /// <summary>
    /// 获取当前服务默认数据库的类型
    /// </summary>
    /// <returns></returns>
    public static Task<DatabaseType> GetDbType()
    {
        return _info.GetDa().GetDbType();
    }

    /// <summary>
    /// 获取当前服务的默认数据源键名
    /// </summary>
    /// <returns></returns>
    public static Task<string> GetDbKey()
    {
        return _info.GetDa().GetDbKey();
    }

    /// <summary>
    /// 数据访问的描述信息
    /// </summary>
    public IAccessInfo AccessInfo => _info;
    #endregion

    #region 实体写入器
    /// <summary>
    /// 创建实体写入器
    /// </summary>
    /// <returns></returns>
    public static IEntityWriter NewWriter() => new EntityWriter(_info.GetDa());
    #endregion

    #region 表结构
    /// <summary>
    /// 获取数据库所有表结构信息
    /// </summary>
    /// <returns></returns>
    public static Task<IReadOnlyDictionary<string, TableSchema>> GetDbSchema()
    {
        return _info.GetDa().GetDbSchema();
    }

    /// <summary>
    /// 获取数据库的所有表名
    /// </summary>
    /// <returns></returns>
    public static Task<List<string>> GetAllTableNames()
    {
        return _info.GetDa().GetAllTableNames();
    }

    /// <summary>
    /// 获取单个表结构信息
    /// </summary>
    /// <param name="p_tblName">表名</param>
    /// <returns></returns>
    public static Task<TableSchema> GetTableSchema(string p_tblName)
    {
        return _info.GetDa().GetTableSchema(p_tblName);
    }

    /// <summary>
    /// 同步数据库时间
    /// </summary>
    /// <returns></returns>
    public static Task SyncDbTime()
    {
        return _info.GetDa().SyncDbTime();
    }
    #endregion

    static IAccessInfo _info = new TAgentInfo().GetAccessInfo();
}
