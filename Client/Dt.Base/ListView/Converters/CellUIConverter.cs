#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-09 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// 单元格UI转换器
    /// </summary>
    class CellUIConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ViewItem vi = value as ViewItem;
            ICellUI pre = parameter as ICellUI;
            object result = null;
            if (vi != null && pre != null)
            {
                result = vi.GetCellUI(pre);
                if (result == null && parameter is Dot dot)
                {
                    // 隐藏Dot为了其 Padding 或 Margin 不再占用位置！！！
                    // 未处理Table模式的单元格ContentPresenter，因其负责画右下边线！
                    dot.Visibility = Visibility.Collapsed;
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
