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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class DataPointLabelConverter : IValueConverter
    {
        Dictionary<int, SpreadDataSeries> _dataSeries = new Dictionary<int, SpreadDataSeries>();

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
            string str = (string)(parameter as string);
            if (!string.IsNullOrEmpty(str) && (value != null))
            {
                Dt.Charts.DataPoint point = value as Dt.Charts.DataPoint;
                SpreadDataSeries series = DataSeries[point.SeriesIndex];
                return GetCustomConvertValue(series.GetDataLabel(point.PointIndex), str);
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

        object GetCustomConvertValue(DataLabel dataLabele, string para)
        {
            if (para == "backColor")
            {
                Brush actualFill = dataLabele.ActualFill;
                if ((actualFill != null) && !string.IsNullOrEmpty(dataLabele.Text))
                {
                    return actualFill;
                }
                return new SolidColorBrush(Colors.Transparent);
            }
            if (para == "borderDashArray")
            {
                DoubleCollection strokeDashes = Dt.Cells.Data.StrokeDashHelper.GetStrokeDashes(dataLabele.ActualStrokeDashType);
                if (strokeDashes != null)
                {
                    return strokeDashes;
                }
                return new DoubleCollection();
            }
            if (para == "borderColor")
            {
                Brush actualStroke = dataLabele.ActualStroke;
                if ((actualStroke != null) && !string.IsNullOrEmpty(dataLabele.Text))
                {
                    return actualStroke;
                }
                return new SolidColorBrush(Colors.Transparent);
            }
            if (para == "borderThickness")
            {
                return (double)(dataLabele.ActualStrokeThickness * ZoomFactor);
            }
            if (para == "margin")
            {
                return new Thickness(dataLabele.ActualStrokeThickness * ZoomFactor);
            }
            if (para == "textFontSize")
            {
                if (dataLabele.ActualFontSize > 0.0)
                {
                    return (double)(dataLabele.ActualFontSize * ZoomFactor);
                }
            }
            else
            {
                if (para == "textForeground")
                {
                    Brush actualForeground = dataLabele.ActualForeground;
                    if (actualForeground == null)
                    {
                        actualForeground = new SolidColorBrush(Colors.Black);
                    }
                    return actualForeground;
                }
                if (para == "text")
                {
                    return dataLabele.Text;
                }
                if (para == "textFontFamily")
                {
                    FontFamily actualFontFamily = dataLabele.ActualFontFamily;
                    if (actualFontFamily == null)
                    {
                        actualFontFamily = Utility.DefaultFontFamily;
                    }
                    return actualFontFamily;
                }
                if (para == "textFontStretch")
                {
                    return dataLabele.ActualFontStretch;
                }
                if (para == "textFontStyle")
                {
                    return dataLabele.ActualFontStyle;
                }
                if (para == "textFontWeight")
                {
                    return dataLabele.ActualFontWeight;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, SpreadDataSeries> DataSeries
        {
            get { return _dataSeries; }
            set { _dataSeries = value; }
        }

        /// <summary>
        /// Gets or sets the zoom factor.
        /// </summary>
        /// <value>
        /// The zoom factor.
        /// </value>
        public double ZoomFactor { get; set; }
    }
}

