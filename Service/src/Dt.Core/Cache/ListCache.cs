#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Cache
{
    /// <summary>
    /// 值为值为按插入顺序排序的字符串列表的缓存基类
    /// </summary>
    /// <typeparam name="TCacheItem">缓存类型，可以为任意类型</typeparam>
    public class ListCache<TCacheItem> : BaseCache
    {
        public ListCache(string p_keyPrefix)
            : base(p_keyPrefix)
        {
        }
    }
}