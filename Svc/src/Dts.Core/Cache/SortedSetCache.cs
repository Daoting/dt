#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2019-06-21 ����
******************************************************************************/
#endregion

#region ��������
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Cache
{
    /// <summary>
    /// ֵΪ��Ȩ�ز���������ַ������򼯺ϵĻ������
    /// </summary>
    /// <typeparam name="TCacheItem">�������ͣ�����Ϊ��������</typeparam>
    public class SortedSetCache<TCacheItem> : BaseCache
    {
        public SortedSetCache(string p_keyPrefix)
            : base(p_keyPrefix)
        {
        }
    }
}