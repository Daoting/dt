#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-07 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 实体对象的增删改事件类型
    /// </summary>
    public enum CudEvent
    {
        /// <summary>
        /// 不触发增删改事件
        /// </summary>
        None,

        /// <summary>
        /// 触发本地增删改事件
        /// </summary>
        Local,

        /// <summary>
        /// 触发远程增删改事件
        /// </summary>
        Remote
    }
}