#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 实体对象的增删改事件类型
    /// </summary>
    [Flags]
    public enum CudEvent
    {
        /// <summary>
        /// 不触发增删改事件
        /// </summary>
        None = 0x00,

        /// <summary>
        /// 触发本地插入事件
        /// </summary>
        LocalInsert = 0x01,

        /// <summary>
        /// 触发远程插入事件
        /// </summary>
        RemoteInsert = 0x02,

        /// <summary>
        /// 触发本地更新事件
        /// </summary>
        LocalUpdate = 0x04,

        /// <summary>
        /// 触发远程更新事件
        /// </summary>
        RemoteUpdate = 0x08,

        /// <summary>
        /// 触发本地删除事件
        /// </summary>
        LocalDelete = 0x10,

        /// <summary>
        /// 触发远程删除事件
        /// </summary>
        RemoteDelete = 0x20,
    }
}