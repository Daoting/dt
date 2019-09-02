#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-21 创建
******************************************************************************/
#endregion

#region 引用命名
using StackExchange.Redis;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Core.Caches
{
    /// <summary>
    /// Redis的基础管理
    /// </summary>
    public static class Redis
    {
        static readonly ConnectionMultiplexer _muxer;

        static Redis()
        {
            var cfg = Glb.GetCfg<string>("redis");
            if (string.IsNullOrEmpty(cfg))
                throw new InvalidOperationException("未找Redis的连接配置！");
            _muxer = ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(cfg));
        }

        /// <summary>
        /// 获取Redis数据库
        /// </summary>
        public static IDatabase Db
        {
            get { return _muxer.GetDatabase(); }
        }

        /// <summary>
        /// 在库中删除指定前缀的所有键值
        /// </summary>
        /// <param name="p_db"></param>
        /// <param name="p_prefix">缓存键前缀</param>
        public static void KeyDeleteWithPrefix(this IDatabase p_db, string p_prefix)
        {
            if (p_db == null || string.IsNullOrWhiteSpace(p_prefix))
                return;

            p_db.ScriptEvaluate(@"
                local keys = redis.call('keys', ARGV[1]) 
                for i=1,#keys,5000 do 
                redis.call('del', unpack(keys, i, math.min(i+4999, #keys)))
                end", values: new RedisValue[] { p_prefix + ":*" });
        }

        /// <summary>
        /// 统计具有指定前缀的行数
        /// </summary>
        /// <param name="p_db"></param>
        /// <param name="p_prefix"></param>
        /// <returns></returns>
        public static int KeyCount(this IDatabase p_db, string p_prefix)
        {
            if (p_db == null || string.IsNullOrWhiteSpace(p_prefix))
                return 0;

            var retVal = p_db.ScriptEvaluate("return table.getn(redis.call('keys', ARGV[1]))", values: new RedisValue[] { p_prefix + ":*" });
            if (retVal.IsNull)
                return 0;
            return (int)retVal;
        }

        /// <summary>
        /// 返回模式匹配的所有键，影响性能，慎用！！！
        /// </summary>
        /// <param name="p_patternKey"></param>
        /// <returns></returns>
        public static IEnumerable<RedisKey> Keys(string p_patternKey)
        {
            var server = _muxer.GetServer(_muxer.GetEndPoints(true)[0]);
            return server.Keys(_muxer.GetDatabase().Database, p_patternKey);
        }

        /// <summary>
        /// 清空当前库
        /// </summary>
        internal static void ClearDb()
        {
            var db = _muxer.GetDatabase();
            var endpoints = _muxer.GetEndPoints(true);
            foreach (var endpoint in endpoints)
            {
                var server = _muxer.GetServer(endpoint);
                server.FlushDatabase(db.Database);
            }
        }
    }
}