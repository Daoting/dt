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
    /// Represents an interface for supporting a theme context.
    /// </summary>
    public interface IThemeContextSupport
    {
        /// <summary>
        /// Gets the theme context.
        /// </summary>
        /// <returns>Returns the theme context.</returns>
        IThemeSupport GetContext();
        /// <summary>
        /// Sets the theme context.
        /// </summary>
        /// <param name="context">The theme context object.</param>
        void SetContext(IThemeSupport context);
    }
}

