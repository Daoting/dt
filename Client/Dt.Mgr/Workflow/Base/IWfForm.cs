#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-24 创建
**************************************************************************/
#endregion

#region 命名空间
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 流程表单接口
    /// </summary>
    public interface IWfForm
    {
        /// <summary>
        /// 表单对应的Info
        /// </summary>
        WfFormInfo Info { get; set; }

        /// <summary>
        /// 保存表单数据
        /// </summary>
        /// <returns></returns>
        Task<bool> OnSave();

        /// <summary>
        /// 执行发送
        /// </summary>
        /// <returns></returns>
        Task<bool> OnSend();

        /// <summary>
        /// 删除表单数据，禁止删除或删除失败时可返回false
        /// </summary>
        /// <returns></returns>
        Task<bool> OnDelete();

        /// <summary>
        /// 任务单名称
        /// </summary>
        string GetPrcName();
    }
}
