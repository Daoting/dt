#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-05-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 将值转换成两种类型画刷
    /// </summary>
    public class FlagColorConverter : IValueConverter
    {
        SolidColorBrush _trueBrush = Res.亮蓝;
        SolidColorBrush _falseBrush = Res.深灰2;

        /// <summary>
        /// 获取设置值为1或true时返回的画刷
        /// </summary>
        public SolidColorBrush TrueBrush
        {
            get { return _trueBrush; }
            set { _trueBrush = value; }
        }

        /// <summary>
        /// 获取设置值为0或false时返回的画刷
        /// </summary>
        public SolidColorBrush FalseBrush
        {
            get { return _falseBrush; }
            set { _falseBrush = value; }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string str;
            if (value == null || string.IsNullOrEmpty(str = value.ToString().ToLower()))
                return _falseBrush;
            return (str == "1" || str == "true") ? _trueBrush : _falseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

