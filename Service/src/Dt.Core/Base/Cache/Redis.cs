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
using System.Security.Cryptography;
using System.Text;
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
            InitLuaScript();
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
        /// 先根据 p_key 查到实体的主键值，再根据主键前缀 p_priKeyPrefix 和主键值查询实体json
        /// </summary>
        /// <param name="p_key">实体非主键列键名：“表名:列名:列值”</param>
        /// <param name="p_priKeyPrefix">查询实体json的键名：“表名:主键列名”</param>
        /// <returns></returns>
        public static async Task<string> GetEntityJson(string p_key, string p_priKeyPrefix)
        {
            if (string.IsNullOrEmpty(p_key) || string.IsNullOrEmpty(p_priKeyPrefix))
                return null;

            // lua脚本：先查到主键值，再用主键值查询json
            var result = await Db.ScriptEvaluateAsync(
                _sha1LuaGet,
                new RedisKey[] { p_key },
                new RedisValue[] { p_priKeyPrefix });

            if (!result.IsNull)
                return (string)result;
            return null;
        }

        /// <summary>
        /// 根据键名列表批量删除缓存
        /// </summary>
        /// <param name="p_keys">键名列表</param>
        /// <returns></returns>
        public static Task BatchKeyDelete(List<string> p_keys)
        {
            if (p_keys == null || p_keys.Count== 0)
                return Task.CompletedTask;

            RedisKey[] ls = (from k in p_keys
                             where !string.IsNullOrEmpty(k)
                             select (RedisKey)k).ToArray();

            // lua脚本：批量删除
            return Db.ScriptEvaluateAsync(_sha1LuaDel, ls);
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

        #region lua脚本
        // 先查出主键值，再查询实体json
        const string _luaGet =
            "local id = redis.call('GET', KEYS[1])\n" +
            "if not id then\n" +
            "  return nil\n" +
            "end\n" +
            "local key = ARGV[1] .. ':' .. id\n" +
            "return redis.call('GET', key)\n";

        // 批量删除
        const string _luaDel =
            "for i = 1,#(KEYS) do\n" +
            "  redis.call('del', KEYS[i])\n" +
            "end\n" +
            "return true";

        static byte[] _sha1LuaGet;
        static byte[] _sha1LuaDel;

        static void InitLuaScript()
        {
            // 服务器上预加载脚本
            var server = Server;
            _sha1LuaGet = CalcLuaSha1(_luaGet);
            if (!server.ScriptExists(_sha1LuaGet))
                _sha1LuaGet = server.ScriptLoad(_luaGet);

            _sha1LuaDel = CalcLuaSha1(_luaDel);
            if (!server.ScriptExists(_sha1LuaDel))
                _sha1LuaDel = server.ScriptLoad(_luaDel);
        }

        static byte[] CalcLuaSha1(string p_lua)
        {
            SHA1 sha1 = SHA1.Create();
            var bytesSha1In = Encoding.UTF8.GetBytes(p_lua);
            return sha1.ComputeHash(bytesSha1In);
        }
        #endregion
    }
}