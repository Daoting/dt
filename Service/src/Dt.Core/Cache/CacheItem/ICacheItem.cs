#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-31 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Caches
{
    /// <summary>
    /// 缓存项标志
    /// </summary>
    public interface ICacheItem
    {
        /// <summary>
        /// 缓存中没有从db查询后调用，用来加载附加数据
        /// </summary>
        /// <param name="p_db"></param>
        /// <returns></returns>
        Task Init(Db p_db)
        {
            return Task.CompletedTask;
        }
    }
}
