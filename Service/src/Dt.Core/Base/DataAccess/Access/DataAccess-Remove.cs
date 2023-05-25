#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dapper;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Dt.Core
{
    // Dapper涉及dynamic的速度非常慢！！！
    // 强类型及多映射查询作废

    //abstract class DataAccess
    //{
    //    #region 强类型查询
    //    /* 
    //     * Dapper涉及dynamic的速度非常慢！已移除
    //     * 
    //     * Dapper的ORM映射优先级：
    //     * 1. 存在和字段名称相同的属性，且属性含有setter，setter可以为private
    //     * 2. 存在和字段名称相同的变量(field)
    //     * 3. 存在和字段名称相同的属性，但无setter，直接设置到自动变量(Backing Field)
    //     * 4. 删除字段名称中的_ ，按上述方法比较是否有映射属性
    //     */

    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，返回泛型列表
    //    /// </summary>
    //    /// <typeparam name="T">ORM类型</typeparam>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <returns>返回泛型列表</returns>
    //    public async Task<List<T>> GetList<T>(string p_keyOrSql, object p_params = null)
    //    {
    //        return (List<T>)await GetListInternal<T>(p_keyOrSql, p_params, false);
    //    }

    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，返回泛型枚举，高性能
    //    /// </summary>
    //    /// <typeparam name="T">ORM类型</typeparam>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <returns>返回泛型枚举</returns>
    //    public Task<IEnumerable<T>> EachItem<T>(string p_keyOrSql, object p_params = null)
    //    {
    //        return GetListInternal<T>(p_keyOrSql, p_params, true);
    //    }

    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，返回指定类型的对象列表，可直接转型为List`T
    //    /// </summary>
    //    /// <param name="p_type">ORM类型</param>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <returns>返回指定类型的对象列表</returns>
    //    public Task<System.Collections.IEnumerable> GetList(Type p_type, string p_keyOrSql, object p_params = null)
    //    {
    //        return GetListInternal(p_type, p_keyOrSql, p_params, false);
    //    }

    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，返回指定类型对象的枚举，高性能
    //    /// </summary>
    //    /// <param name="p_type">ORM类型</param>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <returns>返回指定类型对象的枚举</returns>
    //    public Task<System.Collections.IEnumerable> EachItem(Type p_type, string p_keyOrSql, object p_params = null)
    //    {
    //        return GetListInternal(p_type, p_keyOrSql, p_params, true);
    //    }

    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，只返回第一行数据
    //    /// </summary>
    //    /// <typeparam name="T">ORM类型</typeparam>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <returns>返回第一行数据对象，无数据时返回空</returns>
    //    public async Task<T> GetItem<T>(string p_keyOrSql, object p_params = null)
    //    {
    //        return (T)await GetItem(typeof(T), p_keyOrSql, p_params);
    //    }

    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，只返回第一行数据
    //    /// </summary>
    //    /// <param name="p_tgtType">ORM类型</param>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <returns>返回第一行数据对象，无数据时返回空</returns>
    //    public async Task<object> GetItem(Type p_tgtType, string p_keyOrSql, object p_params = null)
    //    {
    //        var cmd = CreateCommand(p_keyOrSql, p_params, false);
    //        try
    //        {
    //            await OpenConnection();
    //            return await _conn.QueryFirstOrDefaultAsync(p_tgtType, cmd);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw GetSqlException(cmd, ex);
    //        }
    //        finally
    //        {
    //            ReleaseConnection();
    //        }
    //    }

    //    async Task<IEnumerable<T>> GetListInternal<T>(string p_keyOrSql, object p_params, bool p_deferred)
    //    {
    //        var cmd = CreateCommand(p_keyOrSql, p_params, p_deferred);
    //        try
    //        {
    //            await OpenConnection();
    //            return await _conn.QueryAsync<T>(cmd);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw GetSqlException(cmd, ex);
    //        }
    //        finally
    //        {
    //            ReleaseConnection();
    //        }
    //    }

    //    // 泛型方法 QueryAsync<T>
    //    static MethodInfo _queryMethod = typeof(SqlMapper).GetMethod("QueryAsync", 1, new Type[] { typeof(IDbConnection), typeof(CommandDefinition) });
    //    async Task<System.Collections.IEnumerable> GetListInternal(Type p_type, string p_keyOrSql, object p_params, bool p_deferred)
    //    {
    //        if (p_type == null)
    //            throw new ArgumentNullException(nameof(p_type));

    //        var cmd = CreateCommand(p_keyOrSql, p_params, p_deferred);
    //        try
    //        {
    //            await OpenConnection();
    //            // 实例化泛型方法 QueryAsync<T>，若调用 QueryAsync 返回 IEnumerable<object>，外部还需要类型转换！
    //            Task task = _queryMethod.MakeGenericMethod(p_type).Invoke(null, new object[] { _conn, cmd }) as Task;
    //            await task;
    //            return task.GetType().GetProperty("Result").GetValue(task) as System.Collections.IEnumerable;
    //            //return await _conn.QueryAsync(p_type, cmd);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw GetSqlException(cmd, ex);
    //        }
    //        finally
    //        {
    //            ReleaseConnection();
    //        }
    //    }
    //    #endregion

    //    #region 多映射查询
    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 列表
    //    /// </summary>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_map">合并结果的回调方法</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
    //    /// <returns>返回指定类型的对象列表</returns>
    //    public async Task<List<TReturn>> Map<TFirst, TSecond, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TReturn> p_map, object p_params = null, string p_splitOn = "id")
    //    {
    //        return (List<TReturn>)await QueryMap<TFirst, TSecond, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, false);
    //    }

    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 的枚举，高性能
    //    /// </summary>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_map">合并结果的回调方法</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
    //    /// <returns>返回指定类型的对象枚举</returns>
    //    public Task<IEnumerable<TReturn>> ForEach<TFirst, TSecond, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TReturn> p_map, object p_params = null, string p_splitOn = "id")
    //    {
    //        return QueryMap<TFirst, TSecond, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, true);
    //    }

    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 列表
    //    /// </summary>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_map">合并结果的回调方法</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
    //    /// <returns>返回指定类型的对象列表</returns>
    //    public async Task<List<TReturn>> Map<TFirst, TSecond, TThird, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TReturn> p_map, object p_params = null, string p_splitOn = "id")
    //    {
    //        return (List<TReturn>)await QueryMap<TFirst, TSecond, TThird, DontMap, DontMap, DontMap, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, false);
    //    }

    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 的枚举，高性能
    //    /// </summary>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_map">合并结果的回调方法</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
    //    /// <returns>返回指定类型的对象枚举</returns>
    //    public Task<IEnumerable<TReturn>> ForEach<TFirst, TSecond, TThird, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TReturn> p_map, object p_params = null, string p_splitOn = "id")
    //    {
    //        return QueryMap<TFirst, TSecond, TThird, DontMap, DontMap, DontMap, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, true);
    //    }

    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 列表
    //    /// </summary>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_map">合并结果的回调方法</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
    //    /// <returns>返回指定类型的对象列表</returns>
    //    public async Task<List<TReturn>> Map<TFirst, TSecond, TThird, TFourth, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TFourth, TReturn> p_map, object p_params = null, string p_splitOn = "id")
    //    {
    //        return (List<TReturn>)await QueryMap<TFirst, TSecond, TThird, TFourth, DontMap, DontMap, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, false);
    //    }

    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 的枚举，高性能
    //    /// </summary>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_map">合并结果的回调方法</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
    //    /// <returns>返回指定类型的对象枚举</returns>
    //    public Task<IEnumerable<TReturn>> ForEach<TFirst, TSecond, TThird, TFourth, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TFourth, TReturn> p_map, object p_params = null, string p_splitOn = "id")
    //    {
    //        return QueryMap<TFirst, TSecond, TThird, TFourth, DontMap, DontMap, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, true);
    //    }

    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 列表
    //    /// </summary>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_map">合并结果的回调方法</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
    //    /// <returns>返回指定类型的对象列表</returns>
    //    public async Task<List<TReturn>> Map<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> p_map, object p_params = null, string p_splitOn = "id")
    //    {
    //        return (List<TReturn>)await QueryMap<TFirst, TSecond, TThird, TFourth, TFifth, DontMap, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, false);
    //    }

    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 的枚举，高性能
    //    /// </summary>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_map">合并结果的回调方法</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
    //    /// <returns>返回指定类型的对象枚举</returns>
    //    public Task<IEnumerable<TReturn>> ForEach<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> p_map, object p_params = null, string p_splitOn = "id")
    //    {
    //        return QueryMap<TFirst, TSecond, TThird, TFourth, TFifth, DontMap, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, true);
    //    }

    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 列表
    //    /// </summary>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_map">合并结果的回调方法</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
    //    /// <returns>返回指定类型的对象列表</returns>
    //    public async Task<List<TReturn>> Map<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> p_map, object p_params = null, string p_splitOn = "id")
    //    {
    //        return (List<TReturn>)await QueryMap<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, false);
    //    }

    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 的枚举，高性能
    //    /// </summary>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_map">合并结果的回调方法</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
    //    /// <returns>返回指定类型的对象枚举</returns>
    //    public Task<IEnumerable<TReturn>> ForEach<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> p_map, object p_params = null, string p_splitOn = "id")
    //    {
    //        return QueryMap<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, DontMap, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, true);
    //    }

    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 列表
    //    /// </summary>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_map">合并结果的回调方法</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
    //    /// <returns>返回指定类型的对象列表</returns>
    //    public async Task<List<TReturn>> Map<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> p_map, object p_params = null, string p_splitOn = "id")
    //    {
    //        return (List<TReturn>)await QueryMap<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, false);
    //    }

    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 的枚举，高性能
    //    /// </summary>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_map">合并结果的回调方法</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
    //    /// <returns>返回指定类型的对象枚举</returns>
    //    public Task<IEnumerable<TReturn>> ForEach<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string p_keyOrSql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> p_map, object p_params = null, string p_splitOn = "id")
    //    {
    //        return QueryMap<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(p_keyOrSql, p_map, p_params, p_splitOn, true);
    //    }

    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 列表
    //    /// </summary>
    //    /// <typeparam name="TReturn"></typeparam>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_types">返回的多个类型</param>
    //    /// <param name="p_map">合并结果的回调方法</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
    //    /// <returns>返回指定类型的对象列表</returns>
    //    public async Task<List<TReturn>> Map<TReturn>(string p_keyOrSql, Type[] p_types, Func<object[], TReturn> p_map, object p_params = null, string p_splitOn = "id")
    //    {
    //        return (List<TReturn>)await QueryMap(p_keyOrSql, p_types, p_map, p_params, false, p_splitOn);
    //    }

    //    /// <summary>
    //    /// 以参数值方式执行Sql语句，返回多个类型对象通过回调合并成 TReturn 的枚举，高性能
    //    /// </summary>
    //    /// <typeparam name="TReturn"></typeparam>
    //    /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
    //    /// <param name="p_types">返回的多个类型</param>
    //    /// <param name="p_map">合并结果的回调方法</param>
    //    /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
    //    /// <param name="p_splitOn">映射到多个对象的切分列名，支持一个或多个切分列，多个时用逗号隔开</param>
    //    /// <returns>返回指定类型的对象枚举</returns>
    //    public Task<IEnumerable<TReturn>> ForEach<TReturn>(string p_keyOrSql, Type[] p_types, Func<object[], TReturn> p_map, object p_params = null, string p_splitOn = "id")
    //    {
    //        return QueryMap(p_keyOrSql, p_types, p_map, p_params, true, p_splitOn);
    //    }

    //    async Task<IEnumerable<TReturn>> QueryMap<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
    //        string p_keyOrSql,
    //        Delegate p_map,
    //        object p_params,
    //        string p_splitOn,
    //        bool p_deferred)
    //    {
    //        if (p_map == null)
    //            throw new ArgumentNullException(nameof(p_map));

    //        var cmd = CreateCommand(p_keyOrSql, p_params, p_deferred);
    //        try
    //        {
    //            await OpenConnection();
    //            IEnumerable<TReturn> result = null;
    //            if (typeof(TThird) == typeof(DontMap))
    //                result = await _conn.QueryAsync(cmd, (Func<TFirst, TSecond, TReturn>)p_map, p_splitOn);
    //            else if (typeof(TFourth) == typeof(DontMap))
    //                result = await _conn.QueryAsync(cmd, (Func<TFirst, TSecond, TThird, TReturn>)p_map, p_splitOn);
    //            else if (typeof(TFifth) == typeof(DontMap))
    //                result = await _conn.QueryAsync(cmd, (Func<TFirst, TSecond, TThird, TFourth, TReturn>)p_map, p_splitOn);
    //            else if (typeof(TSixth) == typeof(DontMap))
    //                result = await _conn.QueryAsync(cmd, (Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>)p_map, p_splitOn);
    //            else if (typeof(TSeventh) == typeof(DontMap))
    //                result = await _conn.QueryAsync(cmd, (Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>)p_map, p_splitOn);
    //            else
    //                result = await _conn.QueryAsync(cmd, (Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>)p_map, p_splitOn);
    //            return result;
    //        }
    //        catch (Exception ex)
    //        {
    //            throw GetSqlException(cmd, ex);
    //        }
    //        finally
    //        {
    //            ReleaseConnection();
    //        }
    //    }

    //    async Task<IEnumerable<TReturn>> QueryMap<TReturn>(
    //        string p_keyOrSql,
    //        Type[] p_types,
    //        Func<object[], TReturn> p_map,
    //        object p_params,
    //        bool p_deferred,
    //        string p_splitOn)
    //    {
    //        string sql = Kit.Sql(p_keyOrSql);

    //        try
    //        {
    //            await OpenConnection();
    //            var result = await _conn.QueryAsync(sql, p_types, p_map, p_params, _tran, !p_deferred, p_splitOn);
    //            return result;
    //        }
    //        catch (Exception ex)
    //        {
    //            throw GetSqlException(CreateCommand(p_keyOrSql, p_params, p_deferred), ex);
    //        }
    //        finally
    //        {
    //            ReleaseConnection();
    //        }
    //    }

    //    class DontMap { }
    //    #endregion

    //}
}