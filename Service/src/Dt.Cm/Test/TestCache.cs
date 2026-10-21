#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-30 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Cm;

/// <summary>
/// 功能测试Api
/// </summary>
[Api(IsTest = true)]
public class TestCache : RpcApi
{
    public async Task<object> SetGetCache(object p_key, object p_value, bool p_persist)
    {
        await _da.SetCache(p_key, p_value, p_persist);
        return await _da.GetCache<object>(p_key, p_persist);
    }
    
    public async Task<string> CacheStr(string p_key, string p_val, bool p_persist)
    {
        await _da.SetCache(p_key, p_val, p_persist);
        return await _da.GetCache<string>(p_key);
    }

    public async Task<long> CacheLong(string p_key, long p_val, bool p_persist)
    {
        await _da.SetCache(p_key, p_val, p_persist);
        return await _da.GetCache<long>(p_key);
    }

    public async Task<Table> CacheTable(string p_key, Table p_val, bool p_persist)
    {
        await _da.SetCache(p_key, p_val, p_persist);
        return await _da.GetCache<Table>(p_key);
    }

    public async Task<Dict> CacheDict(string p_key, Dict p_val, bool p_persist)
    {
        await _da.SetCache(p_key, p_val, p_persist);
        return await _da.GetCache<Dict>(p_key);
    }
    
}
