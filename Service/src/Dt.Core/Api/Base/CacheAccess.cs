#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.Caching.Memory;
#endregion

namespace Dt.Core;

/// <summary>
/// 缓存的访问Api
/// </summary>
[Api(AgentMode = AgentMode.Generic)]
public class CacheAccess : RpcApi
{
    
    public async Task<T> GetCache<T>(object p_key, bool p_persist)
    {
        if (Kit.Cache.TryGetValue(p_key, out T val))
            return val;
        return default;
    }

    public bool SetCache(object p_key, object p_value, double p_seconds)
    {
        var option = new MemoryCacheEntryOptions { Size = 1 };
        if (p_seconds > 0)
        {
            option.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(p_seconds);
        }
        Kit.Cache.Set(p_key, p_value, option);
        return true;
    }

    public bool DeleteCache(object p_key)
    {
        return true;
    }
    
}
