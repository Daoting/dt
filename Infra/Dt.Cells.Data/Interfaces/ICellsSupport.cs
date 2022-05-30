#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Interface that supports cell, column, and row operations.
    /// </summary>
    public interface ICellsSupport
    {
        /// <summary>
        /// Gets the two-dimensional collection of Cell objects.
        /// </summary>
        /// <value>The two-dimensional collection of Cell objects.</value>
        Cells Cells { get; }

        /// <summary>
        /// Gets the one-dimensional collection of Column objects.
        /// </summary>
        /// <value>The one-dimensional collection of Column objects.</value>
        Columns Columns { get; }

        /// <summary>
        /// Gets the one-dimensional collection of Row objects.
        /// </summary>
        /// <value>The one-dimensional collection of Row objects.</value>
        Rows Rows { get; }
    }
}

