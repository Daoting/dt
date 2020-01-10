#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-09 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
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
            if (vi != null && pre != null)
                return vi.GetCellUI(pre);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
