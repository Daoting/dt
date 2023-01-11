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
        /// 触发插入事件
        /// </summary>
        Insert = 0x01,

        /// <summary>
        /// 触发更新事件
        /// </summary>
        Update = 0x04,

        /// <summary>
        /// 触发删除事件
        /// </summary>
        Delete = 0x10,
    }
}