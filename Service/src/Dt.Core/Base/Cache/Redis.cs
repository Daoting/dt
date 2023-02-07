#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2019-06-21 ����
******************************************************************************/
#endregion

#region ��������
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
#endregion

namespace Dt.Core.Caches
{
    /// <summary>
    /// Redis�Ļ��������ⲿͳһ��Kit �� BaseCache �������
    /// </summary>
    static class Redis
    {
        static readonly ConnectionMultiplexer _muxer;
        
        static Redis()
        {
            var cfg = Kit.GetCfg<string>("redis");
            if (string.IsNullOrEmpty(cfg))
                throw new InvalidOperationException("δ��Redis���������ã�");
            _muxer = ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(cfg));
            InitLuaScript();
        }

        /// <summary>
        /// ��ȡRedis���ݿ�
        /// </summary>
        public static IDatabase Db
        {
            get { return _muxer.GetDatabase(); }
        }

        /// <summary>
        /// ��ȡRedis������
        /// </summary>
        public static IServer Server
        {
            get { return _muxer.GetServer(_muxer.GetEndPoints(true)[0]); }
        }

        /// <summary>
        /// ����ģʽƥ������м���Ӱ�����ܣ����ã�����
        /// </summary>
        /// <param name="p_patternKey"></param>
        /// <returns></returns>
        public static IEnumerable<RedisKey> Keys(string p_patternKey)
        {
            var server = _muxer.GetServer(_muxer.GetEndPoints(true)[0]);
            return server.Keys(_muxer.GetDatabase().Database, p_patternKey);
        }

        /// <summary>
        /// �ȸ��� p_key �鵽ʵ�������ֵ���ٸ�������ǰ׺ p_priKeyPrefix ������ֵ��ѯʵ��json
        /// </summary>
        /// <param name="p_key">ʵ��������м�����������:����:��ֵ��</param>
        /// <param name="p_priKeyPrefix">��ѯʵ��json�ļ�����������:����������</param>
        /// <returns></returns>
        public static async Task<string> GetEntityJson(string p_key, string p_priKeyPrefix)
        {
            if (string.IsNullOrEmpty(p_key) || string.IsNullOrEmpty(p_priKeyPrefix))
                return null;

            // lua�ű����Ȳ鵽����ֵ����������ֵ��ѯjson
            var result = await Db.ScriptEvaluateAsync(
                _sha1LuaGet,
                new RedisKey[] { p_key },
                new RedisValue[] { p_priKeyPrefix });

            if (!result.IsNull)
                return (string)result;
            return null;
        }

        /// <summary>
        /// ���ݼ����б�����ɾ������
        /// </summary>
        /// <param name="p_keys">�����б�</param>
        /// <returns></returns>
        public static Task BatchKeyDelete(List<string> p_keys)
        {
            if (p_keys == null || p_keys.Count== 0)
                return Task.CompletedTask;

            RedisKey[] ls = (from k in p_keys
                             where !string.IsNullOrEmpty(k)
                             select (RedisKey)k).ToArray();

            // lua�ű�������ɾ��
            return Db.ScriptEvaluateAsync(_sha1LuaDel, ls);
        }

        /// <summary>
        /// ��յ�ǰ��
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

        #region lua�ű�
        // �Ȳ������ֵ���ٲ�ѯʵ��json
        const string _luaGet =
            "local id = redis.call('GET', KEYS[1])\n" +
            "if not id then\n" +
            "  return nil\n" +
            "end\n" +
            "local key = ARGV[1] .. ':' .. id\n" +
            "return redis.call('GET', key)\n";

        // ����ɾ��
        const string _luaDel =
            "for i = 1,#(KEYS) do\n" +
            "  redis.call('del', KEYS[i])\n" +
            "end\n" +
            "return true";

        static byte[] _sha1LuaGet;
        static byte[] _sha1LuaDel;

        static void InitLuaScript()
        {
            // ��������Ԥ���ؽű�
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