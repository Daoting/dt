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
#endregion

namespace Dt.Core.Caches
{
    /// <summary>
    /// Redis�Ļ�������
    /// </summary>
    public static class Redis
    {
        static readonly ConnectionMultiplexer _muxer;

        static Redis()
        {
            var cfg = Glb.GetCfg<string>("redis");
            if (string.IsNullOrEmpty(cfg))
                throw new InvalidOperationException("δ��Redis���������ã�");
            _muxer = ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(cfg));
        }

        /// <summary>
        /// ��ȡRedis���ݿ�
        /// </summary>
        public static IDatabase Db
        {
            get { return _muxer.GetDatabase(); }
        }

        /// <summary>
        /// �ڿ���ɾ��ָ��ǰ׺�����м�ֵ
        /// </summary>
        /// <param name="p_db"></param>
        /// <param name="p_prefix">�����ǰ׺</param>
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
        /// ͳ�ƾ���ָ��ǰ׺������
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
    }
}