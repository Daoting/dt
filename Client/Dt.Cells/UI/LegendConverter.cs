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
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class LegendConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            string str = (string) (parameter as string);
            if ((!string.IsNullOrEmpty(str) && (value != null)) && (Legend != null))
            {
                if (str == "borderThickness")
                {
                    double num = Legend.StrokeThickness * ZoomFactor;
                    if (num < 0.0)
                    {
                        num = 0.0;
                    }
                    return (double) num;
                }
                if (str == "borderDashArray")
                {
                    return Dt.Cells.Data.StrokeDashHelper.GetStrokeDashes(Legend.StrokeDashType);
                }
            }
            return null;
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public Dt.Cells.Data.Legend Legend { get; set; }

        /// <summary>
        /// Gets or sets the zoom factor.
        /// </summary>
        /// <value>
        /// The zoom factor.
        /// </value>
        public double ZoomFactor { get; set; }
    }
}

