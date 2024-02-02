#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-02-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Collections.Generic;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 待离线推送项
    /// </summary>
    public class OfflineItem
    {
        /// <summary>
        /// 待推送的用户列表
        /// </summary>
        public List<long> Users { get; set; }

        /// <summary>
        /// 待推送内容
        /// </summary>
        public MsgInfo Msg { get; set; }
    }
}
