#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 视图参数编辑器
    /// </summary>
    public interface IViewParamsEditor
    {
        /// <summary>
        /// 显示视图参数编辑器
        /// </summary>
        /// <param name="p_params">初始参数串</param>
        /// <returns>编辑后的参数串</returns>
        Task<string> ShowDlg(string p_params);
    }
}