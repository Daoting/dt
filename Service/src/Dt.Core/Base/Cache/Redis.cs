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