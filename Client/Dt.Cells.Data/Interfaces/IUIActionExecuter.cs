#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// An interface that executes code from the UI.
    /// </summary>
    public interface IUIActionExecuter
    {
        /// <summary>
        /// Begins the UI action.
        /// </summary>
        /// <returns></returns>
        IDisposable BeginUIAction();
    }
}

