#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-24 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.App.Workflow;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.App
{
    /// <summary>
    /// 发送前事件参数
    /// </summary>
    public class WfSendingArgs : AsyncEventArgs
    {
        public WfSendingArgs(AtvRecvs p_recvs)
        {
            Recvs = p_recvs;
        }

        /// <summary>
        /// 获取后续活动相关内容
        /// </summary>
        public AtvRecvs Recvs { get; }
    }
}
