#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-09-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Media;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 主题样式接口
    /// </summary>
    public interface ITheme
    {
        /// <summary>
        /// 获取主题画刷
        /// </summary>
        Brush GetThemeBrush();
    }
}