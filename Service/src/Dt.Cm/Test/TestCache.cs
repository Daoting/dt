#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Caches;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 功能测试Api
    /// </summary>
    [Api(GroupName = "功能测试")]
    public class TestCache : BaseApi
    {
        public async Task<string> CacheStr(string p_key, string p_val)
        {
            var cache = new StringCache("Test:Str");
            await cache.Set(p_key, p_val);
            return await cache.Get<string>(p_key);
        }

        public async Task<long> CacheLong(string p_key, long p_val)
        {
            var cache = new StringCache("Test:Long");
            await cache.Set(p_key, p_val);
            return await cache.Get<long>(p_key);
        }

        public async Task CacheStrObj(string p_key, string p_name, int p_age)
        {
            var cache = new StringCache("Test:StrObj");
            await cache.Set(p_key, new TestCacheObject { Name = p_name, Age = p_age });
            var obj = await cache.Get<TestCacheObject>(p_key);
            if (obj != null && !string.IsNullOrEmpty(obj.Name))
                _log.Information(obj.Name);
        }

        public Task CacheExpiry(string p_key, string p_val, int p_expiry)
        {
            return Kit.StringSet("Test:Str", p_key, p_val, p_expiry > 0 ? TimeSpan.FromSeconds(p_expiry) : (TimeSpan?)null);
        }

        public Task BatchCacheStr()
        {
            return Task.CompletedTask;
        }

        public Task<string> GetStr(string p_key)
        {
            return Kit.StringGet<string>("Test:Str", p_key);
        }

        public Task<long> GetLong(string p_key)
        {
            return Kit.StringGet<long>("Test:Long", p_key);
        }

        public async Task<string> GetObjName(string p_key)
        {
            var obj = await Kit.StringGet<TestCacheObject>("Test:StrObj", p_key);
            if (obj != null)
                return obj.Name;
            return null;
        }

        public void ClearStrCache()
        {
            var cache = new StringCache("Test:Str");
            int cnt = cache.Count();
            if (cnt > 0)
                cache.Clear();
        }

        public Task CacheHash(string p_key, string p_name, int p_age)
        {
            return Kit.HashSet("Test:Hash", p_key, new TestCacheObject { Name = p_name, Age = p_age });
        }

        public async Task<string> GetHash(string p_key)
        {
            var obj = await Kit.HashGet<TestCacheObject>("Test:Hash", p_key);
            if (obj != null)
                return obj.Name;
            return null;
        }

        public Task<int> GetHashAge(string p_key)
        {
            return Kit.HashGetField<int>("Test:Hash", p_key, "Age");
        }

        public Task SetHashAge(string p_key, int p_age)
        {
            return Kit.HashSetField("Test:Hash", p_key, "Age", p_age);
        }
    }

    public class TestCacheObject
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }
}
