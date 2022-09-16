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
    public class CustomTheme : ITheme
    {
        /// <summary>
        /// 获取主题画刷
        /// </summary>
        public Brush GetThemeBrush()
        {
            if (DateTime.Now.Month > 6)
                return new SolidColorBrush(Colors.Red);
            return new SolidColorBrush(Colors.Green);
        }
    }
}
