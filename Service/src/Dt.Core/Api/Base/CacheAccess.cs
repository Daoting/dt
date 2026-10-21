#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core;

/// <summary>
/// 缓存的访问Api
/// </summary>
[Api(AgentMode = AgentMode.Generic)]
public class CacheAccess : RpcApi
{
    public Task<object> GetCache(object p_key, bool p_persist)
    {
        return _da.GetCache<object>(p_key, p_persist);
    }
    
    public Task<bool> SetCache(object p_key, object p_value, bool p_persist = false)
    {
        return _da.SetCache(p_key, p_value, p_persist);
    }

    public Task<bool> SetExpireCache(object p_key, object p_value, double p_seconds)
    {
        return _da.SetExpireCache(p_key, p_value, p_seconds);
    }

    public Task<bool> DeleteCache(object p_key, bool p_persist = false)
    {
        return _da.DeleteCache(p_key, p_persist);
    }

    public Task<bool> BatchDeleteCache(List<object> p_keys, bool p_persist = false)
    {
        return _da.BatchDeleteCache(p_keys, p_persist);
    }
}
