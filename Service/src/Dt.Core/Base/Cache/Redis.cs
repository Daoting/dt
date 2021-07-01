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
    /// Redis的基础管理，外部统一用Kit 或 BaseCache 子类操作
    /// </summary>
    static class Redis
    {
        static readonly ConnectionMultiplexer _muxer;

        static Redis()
        {
            var cfg = Kit.GetCfg<string>("redis");
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
        /// 获取Redis服务器
        /// </summary>
        public static IServer Server
        {
            get { return _muxer.GetServer(_muxer.GetEndPoints(true)[0]); }
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