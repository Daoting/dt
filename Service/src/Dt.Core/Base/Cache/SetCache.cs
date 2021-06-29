#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-21 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core.Caches
{
    /// <summary>
    /// 值为无序字符串集合的缓存基类
    /// </summary>
    /// <typeparam name="TCacheItem">缓存类型，可以为任意类型</typeparam>
    public class SetCache<TCacheItem> : BaseCache
    {
        public SetCache(string p_keyPrefix)
            : base(p_keyPrefix)
        {
        }
    }
}