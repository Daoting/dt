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

namespace Dt.Core.Cache
{
    /// <summary>
    /// ֵΪֵΪ������˳��������ַ����б�Ļ������
    /// </summary>
    /// <typeparam name="TCacheItem">�������ͣ�����Ϊ��������</typeparam>
    public class ListCache<TCacheItem> : BaseCache
    {
        public ListCache(string p_keyPrefix)
            : base(p_keyPrefix)
        {
        }
    }
}