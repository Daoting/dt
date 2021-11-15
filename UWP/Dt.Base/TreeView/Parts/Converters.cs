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

namespace Dt.Base.TreeViews
{
    /// <summary>
    /// IsSelected -> 字符图标
    /// </summary>
    class IsSelectedIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool? selected = (bool?)value;
            if (selected.HasValue)
                return selected.Value ? "\uE059" : "\uE057";
            return "\uE058";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// IsSelected -> 背景
    /// </summary>
    class SelectedBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool? selected = (bool?)value;
            if (selected.HasValue)
                return selected.Value ? Res.暗遮罩 : null;
            return Res.暗遮罩;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
