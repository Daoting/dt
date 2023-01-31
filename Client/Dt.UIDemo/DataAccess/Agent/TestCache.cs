namespace Dt.UIDemo
{
    /// <summary>
    /// 功能测试Api
    /// </summary>
    public partial class AtTestCm
    {
        public static Task<string> CacheStr(string p_key, string p_val)
        {
            return Kit.Rpc<string>(
                "cm",
                "TestCache.CacheStr",
                p_key,
                p_val
            );
        }

        public static Task<long> CacheLong(string p_key, long p_val)
        {
            return Kit.Rpc<long>(
                "cm",
                "TestCache.CacheLong",
                p_key,
                p_val
            );
        }

        public static Task CacheStrObj(string p_key, string p_name, int p_age)
        {
            return Kit.Rpc<object>(
                "cm",
                "TestCache.CacheStrObj",
                p_key,
                p_name,
                p_age
            );
        }

        public static Task CacheExpiry(string p_key, string p_val, int p_expiry)
        {
            return Kit.Rpc<object>(
                "cm",
                "TestCache.CacheExpiry",
                p_key,
                p_val,
                p_expiry
            );
        }

        public static Task BatchCacheStr()
        {
            return Kit.Rpc<object>(
                "cm",
                "TestCache.BatchCacheStr"
            );
        }

        public static Task<string> GetStr(string p_key)
        {
            return Kit.Rpc<string>(
                "cm",
                "TestCache.GetStr",
                p_key
            );
        }

        public static Task<long> GetLong(string p_key)
        {
            return Kit.Rpc<long>(
                "cm",
                "TestCache.GetLong",
                p_key
            );
        }

        public static Task<string> GetObjName(string p_key)
        {
            return Kit.Rpc<string>(
                "cm",
                "TestCache.GetObjName",
                p_key
            );
        }

        public static Task ClearStrCache()
        {
            return Kit.Rpc<object>(
                "cm",
                "TestCache.ClearStrCache"
            );
        }

        public static Task CacheHash(string p_key, string p_name, int p_age)
        {
            return Kit.Rpc<object>(
                "cm",
                "TestCache.CacheHash",
                p_key,
                p_name,
                p_age
            );
        }

        public static Task<string> GetHash(string p_key)
        {
            return Kit.Rpc<string>(
                "cm",
                "TestCache.GetHash",
                p_key
            );
        }

        public static Task<int> GetHashAge(string p_key)
        {
            return Kit.Rpc<int>(
                "cm",
                "TestCache.GetHashAge",
                p_key
            );
        }

        public static Task SetHashAge(string p_key, int p_age)
        {
            return Kit.Rpc<object>(
                "cm",
                "TestCache.SetHashAge",
                p_key,
                p_age
            );
        }
    }
}
