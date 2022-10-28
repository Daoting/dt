#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-07-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
#endregion

namespace Dt.Sample
{
    class CustomTheme : ITheme
    {
        /// <summary>
        /// 获取主题画刷，还需修改各平台的logo和背景图片与主题匹配
        /// </summary>
        public Brush ThemeBrush { get; } = new SolidColorBrush(Colors.Red);
    }
}
