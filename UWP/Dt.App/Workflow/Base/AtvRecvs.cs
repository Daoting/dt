#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-07-08 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.App.Workflow;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#endregion

namespace Dt.App
{
    /// <summary>
    /// 后续活动列表
    /// </summary>
    public class AtvRecvs
    {
        /// <summary>
        /// 普遍活动的后续活动列表
        /// </summary>
        public List<AtvRecv> Atvs { get; } = new List<AtvRecv>();

        /// <summary>
        /// 同步活动的后续活动
        /// </summary>
        public AtvSyncRecv SyncAtv { get; set; }

        /// <summary>
        /// 结束活动
        /// </summary>
        public AtvFinishedRecv FinishedAtv { get; set; }

        /// <summary>
        /// 后续活动数
        /// </summary>
        public int AtvCount
        {
            get { return Atvs.Count + (SyncAtv == null ? 0 : 1) + (FinishedAtv == null ? 0 : 1); }
        }
    }
}
