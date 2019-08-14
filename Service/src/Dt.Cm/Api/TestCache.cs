#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Cache;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 功能测试Api
    /// </summary>
    [Api(true, "功能测试", AgentMode.Generic)]
    public class TestCache : BaseApi
    {
        public Task CacheStr(string p_key, string p_val)
        {
            return new StringCache("Test:Str").Set(p_key, p_val);
        }

        public Task CacheLong(string p_key, long p_val)
        {
            return new StringCache("Test:Long").Set(p_key, p_val);
        }

        public Task CacheStrObj(string p_key, string p_name, int p_age)
        {
            return new StringCache("Test:StrObj").Set(p_key, new TestCacheObject { Name = p_name, Age = p_age });
        }

        public Task CacheExpiry(string p_key, string p_val, int p_expiry)
        {
            return new StringCache("Test:Str").Set(p_key, p_val, p_expiry > 0 ? TimeSpan.FromSeconds(p_expiry) : (TimeSpan?)null);
        }

        public Task BatchCacheStr()
        {
            return Task.CompletedTask;
        }

        public Task<string> GetStr(string p_key)
        {
            return new StringCache("Test:Str").Get<string>(p_key);
        }

        public Task<long> GetLong(string p_key)
        {
            return new StringCache("Test:Long").Get<long>(p_key);
        }

        public async Task<string> GetObjName(string p_key)
        {
            var obj = await new StringCache("Test:StrObj").Get<TestCacheObject>(p_key);
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
            return new HashCache("Test:Hash").Set(p_key, new TestCacheObject { Name = p_name, Age = p_age });
        }

        public async Task<string> GetHash(string p_key)
        {
            var obj = await new HashCache("Test:Hash").Get<TestCacheObject>(p_key);
            if (obj != null)
                return obj.Name;
            return null;
        }

        public async Task<int> GetHashAge(string p_key)
        {
            return await new HashCache("Test:Hash").GetField<int>(p_key, "Age");
        }

        public async void SetHashAge(string p_key, int p_age)
        {
            await new HashCache("Test:Hash").SetField(p_key, "Age", p_age);
        }
    }

    public class TestCacheObject
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }
}
