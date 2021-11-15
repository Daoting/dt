#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-24 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.App.Workflow;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.App
{
    /// <summary>
    /// 流程表单的使用场景
    /// </summary>
    public enum WfFormUsage
    {
        /// <summary>
        /// 填写发送表单
        /// </summary>
        Edit,

        /// <summary>
        /// 浏览表单
        /// </summary>
        Read,

        /// <summary>
        /// 管理表单
        /// </summary>
        Manage
    }
}
