#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a blank value converter.
    /// </summary>
    public class AutoFilterItemValueConverter : IValueConverter
    {
        /// <summary>
        /// Converts a filter item value to dropdown item's value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == BlankFilterItem.Blank)
            {
                return SR<ResourceStrings>.GetString("AutoFilterList.Blanks");
            }
            string str = (string) (value as string);
            if (!string.IsNullOrEmpty(str))
            {
                value = str.Replace(Environment.NewLine, string.Empty);
            }
            return value;
        }

        /// <summary>
        /// Converts a value to a filter item value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <exception cref="T:System.NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

