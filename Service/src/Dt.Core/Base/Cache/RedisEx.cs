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
    /// Redis��չ��
    /// </summary>
    public static class RedisEx
    {
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
        /// ��HashEntry[]����תΪ�ڲ�����Dict
        /// </summary>
        /// <param name="p_arr"></param>
        /// <returns></returns>
        public static Dict ToDict(this HashEntry[] p_arr)
        {
            if (p_arr == null)
                return null;

            Dict dt = new Dict(p_arr.Length);
            for (int i = 0; i < p_arr.Length; i++)
            {
                var v = p_arr[i];
                dt[v.Name] = v.Value.HasValue ? v.Value.ToString() : "";
            }
            return dt;
        }
    }
}