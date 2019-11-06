#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.EventBus;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Domain
{
    /// <summary>
    /// 聚合根类接口，包含"ID"主键
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    public interface IRoot<TKey> : IEntity<TKey>
    {
        /// <summary>
        /// 启动跟踪属性值变化，Update时只更新变化的列
        /// </summary>
        void StartTrack();

        /// <summary>
        /// 是否已启动跟踪
        /// </summary>
        /// <returns></returns>
        bool IsTracking();

        /// <summary>
        /// 判断属性值是否和原始值相同
        /// </summary>
        /// <param name="p_propName">属性名</param>
        /// <param name="p_val">当前值</param>
        /// <returns></returns>
        bool IsOriginalVal(string p_propName, object p_val);

        /// <summary>
        /// 停止跟踪
        /// </summary>
        public void StopTrack();

        IReadOnlyCollection<IEvent> GetLocalEvents();
        IReadOnlyCollection<IEvent> GetRemoteEvents();
        void ClearLocalEvents();
        void ClearRemoteEvents();
    }
}
