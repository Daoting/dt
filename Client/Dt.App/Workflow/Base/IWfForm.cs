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
    /// 流程表单接口
    /// </summary>
    public interface IWfForm
    {
        /// <summary>
        /// 任务单名称
        /// </summary>
        string GetPrcName();

        /// <summary>
        /// 保存表单数据，成功不提示，失败提示错误信息
        /// </summary>
        /// <returns></returns>
        Task<bool> Save();

        /// <summary>
        /// 删除表单数据，禁止删除或删除失败时可返回false
        /// <para>成功不提示，失败提示错误信息</para>
        /// </summary>
        /// <returns></returns>
        Task<bool> Delete();
    }
}
