#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using Dt.Xls.Chart;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Cells.Data
{
    internal static class ExcelChartExtension
    {
        internal static HashSet<int> _axisCollection = new HashSet<int>();

        internal static string ConvertImageSourceToBase64StringInPng(IExcelImage image)
        {
            if (image == null)
            {
                return null;
            }
            string result = null;
            try
            {
                using (MemoryStream stream = new MemoryStream(image.SourceArray))
                {
                    stream.Seek(0L, (SeekOrigin)SeekOrigin.Begin);
                    byte[] buffer = new byte[stream.Length];
                    stream.Seek(0L, (SeekOrigin)SeekOrigin.Begin);
                    stream.Read(buffer, 0, (int)stream.Length);
                    result = Convert.ToBase64String(buffer);
                }
            }
            catch
            { }
            return result;
        }

        internal static IExcelChartFormat CreateExcelChartFormat(string fillThemeColor, Brush fill, string strokeThemeColor, Brush stroke, double strokeThickness, bool automaticFill, bool automaticStroke, StrokeDashType dashType, double defaultThickness = 1.0, ExcelDrawingColorSettings fillDrawingSettings = null, ExcelDrawingColorSettings strokeDrawingSettings = null)
        {
            ExcelChartFormat format = new ExcelChartFormat();
            if ((((fill == null) && (stroke == null)) && (string.IsNullOrWhiteSpace(fillThemeColor) && string.IsNullOrWhiteSpace(strokeThemeColor))) && (((strokeThickness - defaultThickness).IsZero() && automaticFill) && automaticStroke))
            {
                return null;
            }
            if (!automaticFill)
            {
                if (!string.IsNullOrWhiteSpace(fillThemeColor))
                {
                    SolidFillFormat format2 = new SolidFillFormat
                    {
                        Color = fillThemeColor.GetExcelChartThemeColor()
                    };
                    if (((format2.Color != null) && (fillDrawingSettings != null)) && !format2.Color.Tint.IsZero())
                    {
                        fillDrawingSettings.Tint = new double?(format2.Color.Tint);
                    }
                    format2.DrawingColorSettings = fillDrawingSettings;
                    format.FillFormat = format2;
                }
                else if (fill != null)
                {
                    format.FillFormat = fill.ToExcelFillFormat();
                }
            }
            if (!automaticStroke)
            {
                if (!string.IsNullOrWhiteSpace(strokeThemeColor))
                {
                    SolidFillFormat format3 = new SolidFillFormat
                    {
                        Color = strokeThemeColor.GetExcelChartThemeColor()
                    };
                    if (((format3.Color != null) && (strokeDrawingSettings != null)) && !format3.Color.Tint.IsZero())
                    {
                        strokeDrawingSettings.Tint = new double?(format3.Color.Tint);
                    }
                    format3.DrawingColorSettings = strokeDrawingSettings;
                    if (format.LineFormat == null)
                    {
                        format.LineFormat = new LineFormat();
                    }
                    format.LineFormat.FillFormat = format3;
                }
                else if (stroke != null)
                {
                    if (format.LineFormat == null)
                    {
                        format.LineFormat = new LineFormat();
                    }
                    format.LineFormat.FillFormat = stroke.ToExcelFillFormat();
                }
            }
            if (strokeThickness > 0.0)
            {
                if (format.LineFormat == null)
                {
                    format.LineFormat = new LineFormat();
                }
                format.LineFormat.Width = strokeThickness;
            }
            if ((dashType != StrokeDashType.None) && (format.LineFormat != null))
            {
                format.LineFormat.LineDashType = GetExcelineDashType(dashType);
            }
            if (automaticFill)
            {
                format.FillFormat = null;
            }
            else if (format.FillFormat == null)
            {
                format.FillFormat = new NoFillFormat();
            }
            if (automaticStroke)
            {
                if (format.LineFormat != null)
                {
                    format.LineFormat.FillFormat = null;
                }
                return format;
            }
            if (format.LineFormat == null)
            {
                format.LineFormat = new LineFormat();
            }
            if (format.LineFormat.FillFormat == null)
            {
                format.LineFormat.FillFormat = new NoFillFormat();
            }
            return format;
        }

        internal static bool FontWeightsIsBold(FontWeight fontweight)
        {
            if (fontweight.Weight < FontWeights.Bold.Weight)
            {
                return false;
            }
            return true;
        }

        static int GenerateAxisId()
        {
            Random random = new Random();
            int num = 0;
            do
            {
                num = random.Next(0x989680, 0x5f5e0ff);
            }
            while (_axisCollection.Contains(num));
            _axisCollection.Add(num);
            return num;
        }

        internal static IExcelChartFormat GetExcelChartForamt(this SpreadChartElement chartElement)
        {
            IExcelChartFormat format = CreateExcelChartFormat(chartElement.FillThemeColor, chartElement.Fill, chartElement.StrokeThemeColor, chartElement.Stroke, chartElement.StrokeThickness, chartElement.IsAutomaticFill, chartElement.IsAutomaticStroke, chartElement.StrokeDashType, 1.0, (ExcelDrawingColorSettings)chartElement.FillDrawingColorSettings, (ExcelDrawingColorSettings)chartElement.StrokeDrawingColorSettings);
            if ((format != null) && (format.LineFormat != null))
            {
                format.LineFormat.JoinType = chartElement.JoinType;
                format.LineFormat.LineEndingCap = chartElement.LineEndingCap;
                format.LineFormat.CompoundLineType = chartElement.CompoundLineType;
                format.LineFormat.PenAlignment = chartElement.PenAlignment;
                format.LineFormat.HeadLineEndStyle = chartElement.HeadLineEndStyle;
                format.LineFormat.TailLineEndStyle = chartElement.TailLineEndStyle;
                if (format.LineFormat.LineDashType == Dt.Xls.Chart.LineDashType.None)
                {
                    format.LineFormat.LineDashType = chartElement.LineDashType;
                }
            }
            return format;
        }

        internal static ExcelChartType GetExcelChartType(SpreadChartType chartType)
        {
            switch (chartType)
            {
                case SpreadChartType.BarClustered:
                    return ExcelChartType.BarClustered;

                case SpreadChartType.BarStacked:
                    return ExcelChartType.BarStacked;

                case SpreadChartType.BarStacked100pc:
                    return ExcelChartType.BarStacked100Percent;

                case SpreadChartType.ColumnClustered:
                    return ExcelChartType.ColumnClustered;

                case SpreadChartType.ColumnStacked:
                    return ExcelChartType.ColumnStacked;

                case SpreadChartType.ColumnStacked100pc:
                    return ExcelChartType.ColumnStacked100Percent;

                case SpreadChartType.Line:
                case SpreadChartType.LineSmoothed:
                    return ExcelChartType.Line;

                case SpreadChartType.LineStacked:
                    return ExcelChartType.LineStacked;

                case SpreadChartType.LineStacked100pc:
                    return ExcelChartType.LineStacked100Percent;

                case SpreadChartType.LineWithMarkers:
                case SpreadChartType.LineWithMarkersSmoothed:
                    return ExcelChartType.LineWithMarkers;

                case SpreadChartType.LineStackedWithMarkers:
                    return ExcelChartType.LineStackedWithMarkers;

                case SpreadChartType.LineStacked100pcWithMarkers:
                    return ExcelChartType.LineStacked100PercentWithMarkers;

                case SpreadChartType.Pie:
                    return ExcelChartType.Pie;

                case SpreadChartType.PieExploded:
                    return ExcelChartType.ExplodedPie;

                case SpreadChartType.PieDoughnut:
                    return ExcelChartType.Doughunt;

                case SpreadChartType.PieExplodedDoughnut:
                    return ExcelChartType.DoughuntExploded;

                case SpreadChartType.Area:
                    return ExcelChartType.Area;

                case SpreadChartType.AreaStacked:
                    return ExcelChartType.AreaStacked;

                case SpreadChartType.AreaStacked100pc:
                    return ExcelChartType.AreaStacked100Percent;

                case SpreadChartType.Radar:
                    return ExcelChartType.Radar;

                case SpreadChartType.RadarWithMarkers:
                    return ExcelChartType.RadarWithMarkers;

                case SpreadChartType.RadarFilled:
                    return ExcelChartType.FilledRadar;

                case SpreadChartType.Scatter:
                    return ExcelChartType.Scatter;

                case SpreadChartType.ScatterLines:
                    return ExcelChartType.ScatterLines;

                case SpreadChartType.ScatterLinesWithMarkers:
                    return ExcelChartType.ScatterLinesWithMarkers;

                case SpreadChartType.ScatterLinesSmoothed:
                    return ExcelChartType.ScatterLinesSmooth;

                case SpreadChartType.ScatterLinesSmoothedWithMarkers:
                    return ExcelChartType.ScatterLinesSmoothWithMarkers;

                case SpreadChartType.Bubble:
                    return ExcelChartType.Bubble;

                case SpreadChartType.StockHighLowOpenClose:
                    return ExcelChartType.StockOpenHighLowClose;
            }
            return ExcelChartType.BarClustered;
        }

        internal static Dt.Xls.Chart.LineDashType GetExcelineDashType(StrokeDashType dashType)
        {
            switch (dashType)
            {
                case StrokeDashType.None:
                    return Dt.Xls.Chart.LineDashType.None;

                case StrokeDashType.Hair:
                    return Dt.Xls.Chart.LineDashType.SystemDot;

                case StrokeDashType.Dot:
                    return Dt.Xls.Chart.LineDashType.SystemDash;

                case StrokeDashType.Dash:
                    return Dt.Xls.Chart.LineDashType.Dash;

                case StrokeDashType.DashDot:
                    return Dt.Xls.Chart.LineDashType.DashDot;

                case StrokeDashType.LongDash:
                    return Dt.Xls.Chart.LineDashType.LongDash;

                case StrokeDashType.LongDashDot:
                    return Dt.Xls.Chart.LineDashType.LongDashDot;

                case StrokeDashType.LongDashDotDot:
                    return Dt.Xls.Chart.LineDashType.LongDashDotDot;

                case StrokeDashType.Thin:
                    return Dt.Xls.Chart.LineDashType.Solid;
            }
            return Dt.Xls.Chart.LineDashType.None;
        }

        static List<IExcelTrendLine> GetExcelTrendLines(List<TrendLine> trenlines)
        {
            if ((trenlines == null) || (trenlines.Count == 0))
            {
                return null;
            }
            List<IExcelTrendLine> list = new List<IExcelTrendLine>();
            for (int i = 0; i < trenlines.Count; i++)
            {
                IExcelTrendLine line = trenlines[i].ToExcelTrendLine();
                list.Add(line);
            }
            return list;
        }

        internal static object GetFill(IFillFormat fillFormat, Workbook workbook)
        {
            if (fillFormat != null)
            {
                switch (fillFormat.FillFormatType)
                {
                    case FillFormatType.NoFill:
                        return null;

                    case FillFormatType.SolidFill:
                        {
                            SolidFillFormat solidFill = fillFormat as SolidFillFormat;
                            return GetSolidFill(solidFill, workbook);
                        }
                    case FillFormatType.GradientFill:
                        {
                            GradientFillFormat gradientFill = fillFormat as GradientFillFormat;
                            GradientStopCollection stops = new GradientStopCollection();
                            foreach (ExcelGradientStop stop in gradientFill.GradientStops)
                            {
                                GradientStop stop2 = new GradientStop();
                                stop2.Color = Dt.Cells.Data.ColorHelper.UpdateColor(Dt.Cells.Data.ColorHelper.GetRGBColor(workbook, stop.Color), ToSpreadDrawingColorSettings(stop.DrawingColorSettings), true);
                                stop2.Offset = stop.Position;
                                stops.Add(stop2);
                            }
                            switch (gradientFill.GradientFillType)
                            {
                                case GradientFillType.Linear:
                                    {
                                        LinearGradientBrush result = new LinearGradientBrush(stops, gradientFill.Angle);
                                        Windows.Foundation.Point point = new Windows.Foundation.Point(0.0, 0.5);
                                        Windows.Foundation.Point point2 = new Windows.Foundation.Point(1.0, 0.5);
                                        if (gradientFill.Angle != 0.0)
                                        {
                                            RotateTransform transform = new RotateTransform();
                                            transform.Angle = gradientFill.Angle;
                                            transform.CenterX = 0.5;
                                            transform.CenterY = 0.5;
                                            point = transform.TransformPoint(point);
                                            point2 = transform.TransformPoint(point2);
                                        }
                                        result.StartPoint = point;
                                        result.EndPoint = point2;
                                        return result;
                                    }
                                case GradientFillType.Circle:
                                    return null;

                                case GradientFillType.Rectange:
                                    return null;

                                case GradientFillType.Shape:
                                    return null;
                            }
                            return null;
                        }
                    case FillFormatType.PictureFill:
                        return null;

                    case FillFormatType.PatternFill:
                        {
                            PatternFill fill = fillFormat as PatternFill;
                            if (fill.BackgroundColor == null)
                            {
                                if (fill.ForegroundColor != null)
                                {
                                    return new SolidColorBrush(Dt.Cells.Data.ColorHelper.UpdateColor(Dt.Cells.Data.ColorHelper.GetRGBColor(workbook, fill.ForegroundColor), ToSpreadDrawingColorSettings(fill.ForegroudDrawingColorSettings), true));
                                }
                                return null;
                            }
                            return new SolidColorBrush(Dt.Cells.Data.ColorHelper.UpdateColor(Dt.Cells.Data.ColorHelper.GetRGBColor(workbook, fill.BackgroundColor), ToSpreadDrawingColorSettings(fill.BackgroudDrawingColorSettings), true));
                        }
                    case FillFormatType.GroupFill:
                        return null;
                }
            }
            return null;
        }

        internal static object GetFillBrush(IExcelChartFormat format, Workbook workbook)
        {
            if ((format != null) && (format.FillFormat != null))
            {
                return GetFill(format.FillFormat, workbook);
            }
            return null;
        }

        /// <summary>
        /// Gets the gradinet style.
        /// </summary>
        /// <param name="gradientBrush">The gradient brush.</param>
        /// <returns></returns>
        static double GetGradientFillAngle(GradientBrush gradientBrush)
        {
            // uno 取消注释
            if (gradientBrush is LinearGradientBrush linearGradientBrush)
            {
                Windows.Foundation.Point startPoint = linearGradientBrush.StartPoint;
                Windows.Foundation.Point endPoint = linearGradientBrush.EndPoint;
                double offsetx = endPoint.X - startPoint.X;
                if (Math.Abs((double)(offsetx - 0.0)) < 1E-07)
                {
                    offsetx = 0.0;
                }
                double offsety = endPoint.Y - startPoint.Y;
                double num4 = UpdateAngle(Math.Asin(offsety / Math.Sqrt(Math.Pow(offsetx, 2.0) + Math.Pow(offsety, 2.0))), offsetx, offsety);
                if (Math.Abs(num4) < 0.78539816339744828)
                {
                    if (num4 >= 0.0)
                    {
                        return (45.0 * (Math.Abs(num4) / 0.78539816339744828));
                    }
                    return (360.0 - (45.0 * (Math.Abs(num4) / 0.78539816339744828)));
                }
                if (num4 == 0.78539816339744828)
                {
                    return 45.0;
                }
                if ((0.78539816339744828 < num4) && (num4 <= 1.5707963267948966))
                {
                    return (90.0 * (num4 / 1.5707963267948966));
                }
                if ((1.5707963267948966 < num4) && (num4 <= 2.3561944901923448))
                {
                    return (135.0 * (num4 / 2.3561944901923448));
                }
                if ((num4 > 2.3561944901923448) && (num4 <= 3.1415926535897931))
                {
                    return (180.0 * (num4 / 3.1415926535897931));
                }
                if ((num4 < -2.3561944901923448) && (num4 >= -3.1415926535897931))
                {
                    return (180.0 * ((6.2831853071795862 + num4) / 3.1415926535897931));
                }
                if ((num4 < -0.78539816339744828) && (num4 > -2.3561944901923448))
                {
                    return (180.0 * ((6.2831853071795862 + num4) / 3.1415926535897931));
                }
                if (num4 == -2.3561944901923448)
                {
                    return 225.0;
                }
                if (num4 == -0.78539816339744828)
                {
                    return 315.0;
                }
            }
            return 0.0;
        }

        internal static ImageSource GetImageSource(IExcelImage image)
        {
            if (image == null)
            {
                return null;
            }
            ImageSource result = null;
            try
            {
                using (MemoryStream stream = new MemoryStream(image.SourceArray))
                {
                    stream.Seek(0L, (SeekOrigin)SeekOrigin.Begin);
                    result = new BitmapImage();
                    Utility.InitImageSource(result as BitmapImage, (Stream)stream);
                }
            }
            catch
            { }
            return result;
        }

        static Axis GetPerpendiculaAxis(Axis axis)
        {
            SpreadChart chart = axis.Chart;
            if (chart != null)
            {
                if (axis.Orientation == Dt.Cells.Data.AxisOrientation.X)
                {
                    return chart.AxisY;
                }
                if (axis.Orientation == Dt.Cells.Data.AxisOrientation.Y)
                {
                    return chart.AxisX;
                }
            }
            return null;
        }

        static object GetSolidFill(SolidFillFormat solidFill, Workbook workbook)
        {
            if (solidFill.Color.IsThemeColor)
            {
                if (solidFill.DrawingColorSettings != null)
                {
                    int num = 0;
                    if (solidFill.DrawingColorSettings.Tint.HasValue)
                    {
                        num = (int)(solidFill.DrawingColorSettings.Tint.Value / 100000.0);
                    }
                    solidFill.Color = new ExcelColor(solidFill.Color.ColorType, solidFill.Color.Value, (double)num);
                }
                return solidFill.Color.GetThemeColorName();
            }
            if (solidFill.DrawingColorSettings == null)
            {
                return solidFill.Color.ToBrush(workbook);
            }
            if (solidFill.Color == null)
            {
                return null;
            }

            Windows.UI.Color rGBColor = Dt.Cells.Data.ColorHelper.GetRGBColor(workbook, solidFill.Color);
            if (solidFill.DrawingColorSettings.Alpha.HasValue)
            {
                byte a = (byte)((solidFill.DrawingColorSettings.Alpha.Value / 100000.0) * 255.0);
                rGBColor = Windows.UI.Color.FromArgb(a, rGBColor.R, rGBColor.G, rGBColor.B);
            }
            return new SolidColorBrush(rGBColor);
        }

        internal static SpreadDrawingColorSettings GetSpreadDrawingColorSettings(IFillFormat fillFormat)
        {
            if (fillFormat is SolidFillFormat)
            {
                return ToSpreadDrawingColorSettings((fillFormat as SolidFillFormat).DrawingColorSettings);
            }
            return null;
        }

        internal static object GetStokeBrush(IExcelChartFormat format, Workbook workbook)
        {
            if ((format != null) && ((format.LineFormat != null) && (format.LineFormat.FillFormat != null)))
            {
                return GetFill(format.LineFormat.FillFormat, workbook);
            }
            return null;
        }

        internal static StrokeDashType GetStrokeDashType(Dt.Xls.Chart.LineDashType lineDashType)
        {
            switch (lineDashType)
            {
                case Dt.Xls.Chart.LineDashType.Solid:
                    return StrokeDashType.Thin;

                case Dt.Xls.Chart.LineDashType.Dash:
                    return StrokeDashType.Dash;

                case Dt.Xls.Chart.LineDashType.DashDot:
                    return StrokeDashType.DashDot;

                case Dt.Xls.Chart.LineDashType.Dot:
                    return StrokeDashType.Dot;

                case Dt.Xls.Chart.LineDashType.LongDash:
                    return StrokeDashType.LongDash;

                case Dt.Xls.Chart.LineDashType.LongDashDot:
                    return StrokeDashType.LongDashDot;

                case Dt.Xls.Chart.LineDashType.LongDashDotDot:
                    return StrokeDashType.LongDashDotDot;

                case Dt.Xls.Chart.LineDashType.SystemDash:
                    return StrokeDashType.Dot;

                case Dt.Xls.Chart.LineDashType.SystemDashDot:
                case Dt.Xls.Chart.LineDashType.SystemDashDotDot:
                    return StrokeDashType.DashDot;

                case Dt.Xls.Chart.LineDashType.SystemDot:
                    return StrokeDashType.Hair;
            }
            return StrokeDashType.None;
        }

        static bool IsAreaChart(SpreadChartType chartType)
        {
            if ((chartType != SpreadChartType.Area) && (chartType != SpreadChartType.AreaStacked))
            {
                return (chartType == SpreadChartType.AreaStacked100pc);
            }
            return true;
        }

        static bool IsBarChart(SpreadChartType chartType)
        {
            if ((((chartType != SpreadChartType.ColumnClustered) && (chartType != SpreadChartType.ColumnStacked)) && ((chartType != SpreadChartType.ColumnStacked100pc) && (chartType != SpreadChartType.BarClustered))) && (chartType != SpreadChartType.BarStacked))
            {
                return (chartType == SpreadChartType.BarStacked100pc);
            }
            return true;
        }

        static bool IsBubbleChart(SpreadChartType chartType)
        {
            return (chartType == SpreadChartType.Bubble);
        }

        static bool IsDoughnutChart(SpreadChartType chartType)
        {
            if (chartType != SpreadChartType.PieDoughnut)
            {
                return (chartType == SpreadChartType.PieExplodedDoughnut);
            }
            return true;
        }

        static bool IsLineChart(SpreadChartType chartType)
        {
            if ((((chartType != SpreadChartType.Line) && (chartType != SpreadChartType.LineSmoothed)) && ((chartType != SpreadChartType.LineWithMarkersSmoothed) && (chartType != SpreadChartType.LineStacked))) && (((chartType != SpreadChartType.LineStacked100pc) && (chartType != SpreadChartType.LineStacked100pcWithMarkers)) && (chartType != SpreadChartType.LineStackedWithMarkers)))
            {
                return (chartType == SpreadChartType.LineWithMarkers);
            }
            return true;
        }

        internal static bool IsLineNoFill(IExcelChartFormat format)
        {
            return ((((format != null) && (format.LineFormat != null)) && (format.LineFormat.FillFormat != null)) && (format.LineFormat.FillFormat.FillFormatType == FillFormatType.NoFill));
        }

        internal static bool IsNoFill(IExcelChartFormat format)
        {
            return (((format != null) && (format.FillFormat != null)) && (format.FillFormat.FillFormatType == FillFormatType.NoFill));
        }

        static bool IsPieChart(SpreadChartType chartType)
        {
            if (chartType != SpreadChartType.Pie)
            {
                return (chartType == SpreadChartType.PieExploded);
            }
            return true;
        }

        static bool IsRadarChart(SpreadChartType chartType)
        {
            if ((chartType != SpreadChartType.Radar) && (chartType != SpreadChartType.RadarFilled))
            {
                return (chartType == SpreadChartType.RadarWithMarkers);
            }
            return true;
        }

        static bool IsScatterChart(SpreadChartType chartType)
        {
            if (((chartType != SpreadChartType.Scatter) && (chartType != SpreadChartType.ScatterLinesSmoothed)) && ((chartType != SpreadChartType.ScatterLinesSmoothedWithMarkers) && (chartType != SpreadChartType.ScatterLines)))
            {
                return (chartType == SpreadChartType.ScatterLinesWithMarkers);
            }
            return true;
        }

        internal static Windows.Foundation.Point RotatePoint(Windows.Foundation.Point point, double angle)
        {
            RotateTransform transform = new RotateTransform();
            transform.Angle = angle;
            return transform.TransformPoint(point);
        }

        static void SetAxis(IExcelChartAxis axis, Workbook workbook, Axis result)
        {
            if (axis is IExcelChartCategoryAxis)
            {
                result.AxisType = AxisType.Category;
                result.IsAumaticCategoryAxis = (axis as IExcelChartCategoryAxis).IsAutomaticCategoryAxis;
                result.NoMultiLevelLables = (axis as IExcelChartCategoryAxis).NoMultiLevelLables;
            }
            else if (axis is IExcelChartValueAxis)
            {
                result.AxisType = AxisType.Value;
            }
            else if (axis is IExcelChartDateAxis)
            {
                result.AxisType = AxisType.Date;
            }
            else
            {
                result.AxisType = AxisType.Category;
                result.IsSerAxis = true;
            }
            SetSpreadChartElementStyle(result, axis.ShapeFormat, workbook);
            SetSpreadAxisStyle(result, axis.ShapeFormat);
            result.TextFormat = axis.TextFormat;
            if (axis.TextFormat != null)
            {
                result.Foreground = RichTextUtility.GetRichTextFill(axis.TextFormat.TextParagraphs, workbook);
                double? richTextFontSize = RichTextUtility.GetRichTextFontSize(axis.TextFormat.TextParagraphs);
                if (richTextFontSize.HasValue && richTextFontSize.HasValue)
                {
                    result.FontSize = richTextFontSize.Value;
                }
                FontFamily richTextFamily = RichTextUtility.GetRichTextFamily(axis.TextFormat.TextParagraphs);
                if (richTextFamily != null)
                {
                    result.FontFamily = richTextFamily;
                }
                result.FontStyle = RichTextUtility.GetRichTextFontStyle(axis.TextFormat.TextParagraphs);
                result.FontWeight = RichTextUtility.GetRichTextFontWeight(axis.TextFormat.TextParagraphs);
                if ((axis.TextFormat.Rotation >= -90.0) && (axis.TextFormat.Rotation <= 90.0))
                {
                    result.LabelAngle = axis.TextFormat.Rotation;
                }
            }
            result.Title = axis.AxisTitle.ToSpreadChartTitle(workbook);
            if (result.Title != null)
            {
                result.Title.TitleType = TitleType.AxisTitle;
            }
            if (!string.IsNullOrWhiteSpace(axis.NumberFormat))
            {
                result.LabelFormatter = new GeneralFormatter(axis.NumberFormat);
            }
            result.NumberFormatSourceLinked = axis.NumberFormatLinked;
            if (axis.Scaling != null)
            {
                if (!double.IsNaN(axis.Scaling.LogBase))
                {
                    result.UseLogBase = true;
                    result.LogBase = axis.Scaling.LogBase;
                }
                else
                {
                    result.UseLogBase = false;
                }
                if (double.IsNaN(axis.Scaling.Max))
                {
                    result.AutoMax = true;
                }
                else
                {
                    result.AutoMax = false;
                    result.Max = axis.Scaling.Max;
                }
                if (double.IsNaN(axis.Scaling.Min))
                {
                    result.AutoMin = true;
                }
                else
                {
                    result.AutoMin = false;
                    result.Min = axis.Scaling.Min;
                }
                if (axis.Scaling.Orientation == Dt.Xls.Chart.AxisOrientation.MaxMin)
                {
                    result.Reversed = true;
                }
            }
            if (axis.MajorGridlines == null)
            {
                result.ShowMajorGridlines = false;
            }
            else
            {
                result.ShowMajorGridlines = true;
                if (((axis.MajorGridlines.Format == null) || (axis.MajorGridlines.Format.LineFormat == null)) || (axis.MajorGridlines.Format.LineFormat.FillFormat == null))
                {
                    result.IsMajorGridlinesStrokeAutomatic = true;
                }
                else
                {
                    result.IsMajorGridlinesStrokeAutomatic = false;
                    object stokeBrush = GetStokeBrush(axis.MajorGridlines.Format, workbook);
                    if (stokeBrush is string)
                    {
                        result.MajorGridlinesStrokeThemeColor = (string)(stokeBrush as string);
                    }
                    else if (stokeBrush is Brush)
                    {
                        result.MajorGridlinesStroke = stokeBrush as Brush;
                    }
                    result.MajorGridlineDrawingColorSettings = GetSpreadDrawingColorSettings(axis.MajorGridlines.Format.LineFormat.FillFormat);
                    double width = 0.0;
                    if ((axis.MajorGridlines.Format != null) && (axis.MajorGridlines.Format.LineFormat != null))
                    {
                        width = axis.MajorGridlines.Format.LineFormat.Width;
                    }
                    if (width > 0.0)
                    {
                        result.MajorGridlinesStrokeThickness = width;
                    }
                    if ((axis.MajorGridlines.Format != null) && (axis.MajorGridlines.Format.LineFormat != null))
                    {
                        ILineFormat lineFormat = axis.MajorGridlines.Format.LineFormat;
                        switch (lineFormat.LineEndingCap)
                        {
                            case EndLineCap.Flat:
                                result.MajorGridLineCapType = PenLineCap.Flat;
                                break;

                            case EndLineCap.Round:
                                result.MajorGridLineCapType = PenLineCap.Round;
                                break;

                            case EndLineCap.Square:
                                result.MajorGridLineCapType = PenLineCap.Square;
                                break;
                        }
                        result.MajorGridlinesStrokeDashType = GetStrokeDashType(axis.MajorGridlines.Format.LineFormat.LineDashType);
                        result.MajorGridLineJoinType = (PenLineJoin)lineFormat.JoinType;
                        result.MajorGridLineBeginArrowSettings = lineFormat.HeadLineEndStyle.ToArrowSettings();
                        result.MajorGridLineEndArrowSettings = lineFormat.TailLineEndStyle.ToArrowSettings();
                    }
                }
            }
            if (axis.MinorGridlines == null)
            {
                result.ShowMinorGridlines = false;
            }
            else
            {
                result.ShowMinorGridlines = true;
                if (((axis.MinorGridlines.Format == null) || (axis.MinorGridlines.Format.LineFormat == null)) || (axis.MinorGridlines.Format.LineFormat.FillFormat == null))
                {
                    result.IsMinorGridlinesStrokeAutomatic = true;
                }
                else
                {
                    result.IsMinorGridlinesStrokeAutomatic = false;
                    object obj3 = GetStokeBrush(axis.MinorGridlines.Format, workbook);
                    if (obj3 is string)
                    {
                        result.MinorGridlinesStrokeThemeColor = (string)(obj3 as string);
                    }
                    else if (obj3 is Brush)
                    {
                        result.MinorGridlinesStroke = obj3 as Brush;
                    }
                    result.MajorGridlineDrawingColorSettings = GetSpreadDrawingColorSettings(axis.MajorGridlines.Format.LineFormat.FillFormat);
                    double num2 = 0.0;
                    if ((axis.MinorGridlines.Format != null) && (axis.MinorGridlines.Format.LineFormat != null))
                    {
                        num2 = axis.MinorGridlines.Format.LineFormat.Width;
                    }
                    result.MinorGridlinesStrokeThickness = num2;
                    if ((axis.MinorGridlines.Format != null) && (axis.MinorGridlines.Format.LineFormat != null))
                    {
                        ILineFormat format2 = axis.MinorGridlines.Format.LineFormat;
                        switch (format2.LineEndingCap)
                        {
                            case EndLineCap.Flat:
                                result.MinorGridLineCapType = PenLineCap.Flat;
                                break;

                            case EndLineCap.Round:
                                result.MinorGridLineCapType = PenLineCap.Round;
                                break;

                            case EndLineCap.Square:
                                result.MinorGridLineCapType = PenLineCap.Square;
                                break;
                        }
                        result.MinorGridlinesStrokeDashType = GetStrokeDashType(axis.MinorGridlines.Format.LineFormat.LineDashType);
                        result.MinorGridLineJoinType = (PenLineJoin)format2.JoinType;
                        result.MinorGridLineBeginArrowSettings = format2.HeadLineEndStyle.ToArrowSettings();
                        result.MinorGridLineEndArrowSettings = format2.TailLineEndStyle.ToArrowSettings();
                    }
                }
            }
            result.MajorTickPosition = (AxisTickPosition)axis.MajorTickMark;
            result.MinorTickPosition = (AxisTickPosition)axis.MinorTickMark;
            switch (axis.TickLabelPosition)
            {
                case TickLabelPosition.None:
                    result.ShowAxisLabel = false;
                    break;

                case TickLabelPosition.High:
                    result.ShowAxisLabel = true;
                    result.LabelPosition = AxisLabelPosition.Far;
                    break;

                case TickLabelPosition.Low:
                    result.ShowAxisLabel = true;
                    result.LabelPosition = AxisLabelPosition.Near;
                    break;

                case TickLabelPosition.NextTo:
                    result.ShowAxisLabel = true;
                    result.LabelPosition = AxisLabelPosition.Auto;
                    break;
            }
            result.Visible = !axis.Delete;
            result.Id = axis.Id;
            result.AxisCrosses = (Dt.Cells.Data.AxisCrosses)axis.Crosses;
            result.CrossAt = axis.CrosssAt;
            if ((axis.AxisPosition == Dt.Xls.Chart.AxisPosition.Left) || (axis.AxisPosition == Dt.Xls.Chart.AxisPosition.Bottom))
            {
                result.AxisPosition = Dt.Cells.Data.AxisPosition.Near;
            }
            else
            {
                result.AxisPosition = Dt.Cells.Data.AxisPosition.Far;
            }
            if (axis is ExcelChartCategoryAxis)
            {
                ExcelChartCategoryAxis axis2 = axis as ExcelChartCategoryAxis;
                result.LableOffset = axis2.LabelOffset;
                result.TickLabelInterval = axis2.TickLalelInterval;
                result.TickMarkInterval = axis2.TickMarkInterval;
            }
            else if (axis is ExcelChartDateAxis)
            {
                ExcelChartDateAxis axis3 = axis as ExcelChartDateAxis;
                result.LableOffset = axis3.LabelOffset;
                if (!axis3.IsAutoMajorUnit)
                {
                    result.MajorUnit = axis3.MajorUnit;
                }
                if (!axis3.IsAutoMinorUnit)
                {
                    result.MinorUnit = axis3.MinorUnit;
                }
                if (!axis3.IsAutoMajorTimeUint)
                {
                    result.MajorTimeUnit = (Dt.Cells.Data.AxisTimeUnit)axis3.MajorTimeUnit;
                }
                if (!axis3.IsAutoMinorTimeUnit)
                {
                    result.MinorTimeUnit = (Dt.Cells.Data.AxisTimeUnit)axis3.MinorTimeUnit;
                }
                result.BaseTimeUnit = (Dt.Cells.Data.AxisTimeUnit)axis3.BaseTimeUnit;
            }
            else if (axis is ExcelChartSeriesAxis)
            {
                IExcelChartSeriesAxis axis4 = axis as IExcelChartSeriesAxis;
                result.TickLabelInterval = axis4.TickLalelInterval;
                result.TickMarkInterval = axis4.TickMarkInterval;
            }
            else if (axis is ExcelChartValueAxis)
            {
                ExcelChartValueAxis axis5 = axis as ExcelChartValueAxis;
                if (!axis5.IsAutoMajorUnit)
                {
                    result.MajorUnit = axis5.MajorUnit;
                    result.AutoMajorUnit = false;
                }
                else
                {
                    result.AutoMajorUnit = true;
                }
                if (!axis5.IsAutoMinorUnit)
                {
                    result.MinorUnit = axis5.MinorUnit;
                    result.AutoMinorUnit = false;
                }
                else
                {
                    result.AutoMinorUnit = true;
                }
                if (axis5.DislayUnits != null)
                {
                    result.DisplayUnitSettings = new DisplayUnitSettings();
                    if (axis5.DislayUnits.IsCustomDisplayUnit)
                    {
                        result.DisplayUnitSettings.DisplayUnit = axis5.DislayUnits.CustomDisplayUnit;
                    }
                    else
                    {
                        result.DisplayUnitSettings.DisplayUnit = (double)axis5.DislayUnits.BuiltInDisplayUnit;
                    }
                    result.DisplayUnitSettings.LabelLayout = axis5.DislayUnits.Layout.ToSpreadLayout();
                    if (!string.IsNullOrWhiteSpace(axis5.DislayUnits.TextFormula))
                    {
                        result.DisplayUnitSettings.LabelText = axis5.DislayUnits.TextFormula;
                    }
                    else
                    {
                        result.DisplayUnitSettings.LabelRichText = axis5.DislayUnits.RichText;
                    }
                }
                result.CrossBetween = (AxisCrossBetween)axis5.CrossBetween;
            }
        }

        internal static void SetDataLableSettings(this SpreadDataSeries spreadDateSeries, IExcelDataLabels dataLabels, Workbook workbook)
        {
            if ((spreadDateSeries != null) && (dataLabels != null))
            {
                spreadDateSeries.DataLabelSettings = dataLabels.ToSpreadDataLabelSettings(workbook);
                SetSpreadDataLabelStyleInfo(spreadDateSeries.DataLabelStyle, dataLabels, workbook);
            }
        }

        internal static void SetDataMarker(this SpreadDataSeries spreadDataSeries, ExcelDataMarker dataMarker, Workbook workbook)
        {
            if (dataMarker != null)
            {
                switch (dataMarker.MarkerStyle)
                {
                    case DataPointMarkStyle.None:
                        spreadDataSeries.MarkerType = Dt.Cells.Data.MarkerType.None;
                        break;

                    case DataPointMarkStyle.Circle:
                        spreadDataSeries.MarkerType = Dt.Cells.Data.MarkerType.Circle;
                        break;

                    case DataPointMarkStyle.Dash:
                        spreadDataSeries.MarkerType = Dt.Cells.Data.MarkerType.Dot;
                        break;

                    case DataPointMarkStyle.Diamond:
                        spreadDataSeries.MarkerType = Dt.Cells.Data.MarkerType.Diamond;
                        break;

                    case DataPointMarkStyle.Dot:
                        spreadDataSeries.MarkerType = Dt.Cells.Data.MarkerType.Dot;
                        break;

                    case DataPointMarkStyle.Plus:
                        spreadDataSeries.MarkerType = Dt.Cells.Data.MarkerType.Cross;
                        break;

                    case DataPointMarkStyle.Square:
                        spreadDataSeries.MarkerType = Dt.Cells.Data.MarkerType.Box;
                        break;

                    case DataPointMarkStyle.Star:
                        spreadDataSeries.MarkerType = Dt.Cells.Data.MarkerType.Star4;
                        break;

                    case DataPointMarkStyle.Triangle:
                        spreadDataSeries.MarkerType = Dt.Cells.Data.MarkerType.Triangle;
                        break;

                    case DataPointMarkStyle.X:
                        spreadDataSeries.MarkerType = Dt.Cells.Data.MarkerType.DiagonalCross;
                        break;
                }
                if (dataMarker.MarkerStyle == DataPointMarkStyle.None)
                {
                    spreadDataSeries.MarkerSize = new Windows.Foundation.Size(0.0, 0.0);
                }
                else
                {
                    spreadDataSeries.MarkerSize = new Windows.Foundation.Size((double)dataMarker.MarkerSize, (double)dataMarker.MarkerSize);
                }
                if (dataMarker.Format != null)
                {
                    spreadDataSeries.DataMarkerStyle = dataMarker.Format.ToSpreadChartSymbolStyleInfo(workbook, spreadDataSeries);
                }
            }
        }

        internal static void SetDataPoints(this SpreadDataSeries spreadDataSeries, List<IExcelDataPoint> dataPoints, Workbook workbook)
        {
            if ((dataPoints != null) && (dataPoints != null))
            {
                foreach (IExcelDataPoint point in dataPoints)
                {
                    DataPoint dataPoint = spreadDataSeries.GetDataPoint(point.Index);
                    SetSpreadChartElementStyle(dataPoint, point.DataPointFormat, workbook);
                    dataPoint.InvertIfNegative = point.InvertIfNegative;
                    dataPoint.IsBubble3D = point.IsBubble3D;
                    dataPoint.Explosion = point.Explosion;
                    dataPoint.DataMarker = point.Marker.ToSpreadDataMarker(workbook);
                    dataPoint.PictureOptions = point.PictureOptions;
                }
            }
        }

        internal static void SetDataSeries(SpreadDataSeries spreadDataSeries, IExcelChartSeriesBase excelSeries, Workbook workbook)
        {
            if ((spreadDataSeries != null) && (excelSeries != null))
            {
                spreadDataSeries.SetSeriesValue(excelSeries.SeriesValue);
                spreadDataSeries.SetSeriesName(excelSeries.SeriesName);
                spreadDataSeries.SetDataLableSettings(excelSeries.DataLabels, workbook);
                spreadDataSeries.SetDataPoints(excelSeries.DataPoints, workbook);
                spreadDataSeries.SetStyle(excelSeries.Format, workbook);
            }
        }

        internal static void SetExcelAreaChartSettings(SpreadChart chart, IExcelAreaChartBase excelChart, IEnumerable<SpreadDataSeries> series)
        {
            SetExcelChartBasicSettings(chart, excelChart);
            foreach (SpreadDataSeries series2 in series)
            {
                if (series2 != null)
                {
                    ExcelAreaSeries excelSeries = new ExcelAreaSeries();
                    SetExcelChartBasicSeriesSettings(chart, series2, excelSeries, DataSeriesCounter.Index);
                    excelSeries.Trendlines = GetExcelTrendLines(series2.TrendLines);
                    excelSeries.FirstErrorBars = series2.FirstErrorBars.ToExcelErrorBars();
                    excelSeries.SecondErrorBars = series2.FirstErrorBars.ToExcelErrorBars();
                    excelSeries.PictureOptions = series2.PictureOptions;
                    excelChart.AreaSeries.Add(excelSeries);
                    DataSeriesCounter.Index++;
                }
            }
            excelChart.AxisX = chart.AxisX.ToExcelChartAxis();
            excelChart.AxisY = chart.AxisY.ToExcelChartAxis();
        }

        static void SetExcelAxis(Axis axis, IExcelChartAxis excelAxis)
        {
            if (axis.Title != null)
            {
                excelAxis.AxisTitle = axis.Title.ToExcelSpreadChartTitle();
            }
            if (axis.LabelFormatter != null)
            {
                excelAxis.NumberFormat = axis.LabelFormatter.FormatString;
                excelAxis.NumberFormatLinked = axis.NumberFormatSourceLinked;
            }
            Scaling scaling = new Scaling();
            if (axis.UseLogBase && !double.IsNaN(axis.LogBase))
            {
                scaling.LogBase = axis.LogBase;
            }
            else
            {
                scaling.LogBase = double.NaN;
            }
            if (axis.Reversed)
            {
                scaling.Orientation = Dt.Xls.Chart.AxisOrientation.MaxMin;
            }
            if (!axis.AutoMax)
            {
                scaling.Max = axis.Max;
            }
            else
            {
                scaling.Max = double.NaN;
            }
            if (!axis.AutoMin)
            {
                scaling.Min = axis.Min;
            }
            else
            {
                scaling.Min = double.NaN;
            }
            excelAxis.Scaling = scaling;
            excelAxis.MinorTickMark = (TickMark)axis.MinorTickPosition;
            excelAxis.MajorTickMark = (TickMark)axis.MajorTickPosition;
            if (axis.ShowMajorGridlines)
            {
                excelAxis.MajorGridlines = new ExcelGridLine();
                if (!axis.IsMajorGridlinesStrokeAutomatic)
                {
                    IExcelChartFormat format = CreateExcelChartFormat(null, null, axis.MajorGridlinesStrokeThemeColor, axis.MajorGridlinesStroke, axis.MajorGridlinesStrokeThickness, true, axis.IsMajorGridlinesStrokeAutomatic, axis.MajorGridlinesStrokeDashType, 1.0, null, (ExcelDrawingColorSettings)axis.MajorGridlineDrawingColorSettings);
                    if (format != null)
                    {
                        excelAxis.MajorGridlines.Format = format;
                    }
                }
            }
            if (axis.ShowMinorGridlines)
            {
                excelAxis.MinorGridlines = new ExcelGridLine();
                if (!axis.IsMinorGridlinesStrokeAutomatic)
                {
                    IExcelChartFormat format2 = CreateExcelChartFormat(null, null, axis.MinorGridlinesStrokeThemeColor, axis.MinorGridlinesStroke, axis.MinorGridlinesStrokeThickness, true, axis.IsMinorGridlinesStrokeAutomatic, axis.MinorGridlinesStrokeDashType, 0.0, null, (ExcelDrawingColorSettings)axis.MinorGridlineDrawingColorSettings);
                    if (format2 != null)
                    {
                        excelAxis.MinorGridlines.Format = format2;
                    }
                }
            }
            if (!double.IsNaN(axis.CrossAt))
            {
                excelAxis.CrosssAt = axis.CrossAt;
            }
            else
            {
                Axis perpendiculaAxis = GetPerpendiculaAxis(axis);
                switch (axis.AxisPosition)
                {
                    case Dt.Cells.Data.AxisPosition.Near:
                        if ((perpendiculaAxis == null) || !perpendiculaAxis.Reversed)
                        {
                            excelAxis.Crosses = Dt.Xls.Chart.AxisCrosses.Min;
                            break;
                        }
                        excelAxis.Crosses = Dt.Xls.Chart.AxisCrosses.Max;
                        break;

                    case Dt.Cells.Data.AxisPosition.Far:
                        if ((perpendiculaAxis == null) || !perpendiculaAxis.Reversed)
                        {
                            excelAxis.Crosses = Dt.Xls.Chart.AxisCrosses.Max;
                            break;
                        }
                        excelAxis.Crosses = Dt.Xls.Chart.AxisCrosses.Min;
                        break;
                }
            }
            if (!axis.ShowAxisLabel)
            {
                excelAxis.TickLabelPosition = TickLabelPosition.None;
            }
            else
            {
                switch (axis.LabelPosition)
                {
                    case AxisLabelPosition.Auto:
                        excelAxis.TickLabelPosition = TickLabelPosition.NextTo;
                        break;

                    case AxisLabelPosition.Near:
                        excelAxis.TickLabelPosition = TickLabelPosition.Low;
                        break;

                    case AxisLabelPosition.Far:
                        excelAxis.TickLabelPosition = TickLabelPosition.High;
                        break;
                }
            }
            excelAxis.Delete = !axis.Visible;
        }

        internal static void SetExcelAxisStyle(IExcelChartFormat format, Axis axis)
        {
            if (format == null)
            {
                format = new ExcelChartFormat();
            }
            if (format.LineFormat == null)
            {
                format.LineFormat = new LineFormat();
            }
            if (axis.LineCapType == PenLineCap.Flat)
            {
                format.LineFormat.LineEndingCap = EndLineCap.Flat;
            }
            else if (axis.LineCapType == PenLineCap.Round)
            {
                format.LineFormat.LineEndingCap = EndLineCap.Round;
            }
            else if (axis.LineCapType == PenLineCap.Square)
            {
                format.LineFormat.LineEndingCap = EndLineCap.Square;
            }
            if (axis.LineJoinType == PenLineJoin.Bevel)
            {
                format.LineFormat.JoinType = EndLineJoinType.Bevel;
            }
            else if (axis.LineJoinType == PenLineJoin.Miter)
            {
                format.LineFormat.JoinType = EndLineJoinType.Miter;
            }
            else
            {
                format.LineFormat.JoinType = EndLineJoinType.Round;
            }
        }

        static void SetExcelAxisTextFormat(IExcelChartAxis serAxis, Axis axis)
        {
            serAxis.TextFormat = axis.TextFormat;
            if (((axis.LabelAngle >= -90.0) && (axis.LabelAngle <= 90.0)) && !axis.LabelAngle.IsZero())
            {
                if (serAxis.TextFormat == null)
                {
                    serAxis.TextFormat = new ExcelTextFormat();
                }
                serAxis.TextFormat.Rotation = axis.LabelAngle;
            }
            if ((serAxis.TextFormat != null) && (serAxis.TextFormat.TextParagraphs.Count == 0))
            {
                serAxis.TextFormat.TextParagraphs.Add(new TextParagraph());
            }
        }

        internal static void SetExcelBarChartBaseSettings(SpreadChart chart, IExcelBarChartBase excelChart, IEnumerable<SpreadDataSeries> series)
        {
            SetExcelChartBasicSettings(chart, excelChart);
            if (series != null)
            {
                foreach (SpreadDataSeries series2 in series)
                {
                    if (series2 != null)
                    {
                        ExcelBarSeries excelSeries = new ExcelBarSeries();
                        SetExcelChartBasicSeriesSettings(chart, series2, excelSeries, DataSeriesCounter.Index);
                        excelSeries.InvertIfNegative = series2.InvertIfNegative;
                        excelSeries.Trendlines = GetExcelTrendLines(series2.TrendLines);
                        excelSeries.ErrorBars = series2.FirstErrorBars.ToExcelErrorBars();
                        if (!string.IsNullOrWhiteSpace(series2.NegativeFillThemeColor))
                        {
                            excelSeries.NegativeSolidFillFormat = new SolidFillFormat();
                            excelSeries.NegativeSolidFillFormat.Color = series2.NegativeFillThemeColor.GetExcelChartThemeColor();
                        }
                        else if (series2.NegativeFill is SolidColorBrush)
                        {
                            excelSeries.NegativeSolidFillFormat = new SolidFillFormat();
                            excelSeries.NegativeSolidFillFormat.Color = (series2.NegativeFill as SolidColorBrush).Color.ToExcelColor();
                        }
                        excelSeries.PictureOptions = series2.PictureOptions;
                        excelChart.BarSeries.Add(excelSeries);
                        DataSeriesCounter.Index++;
                    }
                }
            }
            switch (chart.ChartType)
            {
                case SpreadChartType.ColumnClustered:
                case SpreadChartType.ColumnStacked:
                case SpreadChartType.ColumnStacked100pc:
                    excelChart.AxisX = chart.AxisX.ToExcelChartAxis();
                    excelChart.AxisY = chart.AxisY.ToExcelChartAxis();
                    break;

                case SpreadChartType.BarClustered:
                case SpreadChartType.BarStacked:
                case SpreadChartType.BarStacked100pc:
                    excelChart.AxisX = chart.AxisY.ToExcelChartAxis();
                    excelChart.AxisY = chart.AxisX.ToExcelChartAxis();
                    break;
            }
            if ((((excelChart.ChartType == ExcelChartType.BarClustered3D) || (excelChart.ChartType == ExcelChartType.BarStacked3D)) || ((excelChart.ChartType == ExcelChartType.BarStacked100Percent3D) || (excelChart.ChartType == ExcelChartType.BarClustered))) || ((excelChart.ChartType == ExcelChartType.BarStacked) || (excelChart.ChartType == ExcelChartType.BarStacked100Percent)))
            {
                if (chart.AxisX.AxisPosition == Dt.Cells.Data.AxisPosition.Far)
                {
                    excelChart.AxisX.AxisPosition = Dt.Xls.Chart.AxisPosition.Right;
                }
                else
                {
                    excelChart.AxisX.AxisPosition = Dt.Xls.Chart.AxisPosition.Left;
                }
                if (chart.AxisY.AxisPosition == Dt.Cells.Data.AxisPosition.Far)
                {
                    excelChart.AxisY.AxisPosition = Dt.Xls.Chart.AxisPosition.Top;
                }
                else
                {
                    excelChart.AxisY.AxisPosition = Dt.Xls.Chart.AxisPosition.Bottom;
                }
            }
            else
            {
                if (chart.AxisX.AxisPosition == Dt.Cells.Data.AxisPosition.Far)
                {
                    excelChart.AxisX.AxisPosition = Dt.Xls.Chart.AxisPosition.Top;
                }
                else
                {
                    excelChart.AxisX.AxisPosition = Dt.Xls.Chart.AxisPosition.Bottom;
                }
                if (chart.AxisY.AxisPosition == Dt.Cells.Data.AxisPosition.Far)
                {
                    excelChart.AxisY.AxisPosition = Dt.Xls.Chart.AxisPosition.Right;
                }
                else
                {
                    excelChart.AxisY.AxisPosition = Dt.Xls.Chart.AxisPosition.Left;
                }
            }
        }

        static void SetExcelBubbleChartSettings(SpreadChart chart, IExcelBubbleChart excelChart, IEnumerable<SpreadBubbleSeries> series)
        {
            SetExcelChartBasicSettings(chart, excelChart);
            foreach (SpreadBubbleSeries series2 in series)
            {
                if (series2 != null)
                {
                    ExcelBubbleSeries excelSeries = new ExcelBubbleSeries();
                    SetExcelChartBasicSeriesSettingsForScatterBubbleChart(chart, series2, excelSeries, DataSeriesCounter.Index);
                    ExcelSeriesValue value2 = new ExcelSeriesValue();
                    if (!string.IsNullOrWhiteSpace(series2.SizeFormula))
                    {
                        value2.ReferenceFormula = series2.SizeFormula;
                        if ((series2.SizeFormatter != null) && (series2.SizeFormatter.FormatString != "General"))
                        {
                            value2.FormatCode = series2.SizeFormatter.FormatString;
                        }
                    }
                    else if ((series2.SizeValues != null) && (series2.SizeValues.Count > 0))
                    {
                        value2.NumericLiterals = new NumericDataLiterals();
                        if (series2.Formatter != null)
                        {
                            value2.FormatCode = series2.Formatter.FormatString;
                        }
                        for (int i = 0; i < series2.SizeValues.Count; i++)
                        {
                            double num2 = series2.SizeValues[i];
                            ExcelNumberPoint point = new ExcelNumberPoint
                            {
                                Index = i,
                                Value = ((double)num2).ToString((IFormatProvider)CultureInfo.InvariantCulture)
                            };
                            value2.NumericLiterals.NumberPoints.Add(point);
                        }
                    }
                    excelSeries.BubbleSize = value2;
                    excelSeries.InvertIfNegative = series2.InvertIfNegative;
                    excelSeries.Bubble3D = series2.Bubble3D;
                    excelSeries.Trendlines = GetExcelTrendLines(series2.TrendLines);
                    excelSeries.FirstErrorBars = series2.FirstErrorBars.ToExcelErrorBars();
                    excelSeries.SecondErrorBars = series2.SecondErrorBars.ToExcelErrorBars();
                    excelChart.BubbleSeries.Add(excelSeries);
                    DataSeriesCounter.Index++;
                }
            }
            excelChart.AxisX = chart.AxisX.ToExcelChartAxis();
            if (excelChart.AxisX != null)
            {
                if (chart.AxisX.AxisPosition == Dt.Cells.Data.AxisPosition.Near)
                {
                    excelChart.AxisX.AxisPosition = Dt.Xls.Chart.AxisPosition.Bottom;
                }
                else
                {
                    excelChart.AxisX.AxisPosition = Dt.Xls.Chart.AxisPosition.Top;
                }
            }
            excelChart.AxisY = chart.AxisY.ToExcelChartAxis();
            if (excelChart.AxisY != null)
            {
                if (chart.AxisY.AxisPosition == Dt.Cells.Data.AxisPosition.Near)
                {
                    excelChart.AxisY.AxisPosition = Dt.Xls.Chart.AxisPosition.Left;
                }
                else
                {
                    excelChart.AxisY.AxisPosition = Dt.Xls.Chart.AxisPosition.Right;
                }
            }
        }

        internal static void SetExcelChartBasicSeriesSettings(SpreadChart chart, SpreadDataSeries series, IExcelChartSeriesBase excelSeries, int index)
        {
            excelSeries.SeriesValue = series.ToExcelSeriesValue();
            excelSeries.SeriesName = series.ToExcelSeriesName();
            DataLabelSettings actualDataLabelSettings = series.ActualDataLabelSettings;
            if (actualDataLabelSettings != null)
            {
                excelSeries.DataLabels = actualDataLabelSettings.ToExcelDataLabels();
                SetExcelDataLabelsStyle(series.DataLabelStyle, excelSeries.DataLabels);
            }
            series.GetDataPoints();
            excelSeries.DataPoints = series.GetDataPoints().ToExcelDataPoints();
            excelSeries.Format = series.ToExcelFillFormat();
            excelSeries.Index = index;
            excelSeries.Order = index;
            if (!chart.AxisX.AutomaticAxisData && (chart.ChartType != SpreadChartType.StockHighLowOpenClose))
            {
                if (((chart.ChartType == SpreadChartType.BarClustered) || (chart.ChartType == SpreadChartType.BarStacked)) || (chart.ChartType == SpreadChartType.BarStacked100pc))
                {
                    excelSeries.CategoryAxisData = chart.AxisY.ToExcelCategoryAxisData();
                }
                else
                {
                    excelSeries.CategoryAxisData = chart.AxisX.ToExcelCategoryAxisData();
                }
            }
        }

        internal static void SetExcelChartBasicSeriesSettingsForScatterBubbleChart(SpreadChart chart, SpreadXYDataSeries series, IExcelChartSeriesBase excelSeries, int index)
        {
            excelSeries.SeriesValue = series.ToExcelSeriesValue();
            excelSeries.SeriesName = series.ToExcelSeriesName();
            DataLabelSettings actualDataLabelSettings = series.ActualDataLabelSettings;
            if (actualDataLabelSettings != null)
            {
                excelSeries.DataLabels = actualDataLabelSettings.ToExcelDataLabels();
                SetExcelDataLabelsStyle(series.DataLabelStyle, excelSeries.DataLabels);
            }
            series.GetDataPoints();
            excelSeries.DataPoints = series.GetDataPoints().ToExcelDataPoints();
            excelSeries.Format = series.ToExcelFillFormat();
            excelSeries.Index = index;
            excelSeries.Order = index;
            if (!string.IsNullOrWhiteSpace(series.XValueFormula) || (series.XValues.Count > 0))
            {
                ExcelCategoryAxisData data = new ExcelCategoryAxisData();
                if (!string.IsNullOrWhiteSpace(series.XValueFormula))
                {
                    data.NumberReferencesFormula = series.XValueFormula;
                }
                else if (series.XValues.Count > 0)
                {
                    data.NumericLiterals = new NumericDataLiterals();
                    for (int i = 0; i < series.Values.Count; i++)
                    {
                        ExcelNumberPoint point = new ExcelNumberPoint
                        {
                            Index = i
                        };
                        double num2 = series.Values[i];
                        point.Value = ((double)num2).ToString((IFormatProvider)CultureInfo.InvariantCulture);
                        data.NumericLiterals.NumberPoints.Add(point);
                    }
                }
                if ((series.XValueFormatter != null) && (series.XValueFormatter.FormatString != "General"))
                {
                    data.FormatCode = series.XValueFormatter.FormatString;
                }
                excelSeries.CategoryAxisData = data;
            }
        }

        internal static void SetExcelChartBasicSettings(SpreadChart chart, IExcelChartBase excelChart)
        {
            if ((chart != null) && (excelChart != null))
            {
                if (chart.DataLabelSettings != null)
                {
                    excelChart.DataLabels = chart.DataLabelSettings.ToExcelDataLabels();
                    SetExcelDataLabelsStyle(chart.DataLabelStyleInfo, excelChart.DataLabels);
                }
                excelChart.VaryColors = chart.VaryColorsByPoint;
            }
        }

        internal static void SetExcelDataLabelsStyle(ChartLabelStyleInfo chartLabelStyleInfo, IExcelDataLabels dataLabels)
        {
            if (chartLabelStyleInfo != null)
            {
                if (chartLabelStyleInfo.Formatter != null)
                {
                    dataLabels.NumberFormat = chartLabelStyleInfo.Formatter.FormatString;
                }
                dataLabels.ShapeFormat = CreateExcelChartFormat(chartLabelStyleInfo.FillThemeColor, chartLabelStyleInfo.Fill, chartLabelStyleInfo.StrokeThemeColor, chartLabelStyleInfo.Stroke, chartLabelStyleInfo.StrokeThickness, false, false, chartLabelStyleInfo.StrokeDashType, 1.0, (ExcelDrawingColorSettings)chartLabelStyleInfo.FillDrawingColorSettings, (ExcelDrawingColorSettings)chartLabelStyleInfo.StrokeDrawingColorSettings);
                if ((chartLabelStyleInfo != null) && ((((chartLabelStyleInfo.FontFamily != null) || (chartLabelStyleInfo.Foreground != null)) || (!string.IsNullOrWhiteSpace(chartLabelStyleInfo.ForegroundThemeColor) || !(chartLabelStyleInfo.FontSize + 1.0).IsZero())) || (FontWeightsIsBold(chartLabelStyleInfo.FontWeight) || (chartLabelStyleInfo.FontStyle != FontStyle.Normal))))
                {
                    if (dataLabels.TextFormat == null)
                    {
                        dataLabels.TextFormat = new ExcelTextFormat();
                        dataLabels.TextFormat.TextParagraphs.Add(new TextParagraph());
                    }
                    if (chartLabelStyleInfo.FontFamily != null)
                    {
                        RichTextUtility.SetFontFamily(dataLabels.TextFormat.TextParagraphs, chartLabelStyleInfo.FontFamily.GetFontName());
                    }
                    RichTextUtility.SetRichTextFill(dataLabels.TextFormat.TextParagraphs, chartLabelStyleInfo.ActualForeground);
                    RichTextUtility.SetRichTextFontSize(dataLabels.TextFormat.TextParagraphs, chartLabelStyleInfo.ActualFontSize);
                    if (chartLabelStyleInfo.FontStyle == FontStyle.Italic)
                    {
                        RichTextUtility.SetRichTextFontStyle(dataLabels.TextFormat.TextParagraphs, true);
                    }
                    if (FontWeightsIsBold(chartLabelStyleInfo.FontWeight))
                    {
                        RichTextUtility.SetRichtTextFontWeight(dataLabels.TextFormat.TextParagraphs, true);
                    }
                    else
                    {
                        RichTextUtility.SetRichtTextFontWeight(dataLabels.TextFormat.TextParagraphs, false);
                    }
                }
            }
        }

        internal static void SetExcelLineChartSettings(SpreadChart chart, IExcelLineChartBase excelChart, IEnumerable<SpreadDataSeries> series)
        {
            SetExcelChartBasicSettings(chart, excelChart);
            foreach (SpreadDataSeries series2 in series)
            {
                if (series2 != null)
                {
                    ExcelLineSeries excelSeries = new ExcelLineSeries();
                    SetExcelChartBasicSeriesSettings(chart, series2, excelSeries, DataSeriesCounter.Index);
                    if ((series2.ChartType == SpreadChartType.LineSmoothed) || (series2.ChartType == SpreadChartType.LineWithMarkersSmoothed))
                    {
                        excelSeries.Smoothing = true;
                    }
                    else
                    {
                        excelSeries.Smoothing = false;
                    }
                    if (((series2.ChartType == SpreadChartType.LineStacked100pcWithMarkers) || (series2.ChartType == SpreadChartType.LineStackedWithMarkers)) || ((series2.ChartType == SpreadChartType.LineWithMarkers) || (series2.ChartType == SpreadChartType.LineWithMarkersSmoothed)))
                    {
                        excelSeries.Marker = series2.ToExcelDataMarker();
                    }
                    else
                    {
                        ExcelDataMarker marker = new ExcelDataMarker
                        {
                            MarkerStyle = DataPointMarkStyle.None
                        };
                        excelSeries.Marker = marker;
                    }
                    excelSeries.Trendlines = GetExcelTrendLines(series2.TrendLines);
                    excelSeries.ErrorBars = series2.FirstErrorBars.ToExcelErrorBars();
                    excelChart.LineSeries.Add(excelSeries);
                    DataSeriesCounter.Index++;
                }
            }
            excelChart.AxisX = chart.AxisX.ToExcelChartAxis();
            if (chart.AxisX.AxisPosition == Dt.Cells.Data.AxisPosition.Near)
            {
                excelChart.AxisX.AxisPosition = Dt.Xls.Chart.AxisPosition.Bottom;
            }
            else
            {
                excelChart.AxisX.AxisPosition = Dt.Xls.Chart.AxisPosition.Top;
            }
            excelChart.AxisY = chart.AxisY.ToExcelChartAxis();
            if (chart.AxisY.AxisPosition == Dt.Cells.Data.AxisPosition.Near)
            {
                excelChart.AxisY.AxisPosition = Dt.Xls.Chart.AxisPosition.Left;
            }
            else
            {
                excelChart.AxisY.AxisPosition = Dt.Xls.Chart.AxisPosition.Right;
            }
        }

        internal static void SetExcelPieChartSettings(SpreadChart chart, IExcelPieChartBase excelChart, IEnumerable<SpreadDataSeries> series)
        {
            SetExcelChartBasicSettings(chart, excelChart);
            foreach (SpreadDataSeries series2 in series)
            {
                if (series2 != null)
                {
                    ExcelPieSeries excelSeries = new ExcelPieSeries();
                    SetExcelChartBasicSeriesSettings(chart, series2, excelSeries, DataSeriesCounter.Index);
                    if ((chart.PieChartSettings != null) && chart.ChartType.ToString().ToLower().Contains("exploded"))
                    {
                        excelSeries.Explosion = (int)(chart.PieChartSettings.Explosion * 100.0);
                    }
                    excelChart.PieSeries.Add(excelSeries);
                    DataSeriesCounter.Index++;
                }
            }
        }

        internal static void SetExcelRadarChartSettings(SpreadChart chart, IExcelRadarChart excelChart, IEnumerable<SpreadDataSeries> series)
        {
            SetExcelChartBasicSettings(chart, excelChart);
            foreach (SpreadDataSeries series2 in series)
            {
                if (series2 != null)
                {
                    ExcelRadarSeries excelSeries = new ExcelRadarSeries();
                    SetExcelChartBasicSeriesSettings(chart, series2, excelSeries, DataSeriesCounter.Index);
                    if (series2.ChartType == SpreadChartType.RadarWithMarkers)
                    {
                        excelSeries.Marker = series2.ToExcelDataMarker();
                    }
                    else
                    {
                        ExcelDataMarker marker = new ExcelDataMarker
                        {
                            MarkerStyle = DataPointMarkStyle.None
                        };
                        excelSeries.Marker = marker;
                    }
                    excelChart.RadarSeries.Add(excelSeries);
                    DataSeriesCounter.Index++;
                }
            }
            excelChart.AxisX = chart.AxisX.ToExcelChartAxis();
            excelChart.AxisY = chart.AxisY.ToExcelChartAxis();
        }

        internal static void SetExcelScatterChartSettings(SpreadChart chart, IExcelScatterChart excelChart, IEnumerable<SpreadXYDataSeries> series)
        {
            SetExcelChartBasicSettings(chart, excelChart);
            foreach (SpreadXYDataSeries series2 in series)
            {
                if (series2 != null)
                {
                    ExcelScatterSeries excelSeries = new ExcelScatterSeries();
                    SetExcelChartBasicSeriesSettingsForScatterBubbleChart(chart, series2, excelSeries, DataSeriesCounter.Index);
                    if ((series2.ChartType == SpreadChartType.ScatterLinesSmoothed) || (series2.ChartType == SpreadChartType.ScatterLinesSmoothedWithMarkers))
                    {
                        excelSeries.Smoothing = true;
                    }
                    else
                    {
                        excelSeries.Smoothing = false;
                    }
                    excelSeries.Marker = series2.ToExcelDataMarker();
                    if ((excelSeries.Marker == null) && ((series2.ChartType == SpreadChartType.ScatterLines) || (series2.ChartType == SpreadChartType.ScatterLinesSmoothed)))
                    {
                        excelSeries.Marker = new ExcelDataMarker();
                        excelSeries.Marker.MarkerStyle = DataPointMarkStyle.None;
                    }
                    excelSeries.FirstErrorBars = series2.FirstErrorBars.ToExcelErrorBars();
                    excelSeries.SecondErrorBars = series2.SecondErrorBars.ToExcelErrorBars();
                    excelSeries.Trendlines = GetExcelTrendLines(series2.TrendLines);
                    if (chart.ChartType == SpreadChartType.Scatter)
                    {
                        if (excelSeries.Format == null)
                        {
                            excelSeries.Format = new ExcelChartFormat();
                        }
                        LineFormat format = new LineFormat
                        {
                            FillFormat = new NoFillFormat()
                        };
                        excelSeries.Format.LineFormat = format;
                    }
                    excelChart.ScatterSeries.Add(excelSeries);
                    DataSeriesCounter.Index++;
                }
            }
            excelChart.AxisX = chart.AxisX.ToExcelChartAxis();
            if (excelChart.AxisX != null)
            {
                if (chart.AxisX.AxisPosition == Dt.Cells.Data.AxisPosition.Near)
                {
                    excelChart.AxisX.AxisPosition = Dt.Xls.Chart.AxisPosition.Bottom;
                }
                else
                {
                    excelChart.AxisX.AxisPosition = Dt.Xls.Chart.AxisPosition.Top;
                }
            }
            excelChart.AxisY = chart.AxisY.ToExcelChartAxis();
            if (excelChart.AxisY != null)
            {
                if (chart.AxisY.AxisPosition == Dt.Cells.Data.AxisPosition.Near)
                {
                    excelChart.AxisY.AxisPosition = Dt.Xls.Chart.AxisPosition.Left;
                }
                else
                {
                    excelChart.AxisY.AxisPosition = Dt.Xls.Chart.AxisPosition.Right;
                }
            }
        }

        internal static void SetExcelStokeChartSettings(SpreadChart chart, IExcelStockChart excelChart, SpreadOpenHighLowCloseSeries openHighLowCloseSeries, int startIndex)
        {
            SetExcelChartBasicSettings(chart, excelChart);
            int index = startIndex;
            SpreadDataSeries[] seriesArray = new SpreadDataSeries[] { openHighLowCloseSeries.IsOpenSeriesAutomatic ? null : openHighLowCloseSeries.OpenSeries, openHighLowCloseSeries.HighSeries, openHighLowCloseSeries.LowSeries, openHighLowCloseSeries.CloseSeries };
            foreach (SpreadDataSeries series in seriesArray)
            {
                if (series != null)
                {
                    ExcelLineSeries excelSeries = new ExcelLineSeries();
                    SetExcelChartBasicSeriesSettings(chart, series, excelSeries, index);
                    if (!string.IsNullOrWhiteSpace(openHighLowCloseSeries.XValueFormula))
                    {
                        ExcelCategoryAxisData data = new ExcelCategoryAxisData();
                        if (!string.IsNullOrWhiteSpace(openHighLowCloseSeries.XValueFormula))
                        {
                            data.NumberReferencesFormula = openHighLowCloseSeries.XValueFormula;
                        }
                        if ((openHighLowCloseSeries.XValueFormatter != null) && (openHighLowCloseSeries.XValueFormatter.FormatString != "General"))
                        {
                            data.FormatCode = openHighLowCloseSeries.XValueFormatter.FormatString;
                        }
                        excelSeries.CategoryAxisData = data;
                    }
                    if ((series.ChartType == SpreadChartType.LineSmoothed) || (series.ChartType == SpreadChartType.LineWithMarkersSmoothed))
                    {
                        excelSeries.Smoothing = true;
                    }
                    else
                    {
                        excelSeries.Smoothing = false;
                    }
                    excelSeries.Marker = series.ToExcelDataMarker();
                    excelChart.LineSeries.Add(excelSeries);
                    index++;
                }
            }
            excelChart.AxisX = chart.AxisX.StockChartAxisXToExcelChartAxis();
            if (chart.AxisX.AxisPosition == Dt.Cells.Data.AxisPosition.Near)
            {
                excelChart.AxisX.AxisPosition = Dt.Xls.Chart.AxisPosition.Bottom;
            }
            else
            {
                excelChart.AxisX.AxisPosition = Dt.Xls.Chart.AxisPosition.Top;
            }
            excelChart.AxisY = chart.AxisY.ToExcelChartAxis();
            if (chart.AxisY.AxisPosition == Dt.Cells.Data.AxisPosition.Near)
            {
                excelChart.AxisY.AxisPosition = Dt.Xls.Chart.AxisPosition.Left;
            }
            else
            {
                excelChart.AxisY.AxisPosition = Dt.Xls.Chart.AxisPosition.Right;
            }
        }

        internal static void SetNegativeFillForamt(this SpreadDataSeries series, SolidFillFormat solidFill, Workbook workbook)
        {
            if ((series != null) && (solidFill != null))
            {
                object obj2 = GetSolidFill(solidFill, workbook);
                if (obj2 is string)
                {
                    series.NegativeFillThemeColor = (string)(obj2 as string);
                }
                else if (obj2 is SolidColorBrush)
                {
                    series.NegativeFill = obj2 as SolidColorBrush;
                }
                else if (obj2 is Brush)
                {
                    series.NegativeFill = obj2 as Brush;
                }
            }
        }

        internal static void SetSeriesName(this SpreadDataSeries spreadDataSeries, IExcelSeriesName seriesName)
        {
            if (seriesName != null)
            {
                if (seriesName.IsReferenceFormula && !string.IsNullOrWhiteSpace(seriesName.ReferenceFormula))
                {
                    spreadDataSeries.NameFormula = seriesName.ReferenceFormula;
                }
                else if (!string.IsNullOrWhiteSpace(seriesName.TextValue))
                {
                    spreadDataSeries.Name = seriesName.TextValue;
                }
            }
        }

        internal static void SetSeriesValue(this SpreadDataSeries spreadDataSeries, IExcelSeriesValue seriesValue)
        {
            if (seriesValue != null)
            {
                if (!string.IsNullOrWhiteSpace(seriesValue.ReferenceFormula))
                {
                    spreadDataSeries.ValueFormula = seriesValue.ReferenceFormula;
                    if (!string.IsNullOrWhiteSpace(seriesValue.FormatCode) && (seriesValue.FormatCode != "General"))
                    {
                        spreadDataSeries.Formatter = new GeneralFormatter(seriesValue.FormatCode);
                    }
                }
                else if ((seriesValue.NumericLiterals != null) && (seriesValue.NumericLiterals.NumberPoints != null))
                {
                    int num = seriesValue.NumericLiterals.NumberPoints.Count;
                    for (int i = 0; i < num; i++)
                    {
                        spreadDataSeries.Values.Add(0.0);
                    }
                    foreach (ExcelNumberPoint point in seriesValue.NumericLiterals.NumberPoints)
                    {
                        string formatCode = point.FormatCode;
                        if (string.IsNullOrWhiteSpace(formatCode))
                        {
                            formatCode = seriesValue.NumericLiterals.FormatCode;
                        }
                        if (string.IsNullOrWhiteSpace(formatCode))
                        {
                            formatCode = seriesValue.FormatCode;
                        }
                        if (string.IsNullOrWhiteSpace(formatCode))
                        {
                            formatCode = "General";
                        }
                        double num3 = 0.0;
                        if (double.TryParse(point.Value, (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out num3))
                        {
                            spreadDataSeries.Values[point.Index] = num3;
                        }
                        else
                        {
                            spreadDataSeries.Values[point.Index] = 0.0;
                        }
                    }
                }
            }
        }

        internal static void SetSpreadAxisStyle(Axis axis, IExcelChartFormat format)
        {
            if ((format != null) && (format.LineFormat != null))
            {
                if (format.LineFormat.JoinType == EndLineJoinType.Bevel)
                {
                    axis.LineJoinType = PenLineJoin.Bevel;
                }
                else if (format.LineFormat.JoinType == EndLineJoinType.Miter)
                {
                    axis.LineJoinType = PenLineJoin.Miter;
                }
                else
                {
                    axis.LineJoinType = PenLineJoin.Round;
                }
                if (format.LineFormat.LineEndingCap == EndLineCap.Flat)
                {
                    axis.LineCapType = PenLineCap.Flat;
                }
                else if (format.LineFormat.LineEndingCap == EndLineCap.Round)
                {
                    axis.LineCapType = PenLineCap.Round;
                }
                else
                {
                    axis.LineCapType = PenLineCap.Square;
                }
            }
        }

        internal static void SetSpreadChartElementStyle(SpreadChartElement chartElement, IExcelChartFormat format, Workbook workbook)
        {
            if (((chartElement != null) && (format != null)) && (workbook != null))
            {
                object fillBrush = GetFillBrush(format, workbook);
                if (fillBrush is string)
                {
                    chartElement.FillThemeColor = (string)(fillBrush as string);
                }
                else if (fillBrush is Brush)
                {
                    chartElement.Fill = fillBrush as Brush;
                }
                else if (IsNoFill(format))
                {
                    chartElement.IsAutomaticFill = false;
                }
                else if (format.FillFormat == null)
                {
                    chartElement.IsAutomaticFill = true;
                }
                chartElement.FillDrawingColorSettings = GetSpreadDrawingColorSettings(format.FillFormat);
                object stokeBrush = GetStokeBrush(format, workbook);
                if (stokeBrush is string)
                {
                    chartElement.StrokeThemeColor = (string)(stokeBrush as string);
                }
                else if (stokeBrush is Brush)
                {
                    chartElement.Stroke = stokeBrush as Brush;
                }
                else if (IsLineNoFill(format))
                {
                    chartElement.IsAutomaticStroke = false;
                }
                else if ((format.LineFormat != null) && (format.LineFormat.FillFormat == null))
                {
                    chartElement.IsAutomaticStroke = true;
                }
                if (format.LineFormat != null)
                {
                    chartElement.StrokeDrawingColorSettings = GetSpreadDrawingColorSettings(format.LineFormat.FillFormat);
                }
                if ((format != null) && (format.LineFormat != null))
                {
                    chartElement.StrokeThickness = format.LineFormat.Width;
                    chartElement.StrokeDashType = GetStrokeDashType(format.LineFormat.LineDashType);
                    chartElement.JoinType = format.LineFormat.JoinType;
                    chartElement.LineEndingCap = format.LineFormat.LineEndingCap;
                    chartElement.CompoundLineType = format.LineFormat.CompoundLineType;
                    chartElement.PenAlignment = format.LineFormat.PenAlignment;
                    chartElement.HeadLineEndStyle = format.LineFormat.HeadLineEndStyle;
                    chartElement.TailLineEndStyle = format.LineFormat.TailLineEndStyle;
                    chartElement.LineDashType = format.LineFormat.LineDashType;
                }
            }
        }

        internal static void SetSpreadDataLabelStyleInfo(ChartLabelStyleInfo chartLabelStyleInfo, IExcelDataLabels dataLabels, Workbook workbook)
        {
            ChartLabelStyleInfo result;
            if (chartLabelStyleInfo != null)
            {
                result = chartLabelStyleInfo;
                if (dataLabels.TextFormat != null)
                {
                    result.Foreground = RichTextUtility.GetRichTextFill(dataLabels.TextFormat.TextParagraphs, workbook);
                    double? richTextFontSize = RichTextUtility.GetRichTextFontSize(dataLabels.TextFormat.TextParagraphs);
                    if (richTextFontSize.HasValue && richTextFontSize.HasValue)
                    {
                        result.FontSize = richTextFontSize.Value;
                    }
                    result.FontFamily = RichTextUtility.GetRichTextFamily(dataLabels.TextFormat.TextParagraphs);
                    result.FontStyle = RichTextUtility.GetRichTextFontStyle(dataLabels.TextFormat.TextParagraphs);
                    result.FontWeight = RichTextUtility.GetRichTextFontWeight(dataLabels.TextFormat.TextParagraphs);
                }
                string numberFormat = dataLabels.NumberFormat;
                if (!string.IsNullOrEmpty(numberFormat))
                {
                    result.Formatter = new GeneralFormatter(numberFormat);
                }
                IExcelChartFormat shapeFormat = dataLabels.ShapeFormat;
                if ((shapeFormat != null) && (workbook != null))
                {
                    object fillBrush = GetFillBrush(shapeFormat, workbook);
                    if (fillBrush is string)
                    {
                        result.FillThemeColor = (string)(fillBrush as string);
                    }
                    else if (fillBrush is Brush)
                    {
                        result.Fill = fillBrush as Brush;
                    }
                    result.FillDrawingColorSettings = GetSpreadDrawingColorSettings(shapeFormat.FillFormat);
                    object stokeBrush = GetStokeBrush(shapeFormat, workbook);
                    if (stokeBrush is string)
                    {
                        result.StrokeThemeColor = (string)(stokeBrush as string);
                    }
                    else if (stokeBrush is Brush)
                    {
                        result.Stroke = stokeBrush as Brush;
                    }
                    if (shapeFormat.LineFormat != null)
                    {
                        result.StrokeDrawingColorSettings = GetSpreadDrawingColorSettings(shapeFormat.LineFormat.FillFormat);
                    }
                    double width = 0.0;
                    if ((shapeFormat != null) && (shapeFormat.LineFormat != null))
                    {
                        width = shapeFormat.LineFormat.Width;
                    }
                    result.StrokeThickness = width;
                }
            }
        }

        static void SetSpreadXYDataSeries(SpreadXYDataSeries xyDataSeries, IExcelChartSeriesBase excelSeries, Workbook workbook)
        {
            SetDataSeries(xyDataSeries, excelSeries, workbook);
            if (excelSeries.CategoryAxisData != null)
            {
                ExcelCategoryAxisData categoryAxisData = excelSeries.CategoryAxisData as ExcelCategoryAxisData;
                if (!string.IsNullOrWhiteSpace(categoryAxisData.StringReferencedFormula))
                {
                    xyDataSeries.XValueFormula = categoryAxisData.StringReferencedFormula;
                }
                else if (!string.IsNullOrWhiteSpace(categoryAxisData.NumberReferencesFormula))
                {
                    xyDataSeries.XValueFormula = categoryAxisData.NumberReferencesFormula;
                }
                else if (!string.IsNullOrWhiteSpace(categoryAxisData.MultiLevelStringReferenceFormula))
                {
                    xyDataSeries.XValueFormula = categoryAxisData.MultiLevelStringReferenceFormula;
                }
                else if (categoryAxisData.NumericLiterals != null)
                {
                    string format = !string.IsNullOrWhiteSpace(categoryAxisData.NumericLiterals.FormatCode) ? categoryAxisData.NumericLiterals.FormatCode : categoryAxisData.FormatCode;
                    GeneralFormatter formatter = new GeneralFormatter(format);
                    xyDataSeries.XValueFormatter = formatter;
                    for (int i = 0; i < categoryAxisData.NumericLiterals.NumberPoints.Count; i++)
                    {
                        xyDataSeries.XValues.Add(0.0);
                    }
                    foreach (ExcelNumberPoint point in categoryAxisData.NumericLiterals.NumberPoints)
                    {
                        double num2 = 0.0;
                        double.TryParse(point.Value, (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out num2);
                        xyDataSeries.XValues[point.Index] = num2;
                    }
                }
                else if (categoryAxisData.StringLiterals != null)
                {
                    string formatCode = categoryAxisData.FormatCode;
                    if (string.IsNullOrWhiteSpace(formatCode))
                    {
                        formatCode = "General";
                    }
                    xyDataSeries.Formatter = new GeneralFormatter(formatCode);
                    foreach (string str3 in categoryAxisData.StringLiterals.StringLiteralDatas)
                    {
                        double item = 0.0;
                        double.TryParse(str3, (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out item);
                        xyDataSeries.XValues.Add(item);
                    }
                }
            }
        }

        internal static void SetStyle(this SpreadDataSeries series, IExcelChartFormat format, Workbook workbook)
        {
            SetSpreadChartElementStyle(series, format, workbook);
        }

        internal static void SetTrendLine(this SpreadDataSeries spreadDataSeries, List<IExcelTrendLine> trendlines, Workbook workbook)
        {
            if ((trendlines != null) && (trendlines.Count != 0))
            {
                foreach (IExcelTrendLine line in Enumerable.OrderBy<IExcelTrendLine, int>((IEnumerable<IExcelTrendLine>)trendlines, delegate (IExcelTrendLine item)
                {
                    return item.Order;
                }))
                {
                    spreadDataSeries.TrendLines.Add(line.ToSpreadTrendLine(workbook));
                }
            }
        }

        internal static IExcelChartAxis StockChartAxisXToExcelChartAxis(this Axis axis)
        {
            if ((axis.AxisType == AxisType.Category) || (axis.AxisType == AxisType.Value))
            {
                if (axis.IsSerAxis)
                {
                    ExcelChartSeriesAxis axis2 = new ExcelChartSeriesAxis();
                    SetExcelAxis(axis, axis2);
                    axis2.TickLalelInterval = axis.TickLabelInterval;
                    axis2.TickMarkInterval = axis.TickMarkInterval;
                    axis2.ShapeFormat = axis.GetExcelChartForamt();
                    SetExcelAxisStyle(axis2.ShapeFormat, axis);
                    SetExcelAxisTextFormat(axis2, axis);
                    if (axis.Id == 0)
                    {
                        axis.Id = GenerateAxisId();
                    }
                    axis2.Id = axis.Id;
                    return axis2;
                }
                ExcelChartCategoryAxis axis3 = new ExcelChartCategoryAxis();
                SetExcelAxis(axis, axis3);
                axis3.LabelOffset = axis.LableOffset;
                axis3.TickLalelInterval = axis.TickLabelInterval;
                axis3.TickMarkInterval = axis.TickMarkInterval;
                axis3.ShapeFormat = axis.GetExcelChartForamt();
                SetExcelAxisStyle(axis3.ShapeFormat, axis);
                SetExcelAxisTextFormat(axis3, axis);
                if (axis.Id == 0)
                {
                    axis.Id = GenerateAxisId();
                }
                axis3.Id = axis.Id;
                return axis3;
            }
            if (axis.AxisType != AxisType.Date)
            {
                return null;
            }
            ExcelChartDateAxis excelAxis = new ExcelChartDateAxis();
            SetExcelAxis(axis, excelAxis);
            if (!axis.AutoMajorUnit)
            {
                excelAxis.MajorUnit = (int)Math.Ceiling(axis.MajorUnit);
            }
            if (!axis.AutoMinorUnit)
            {
                excelAxis.MinorUnit = (int)Math.Ceiling(axis.MinorUnit);
            }
            excelAxis.MajorTimeUnit = (Dt.Xls.Chart.AxisTimeUnit)axis.MajorTimeUnit;
            excelAxis.MinorTimeUnit = (Dt.Xls.Chart.AxisTimeUnit)axis.MinorTimeUnit;
            excelAxis.BaseTimeUnit = (Dt.Xls.Chart.AxisTimeUnit)axis.BaseTimeUnit;
            excelAxis.LabelOffset = axis.LableOffset;
            excelAxis.IsAutomaticCategoryAxis = false;
            excelAxis.ShapeFormat = axis.GetExcelChartForamt();
            SetExcelAxisStyle(excelAxis.ShapeFormat, axis);
            SetExcelAxisTextFormat(excelAxis, axis);
            if (axis.Id == 0)
            {
                axis.Id = GenerateAxisId();
            }
            excelAxis.Id = axis.Id;
            return excelAxis;
        }

        internal static ArrowSettings ToArrowSettings(this LineEndStyle lineEndStyle)
        {
            if (lineEndStyle == null)
            {
                return null;
            }
            return new ArrowSettings { Length = (ArrowSize)lineEndStyle.Length, Width = (ArrowSize)lineEndStyle.Width, Type = (ArrowType)lineEndStyle.Type };
        }

        internal static IExcelAreaChart ToExcelAreaChart(this SpreadChart chart)
        {
            if (chart != null)
            {
                List<SpreadDataSeries> list = new List<SpreadDataSeries>();
                ExcelAreaChart excelChart = null;
                foreach (SpreadDataSeries series in chart.DataSeries)
                {
                    if (IsAreaChart(series.ChartType))
                    {
                        list.Add(series);
                    }
                }
                if (list.Count > 0)
                {
                    excelChart = new ExcelAreaChart(GetExcelChartType(list[0].ChartType));
                }
                else if (IsAreaChart(chart.ChartType) && (chart.DataSeries.Count == 0))
                {
                    excelChart = new ExcelAreaChart(GetExcelChartType(chart.ChartType));
                }
                if (excelChart != null)
                {
                    SetExcelAreaChartSettings(chart, excelChart, (IEnumerable<SpreadDataSeries>)list);
                    excelChart.DropLine = chart.DropLine;
                    return excelChart;
                }
            }
            return null;
        }

        internal static IExcelBarChart ToExcelBarChart(this SpreadChart chart)
        {
            if (chart == null)
            {
                return null;
            }
            List<SpreadDataSeries> list = new List<SpreadDataSeries>();
            ExcelBarChart excelChart = null;
            foreach (SpreadDataSeries series in chart.DataSeries)
            {
                if (IsBarChart(series.ChartType))
                {
                    list.Add(series);
                }
            }
            if (list.Count > 0)
            {
                excelChart = new ExcelBarChart(GetExcelChartType(list[0].ChartType));
            }
            else if (IsBarChart(chart.ChartType) && (chart.DataSeries.Count == 0))
            {
                excelChart = new ExcelBarChart(GetExcelChartType(chart.ChartType));
            }
            if (excelChart == null)
            {
                return null;
            }
            SetExcelBarChartBaseSettings(chart, excelChart, (IEnumerable<SpreadDataSeries>)list);
            if (chart.DataSeriesSettings != null)
            {
                if (!double.IsNaN(chart.DataSeriesSettings.SeriesOverlap))
                {
                    excelChart.Overlap = (int)(chart.DataSeriesSettings.SeriesOverlap * 100.0);
                }
                else
                {
                    excelChart.Overlap = 0;
                }
                if (!double.IsNaN(chart.DataSeriesSettings.GapWidth))
                {
                    excelChart.GapWidth = (int)(chart.DataSeriesSettings.GapWidth * 100.0);
                }
                else
                {
                    excelChart.GapWidth = 150;
                }
            }
            excelChart.VaryColors = chart.VaryColorsByPoint;
            excelChart.SeriesLines = chart.SeriesLines;
            return excelChart;
        }

        internal static IExcelBubbleChart ToExcelBubbleChart(this SpreadChart chart)
        {
            if (chart == null)
            {
                return null;
            }
            List<SpreadBubbleSeries> list = new List<SpreadBubbleSeries>();
            ExcelBubbleChart excelChart = null;
            foreach (SpreadDataSeries series in chart.DataSeries)
            {
                if (series is SpreadBubbleSeries)
                {
                    list.Add(series as SpreadBubbleSeries);
                }
            }
            if (list.Count > 0)
            {
                GetExcelChartType(list[0].ChartType);
                excelChart = new ExcelBubbleChart();
            }
            else if (IsBubbleChart(chart.ChartType) && (chart.DataSeries.Count == 0))
            {
                GetExcelChartType(chart.ChartType);
                excelChart = new ExcelBubbleChart();
            }
            if (excelChart == null)
            {
                return null;
            }
            SetExcelBubbleChartSettings(chart, excelChart, (IEnumerable<SpreadBubbleSeries>)list);
            if (chart.BubbleChartSettings != null)
            {
                excelChart.SizeRepresents = (Dt.Xls.Chart.BubbleSizeRepresents)chart.BubbleChartSettings.BubbleSizeRepresents;
                excelChart.BubbleScale = (int)(chart.BubbleChartSettings.BubbleScale * 100.0);
                excelChart.ShowNegativeBubbles = chart.BubbleChartSettings.ShowNegativeBubble;
            }
            return excelChart;
        }

        static IExcelChartCategoryAxisData ToExcelCategoryAxisData(this Axis axis)
        {
            if (axis == null)
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(axis.ItemsFormula) && ((axis.Items == null) || (axis.Items.Count == 0)))
            {
                return null;
            }
            ExcelCategoryAxisData data = new ExcelCategoryAxisData();
            if (!string.IsNullOrWhiteSpace(axis.ItemsFormula))
            {
                data.StringReferencedFormula = axis.ItemsFormula;
            }
            else if (axis.Items.Count > 0)
            {
                bool flag = true;
                foreach (object obj2 in axis.Items)
                {
                    double num = 0.0;
                    if (!double.TryParse(Convert.ToString(obj2, (IFormatProvider)CultureInfo.InvariantCulture), (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out num))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    data.NumericLiterals = new NumericDataLiterals();
                    for (int i = 0; i < axis.Items.Count; i++)
                    {
                        object obj3 = axis.Items[i];
                        ExcelNumberPoint point = new ExcelNumberPoint
                        {
                            Index = i,
                            Value = Convert.ToString(obj3, (IFormatProvider)CultureInfo.InvariantCulture)
                        };
                        data.NumericLiterals.NumberPoints.Add(point);
                    }
                }
                else
                {
                    data.StringLiterals = new StringLiteralData();
                    for (int j = 0; j < axis.Items.Count; j++)
                    {
                        data.StringLiterals.StringLiteralDatas.Add(Convert.ToString(axis.Items[j], (IFormatProvider)CultureInfo.InvariantCulture));
                    }
                }
            }
            if ((axis.LabelFormatter != null) && (axis.LabelFormatter.FormatString != "General"))
            {
                data.FormatCode = axis.LabelFormatter.FormatString;
            }
            return data;
        }

        internal static IExcelChartAxis ToExcelChartAxis(this Axis axis)
        {
            if (axis == null)
            {
                return null;
            }
            if (axis.AxisType == AxisType.Category)
            {
                if (axis.IsSerAxis)
                {
                    ExcelChartSeriesAxis axis2 = new ExcelChartSeriesAxis();
                    SetExcelAxis(axis, axis2);
                    axis2.TickLalelInterval = axis.TickLabelInterval;
                    axis2.TickMarkInterval = axis.TickMarkInterval;
                    axis2.ShapeFormat = axis.GetExcelChartForamt();
                    SetExcelAxisStyle(axis2.ShapeFormat, axis);
                    SetExcelAxisTextFormat(axis2, axis);
                    if (axis.Id == 0)
                    {
                        axis.Id = GenerateAxisId();
                    }
                    axis2.Id = axis.Id;
                    return axis2;
                }
                ExcelChartCategoryAxis axis3 = new ExcelChartCategoryAxis();
                SetExcelAxis(axis, axis3);
                axis3.LabelOffset = axis.LableOffset;
                axis3.TickLalelInterval = axis.TickLabelInterval;
                axis3.TickMarkInterval = axis.TickMarkInterval;
                axis3.ShapeFormat = axis.GetExcelChartForamt();
                SetExcelAxisStyle(axis3.ShapeFormat, axis);
                SetExcelAxisTextFormat(axis3, axis);
                if (axis.Id == 0)
                {
                    axis.Id = GenerateAxisId();
                }
                axis3.Id = axis.Id;
                axis3.IsAutomaticCategoryAxis = axis.IsAumaticCategoryAxis;
                axis3.NoMultiLevelLables = axis.NoMultiLevelLables;
                return axis3;
            }
            if (axis.AxisType == AxisType.Value)
            {
                ExcelChartValueAxis axis4 = new ExcelChartValueAxis();
                SetExcelAxis(axis, axis4);
                if (!double.IsNaN(axis.MajorUnit) && !axis.AutoMajorUnit)
                {
                    axis4.MajorUnit = axis.MajorUnit;
                }
                if (!double.IsNaN(axis.MinorUnit) && !axis.AutoMinorUnit)
                {
                    axis4.MinorUnit = axis.MinorUnit;
                }
                axis4.CrossBetween = (CrossBetween)axis.CrossBetween;
                if (axis.DisplayUnitSettings != null)
                {
                    DisplayUnits units = new DisplayUnits();
                    long[] numArray = new long[] { 100L, 0x3e8L, 0x2710L, 0x186a0L, 0xf4240L, 0x989680L, 0x5f5e100L, 0x3b9aca00L, 0xe8d4a51000L };
                    BuiltInDisplayUnitValue none = BuiltInDisplayUnitValue.None;
                    foreach (long num in numArray)
                    {
                        if ((num - axis.DisplayUnitSettings.DisplayUnit).IsZero())
                        {
                            none = (BuiltInDisplayUnitValue)num;
                        }
                    }
                    if ((axis.DisplayUnitSettings.DisplayUnit > 0.0) && (none != BuiltInDisplayUnitValue.None))
                    {
                        units.BuiltInDisplayUnit = none;
                    }
                    else
                    {
                        units.CustomDisplayUnit = axis.DisplayUnitSettings.DisplayUnit;
                    }
                    units.Layout = axis.DisplayUnitSettings.LabelLayout.ToExcelLayout();
                    if (!string.IsNullOrWhiteSpace(axis.DisplayUnitSettings.LabelText))
                    {
                        units.TextFormula = axis.DisplayUnitSettings.LabelText;
                    }
                    else
                    {
                        units.RichText = axis.DisplayUnitSettings.LabelRichText;
                    }
                    axis4.DislayUnits = units;
                }
                axis4.ShapeFormat = axis.GetExcelChartForamt();
                SetExcelAxisStyle(axis4.ShapeFormat, axis);
                SetExcelAxisTextFormat(axis4, axis);
                if (axis.Id == 0)
                {
                    axis.Id = GenerateAxisId();
                }
                axis4.Id = axis.Id;
                return axis4;
            }
            if (axis.AxisType != AxisType.Date)
            {
                return null;
            }
            ExcelChartDateAxis excelAxis = new ExcelChartDateAxis();
            SetExcelAxis(axis, excelAxis);
            if (!axis.AutoMajorUnit)
            {
                excelAxis.MajorUnit = (int)Math.Ceiling(axis.MajorUnit);
            }
            if (!axis.AutoMinorUnit)
            {
                excelAxis.MinorUnit = (int)Math.Ceiling(axis.MinorUnit);
            }
            excelAxis.MajorTimeUnit = (Dt.Xls.Chart.AxisTimeUnit)axis.MajorTimeUnit;
            excelAxis.MinorTimeUnit = (Dt.Xls.Chart.AxisTimeUnit)axis.MinorTimeUnit;
            excelAxis.BaseTimeUnit = (Dt.Xls.Chart.AxisTimeUnit)axis.BaseTimeUnit;
            excelAxis.LabelOffset = axis.LableOffset;
            excelAxis.IsAutomaticCategoryAxis = false;
            excelAxis.ShapeFormat = axis.GetExcelChartForamt();
            SetExcelAxisStyle(excelAxis.ShapeFormat, axis);
            SetExcelAxisTextFormat(excelAxis, axis);
            if (axis.Id == 0)
            {
                axis.Id = GenerateAxisId();
            }
            excelAxis.Id = axis.Id;
            return excelAxis;
        }

        internal static IExcelChartDataTable ToExcelChartDataTable(this DataTableSettings dataTable)
        {
            if (dataTable == null)
            {
                return null;
            }
            return new ExcelChartDataTable { ShowHorizontalBorder = dataTable.ShowHorizontalBorder, ShowVerticalBorder = dataTable.ShowVerticalBorder, ShowOutlineBorder = dataTable.ShowOutlineBorder, ShowLegendKeys = dataTable.ShowLegendKeys };
        }

        internal static IExcelChartFormat ToExcelChartFormat(this ChartSymbolStyleInfo format)
        {
            if (format == null)
            {
                return null;
            }
            return CreateExcelChartFormat(format.FillThemeColor, format.Fill, format.StrokeThemeColor, format.Stroke, format.StrokeThickness, format.IsAutomaticFill, format.IsAutomaticStroke, format.StrokeDashType, 1.0, (ExcelDrawingColorSettings)format.FillDrawingColorSettings, (ExcelDrawingColorSettings)format.StrokeDrawingColorSettings);
        }

        internal static IExcelChartFormat ToExcelChartFormat(this FloatingObjectStyleInfo format)
        {
            if (format == null)
            {
                return null;
            }
            return CreateExcelChartFormat(format.FillThemeColor, format.Fill, format.StrokeThemeColor, format.Stroke, format.StrokeThickness, false, false, format.StrokeDashType, 1.0, format.FillDrawingColorSettings, format.StrokeDrawingColorSettings);
        }

        internal static IExcelDataLabels ToExcelDataLabels(this DataLabelSettings dataLabelSettings)
        {
            if (dataLabelSettings == null)
            {
                return null;
            }
            ExcelDataLabels labels = new ExcelDataLabels
            {
                ShowCategoryName = dataLabelSettings.ShowCategoryName,
                ShowValue = dataLabelSettings.ShowValue,
                ShowSeriesName = dataLabelSettings.ShowSeriesName,
                ShowLegendKey = dataLabelSettings.IncludeLegendKey,
                Separator = dataLabelSettings.Separator,
                ShowBubbleSize = dataLabelSettings.ShowBubbleSize,
                ShowPercentage = dataLabelSettings.ShowPercent,
                ShowLeaderLines = dataLabelSettings.IncludeLeaderLines,
                Position = (Dt.Xls.Chart.DataLabelPosition)dataLabelSettings.Position
            };
            if (dataLabelSettings.DataLabelList != null)
            {
                foreach (IExcelDataLabel label in dataLabelSettings.DataLabelList)
                {
                    labels.DataLabelList.Add(label);
                }
            }
            labels.TextFormat = dataLabelSettings.TextFormat;
            labels.NumberFormatLinked = dataLabelSettings.NumberFormatLinked;
            return labels;
        }

        internal static ExcelDataMarker ToExcelDataMarker(this DataMarker dataMarker)
        {
            if (dataMarker == null)
            {
                return null;
            }
            ExcelDataMarker marker = new ExcelDataMarker();
            switch (dataMarker.ExcelMarkerType)
            {
                case Dt.Cells.Data.MarkerType.None:
                    marker.MarkerStyle = DataPointMarkStyle.None;
                    break;

                case Dt.Cells.Data.MarkerType.Box:
                    marker.MarkerStyle = DataPointMarkStyle.Square;
                    break;

                case Dt.Cells.Data.MarkerType.Dot:
                    marker.MarkerStyle = DataPointMarkStyle.Dot;
                    break;

                case Dt.Cells.Data.MarkerType.Diamond:
                    marker.MarkerStyle = DataPointMarkStyle.Diamond;
                    break;

                case Dt.Cells.Data.MarkerType.Triangle:
                    marker.MarkerStyle = DataPointMarkStyle.Triangle;
                    break;

                case Dt.Cells.Data.MarkerType.Star4:
                    marker.MarkerStyle = DataPointMarkStyle.Star;
                    break;

                case Dt.Cells.Data.MarkerType.Star8:
                    marker.MarkerStyle = DataPointMarkStyle.Star;
                    break;

                case Dt.Cells.Data.MarkerType.Cross:
                    marker.MarkerStyle = DataPointMarkStyle.Plus;
                    break;

                case Dt.Cells.Data.MarkerType.DiagonalCross:
                    marker.MarkerStyle = DataPointMarkStyle.X;
                    break;

                case Dt.Cells.Data.MarkerType.Circle:
                    marker.MarkerStyle = DataPointMarkStyle.Circle;
                    break;
            }
            Windows.Foundation.Size markerSize = dataMarker.MarkerSize;
            marker.MarkerSize = (int)dataMarker.MarkerSize.Width;
            marker.Format = dataMarker.StyleInfo.ToExcelChartFormat();
            return marker;
        }

        internal static ExcelDataMarker ToExcelDataMarker(this SpreadDataSeries spreadDataSeries)
        {
            if (spreadDataSeries == null)
            {
                return null;
            }
            if (spreadDataSeries.MarkerType == Dt.Cells.Data.MarkerType.Automatic)
            {
                return null;
            }
            ExcelDataMarker marker = new ExcelDataMarker();
            switch (spreadDataSeries.MarkerType)
            {
                case Dt.Cells.Data.MarkerType.None:
                    marker.MarkerStyle = DataPointMarkStyle.None;
                    break;

                case Dt.Cells.Data.MarkerType.Box:
                    marker.MarkerStyle = DataPointMarkStyle.Square;
                    break;

                case Dt.Cells.Data.MarkerType.Dot:
                    marker.MarkerStyle = DataPointMarkStyle.Dot;
                    break;

                case Dt.Cells.Data.MarkerType.Diamond:
                    marker.MarkerStyle = DataPointMarkStyle.Diamond;
                    break;

                case Dt.Cells.Data.MarkerType.Triangle:
                    marker.MarkerStyle = DataPointMarkStyle.Triangle;
                    break;

                case Dt.Cells.Data.MarkerType.Star4:
                    marker.MarkerStyle = DataPointMarkStyle.Star;
                    break;

                case Dt.Cells.Data.MarkerType.Star8:
                    marker.MarkerStyle = DataPointMarkStyle.Star;
                    break;

                case Dt.Cells.Data.MarkerType.Cross:
                    marker.MarkerStyle = DataPointMarkStyle.Plus;
                    break;

                case Dt.Cells.Data.MarkerType.DiagonalCross:
                    marker.MarkerStyle = DataPointMarkStyle.X;
                    break;

                case Dt.Cells.Data.MarkerType.Circle:
                    marker.MarkerStyle = DataPointMarkStyle.Circle;
                    break;
            }
            Windows.Foundation.Size markerSize = spreadDataSeries.MarkerSize;
            marker.MarkerSize = (int)spreadDataSeries.MarkerSize.Width;
            if (spreadDataSeries.DataMarkerStyle != null)
            {
                marker.Format = spreadDataSeries.DataMarkerStyle.ToExcelChartFormat();
            }
            return marker;
        }

        internal static IExcelDataPoint ToExcelDataPoint(this DataPoint dataPoint)
        {
            if (dataPoint == null)
            {
                return null;
            }
            return new ExcelDataPoint { DataPointFormat = dataPoint.GetExcelChartForamt(), InvertIfNegative = dataPoint.InvertIfNegative, IsBubble3D = dataPoint.IsBubble3D, Explosion = dataPoint.Explosion, Marker = dataPoint.DataMarker.ToExcelDataMarker(), PictureOptions = dataPoint.PictureOptions };
        }

        internal static List<IExcelDataPoint> ToExcelDataPoints(this IEnumerable<DataPoint> dps)
        {
            if ((dps == null) || (Enumerable.Count<DataPoint>(dps) == 0))
            {
                return null;
            }
            List<IExcelDataPoint> list = new List<IExcelDataPoint>();
            foreach (DataPoint point in dps)
            {
                IExcelDataPoint point2 = point.ToExcelDataPoint();
                if (point2 != null)
                {
                    point2.Index = point.PointIndex;
                    list.Add(point2);
                }
            }
            if (list.Count == 0)
            {
                return null;
            }
            return list;
        }

        internal static IExcelDoughnutChart ToExcelDoughnutChart(this SpreadChart chart)
        {
            if (chart == null)
            {
                return null;
            }
            List<SpreadDataSeries> list = new List<SpreadDataSeries>();
            ExcelDoughnutChart excelChart = null;
            foreach (SpreadDataSeries series in chart.DataSeries)
            {
                if (IsDoughnutChart(series.ChartType))
                {
                    list.Add(series);
                }
            }
            if (list.Count > 0)
            {
                excelChart = new ExcelDoughnutChart(GetExcelChartType(list[0].ChartType));
            }
            else if (IsDoughnutChart(chart.ChartType))
            {
                excelChart = new ExcelDoughnutChart(GetExcelChartType(chart.ChartType));
            }
            if (excelChart == null)
            {
                return null;
            }
            SetExcelPieChartSettings(chart, excelChart, (IEnumerable<SpreadDataSeries>)list);
            if (chart.PieChartSettings != null)
            {
                excelChart.FirstSliceAngle = chart.PieChartSettings.FirstSliceAngle;
                excelChart.HoleSize = (int)(chart.PieChartSettings.HoleSize * 100.0);
            }
            return excelChart;
        }

        internal static ExcelErrorBars ToExcelErrorBars(this ErrorBars errorBars)
        {
            if (errorBars == null)
            {
                return null;
            }
            ExcelErrorBars bars = new ExcelErrorBars
            {
                ErrorBarType = (ExcelErrorBarType)errorBars.ErrorBarType,
                ErrorBarValueType = (ExcelErrorBarValueType)errorBars.ErrorBarValueType,
                ErrorBarDireciton = (ExcelErrorBarDireciton)errorBars.ErrorBarDireciton,
                NoEndCap = errorBars.NoEndCap
            };
            if (errorBars.Minus != null)
            {
                bars.Minus = errorBars.Minus.ToExcelNumericDataLiterals(errorBars.MinusFormatter);
            }
            else
            {
                bars.MinusReferenceFormula = errorBars.MinusReferenceFormula;
            }
            if (errorBars.Plus != null)
            {
                bars.Plus = errorBars.Plus.ToExcelNumericDataLiterals(errorBars.PlusFormatter);
            }
            else
            {
                bars.PlusReferenceFormula = errorBars.PlusReferenceFormula;
            }
            bars.Value = errorBars.Value;
            bars.ErrorBarsFormat = errorBars.GetExcelChartForamt();
            return bars;
        }

        internal static IExcelChartFormat ToExcelFillFormat(this SpreadDataSeries series)
        {
            if (series == null)
            {
                return null;
            }
            return series.GetExcelChartForamt();
        }

        internal static IFillFormat ToExcelFillFormat(this Brush brush)
        {
            if (brush is SolidColorBrush)
            {
                SolidColorBrush solidColorBrush = brush as SolidColorBrush;
                SolidFillFormat result = new SolidFillFormat();
                result.Color = new ExcelColor(GcColor.FromArgb(solidColorBrush.Color.A, solidColorBrush.Color.R, solidColorBrush.Color.G, solidColorBrush.Color.B));
                if (solidColorBrush.Color.A == 0)
                {
                    if (result.DrawingColorSettings == null)
                    {
                        result.DrawingColorSettings = new ExcelDrawingColorSettings();
                    }
                    result.DrawingColorSettings.Alpha = 0.0;
                }
                return result;
            }
            if (brush is GradientBrush)
            {
                GradientBrush gradientBrhs = brush as GradientBrush;
                GradientFillFormat result = new GradientFillFormat();
                foreach (GradientStop stop in gradientBrhs.GradientStops)
                {
                    ExcelGradientStop stop2 = new ExcelGradientStop
                    {
                        Color = new ExcelColor(GcColor.FromArgb(stop.Color.A, stop.Color.R, stop.Color.G, stop.Color.B)),
                        Position = stop.Offset
                    };
                    result.GradientStops.Add(stop2);
                }
                if (brush is LinearGradientBrush)
                {
                    result.GradientFillType = GradientFillType.Linear;
                    result.Angle = GetGradientFillAngle(brush as GradientBrush);
                }
                return result;
            }
            if (brush is ImageBrush)
            {
                return new BlipFillFormat();
            }
            return new NoFillFormat();
        }

        internal static Dt.Xls.Chart.Layout ToExcelLayout(this Dt.Cells.Data.Layout layout)
        {
            if (layout == null)
            {
                return null;
            }
            Dt.Xls.Chart.Layout layout2 = new Dt.Xls.Chart.Layout();
            if (layout.ManualLayout != null)
            {
                ExcelManualLayout layout3 = new ExcelManualLayout
                {
                    Target = (ExcelLayoutTarget)layout.ManualLayout.Target,
                    HeightMode = (ExcelLayoutMode)layout.ManualLayout.HeightMode,
                    WidthMode = (ExcelLayoutMode)layout.ManualLayout.WidthMode,
                    LeftMode = (ExcelLayoutMode)layout.ManualLayout.LeftMode,
                    TopMode = (ExcelLayoutMode)layout.ManualLayout.TopMode,
                    Height = layout.ManualLayout.Height,
                    Width = layout.ManualLayout.Width,
                    Left = layout.ManualLayout.Left,
                    Top = layout.ManualLayout.Top
                };
                layout2.ManualLayout = layout3;
            }
            return layout2;
        }

        internal static IExcelChartLegend ToExcelLegend(this Legend legend)
        {
            if (legend == null)
            {
                return null;
            }
            ExcelChartLegend result = new ExcelChartLegend
            {
                ShapeFormat = legend.GetExcelChartForamt(),
                Overlay = legend.OverlapChart
            };
            switch (legend.Alignment)
            {
                case LegendAlignment.TopCenter:
                    result.Position = ExcelLegendPositon.Top;
                    break;

                case LegendAlignment.TopRight:
                    result.Position = ExcelLegendPositon.TopRight;
                    break;

                case LegendAlignment.MiddleLeft:
                    result.Position = ExcelLegendPositon.Left;
                    break;

                case LegendAlignment.MiddleRight:
                    result.Position = ExcelLegendPositon.Right;
                    break;

                case LegendAlignment.BottomCenter:
                    result.Position = ExcelLegendPositon.Bottom;
                    break;

                default:
                    result.Position = ExcelLegendPositon.Right;
                    break;
            }
            result.TextFormat = legend.TextFormat;
            if (result.TextFormat == null)
            {
                if ((((legend.FontFamily != null) || (legend.Foreground != null)) || (!string.IsNullOrWhiteSpace(legend.ForegroundThemeColor) || !(legend.FontSize + 1.0).IsZero())) || (FontWeightsIsBold(legend.FontWeight) || (legend.FontStyle != FontStyle.Normal)))
                {
                    result.TextFormat = new ExcelTextFormat();
                    result.TextFormat.TextParagraphs.Add(new TextParagraph());
                    if (legend.FontFamily != null)
                    {
                        RichTextUtility.SetFontFamily(result.TextFormat.TextParagraphs, legend.FontFamily.GetFontName());
                    }
                    RichTextUtility.SetRichTextFill(result.TextFormat.TextParagraphs, legend.ActualForeground);
                    RichTextUtility.SetRichTextFontSize(result.TextFormat.TextParagraphs, legend.ActualFontSize);
                    if (legend.FontStyle == FontStyle.Italic)
                    {
                        RichTextUtility.SetRichTextFontStyle(result.TextFormat.TextParagraphs, true);
                    }
                    if (FontWeightsIsBold(legend.FontWeight))
                    {
                        RichTextUtility.SetRichtTextFontWeight(result.TextFormat.TextParagraphs, true);
                    }
                    else
                    {
                        RichTextUtility.SetRichtTextFontWeight(result.TextFormat.TextParagraphs, false);
                    }
                }
            }
            result.Layout = legend.Layout.ToExcelLayout();
            result.LegendEntries = legend.LegendEntries;
            return result;
        }

        internal static IExcelLineChart ToExcelLineChart(this SpreadChart chart)
        {
            if (chart != null)
            {
                List<SpreadDataSeries> list = new List<SpreadDataSeries>();
                ExcelLineChart excelChart = null;
                foreach (SpreadDataSeries series in chart.DataSeries)
                {
                    if (IsLineChart(series.ChartType))
                    {
                        list.Add(series);
                    }
                }
                if (list.Count > 0)
                {
                    excelChart = new ExcelLineChart(GetExcelChartType(list[0].ChartType));
                }
                else if (IsLineChart(chart.ChartType) && (chart.DataSeries.Count == 0))
                {
                    excelChart = new ExcelLineChart(GetExcelChartType(chart.ChartType));
                }
                if (excelChart != null)
                {
                    SetExcelLineChartSettings(chart, excelChart, (IEnumerable<SpreadDataSeries>)list);
                    excelChart.DropLine = chart.DropLine;
                    excelChart.HighLowLine = chart.HighLowLine;
                    excelChart.UpDownBars = chart.UpDownDarsSettings.ToExcelUpDownBars();
                    return excelChart;
                }
            }
            return null;
        }

        internal static LineEndStyle ToExcelLineEndStyle(this ArrowSettings arrowSettings)
        {
            if (arrowSettings == null)
            {
                return null;
            }
            return new LineEndStyle { Length = (LineSize)arrowSettings.Length, Width = (LineSize)arrowSettings.Width, Type = (LineEndType)arrowSettings.Type };
        }

        internal static NumericDataLiterals ToExcelNumericDataLiterals(this DoubleSeriesCollection seriesCollection, IFormatter formatter)
        {
            if ((seriesCollection == null) || (seriesCollection.Count <= 0))
            {
                return null;
            }
            NumericDataLiterals literals = new NumericDataLiterals();
            if (formatter != null)
            {
                literals.FormatCode = formatter.FormatString;
            }
            else
            {
                literals.FormatCode = "General";
            }
            for (int i = 0; i < seriesCollection.Count; i++)
            {
                double num2 = seriesCollection[i];
                ExcelNumberPoint point = new ExcelNumberPoint
                {
                    Index = i,
                    Value = ((double)num2).ToString((IFormatProvider)CultureInfo.InvariantCulture)
                };
                literals.NumberPoints.Add(point);
            }
            return literals;
        }

        internal static IExcelPieChart ToExcelPieChart(this SpreadChart chart)
        {
            if (chart == null)
            {
                return null;
            }
            List<SpreadDataSeries> list = new List<SpreadDataSeries>();
            ExcelPieChart excelChart = null;
            foreach (SpreadDataSeries series in chart.DataSeries)
            {
                if (IsPieChart(series.ChartType))
                {
                    list.Add(series);
                }
            }
            if (list.Count > 0)
            {
                excelChart = new ExcelPieChart(GetExcelChartType(list[0].ChartType));
            }
            else if (IsPieChart(chart.ChartType))
            {
                excelChart = new ExcelPieChart(GetExcelChartType(chart.ChartType));
            }
            if (excelChart == null)
            {
                return null;
            }
            SetExcelPieChartSettings(chart, excelChart, (IEnumerable<SpreadDataSeries>)list);
            if (chart.PieChartSettings != null)
            {
                excelChart.FirstSliceAngle = chart.PieChartSettings.FirstSliceAngle;
            }
            return excelChart;
        }

        internal static IExcelRadarChart ToExcelRadarChart(this SpreadChart chart)
        {
            if (chart != null)
            {
                List<SpreadDataSeries> list = new List<SpreadDataSeries>();
                ExcelRadarChart excelChart = null;
                foreach (SpreadDataSeries series in chart.DataSeries)
                {
                    if (IsRadarChart(series.ChartType))
                    {
                        list.Add(series);
                    }
                }
                if (list.Count > 0)
                {
                    excelChart = new ExcelRadarChart(GetExcelChartType(list[0].ChartType));
                }
                else if (IsRadarChart(chart.ChartType))
                {
                    excelChart = new ExcelRadarChart(GetExcelChartType(chart.ChartType));
                }
                if (excelChart != null)
                {
                    SetExcelRadarChartSettings(chart, excelChart, (IEnumerable<SpreadDataSeries>)list);
                    return excelChart;
                }
            }
            return null;
        }

        internal static IExcelScatterChart ToExcelScatterChart(this SpreadChart chart)
        {
            if (chart != null)
            {
                List<SpreadXYDataSeries> list = new List<SpreadXYDataSeries>();
                ExcelScatterChart excelChart = null;
                foreach (SpreadDataSeries series in chart.DataSeries)
                {
                    if (IsScatterChart(series.ChartType) && (series is SpreadXYDataSeries))
                    {
                        list.Add(series as SpreadXYDataSeries);
                    }
                }
                if (list.Count > 0)
                {
                    excelChart = new ExcelScatterChart(GetExcelChartType(list[0].ChartType));
                }
                else if (IsScatterChart(chart.ChartType))
                {
                    excelChart = new ExcelScatterChart(GetExcelChartType(chart.ChartType));
                }
                if (excelChart != null)
                {
                    SetExcelScatterChartSettings(chart, excelChart, (IEnumerable<SpreadXYDataSeries>)list);
                    return excelChart;
                }
            }
            return null;
        }

        internal static IExcelSeriesName ToExcelSeriesName(this SpreadDataSeries spreadDataSeries)
        {
            if (spreadDataSeries == null)
            {
                return null;
            }
            ExcelSeriesName name = new ExcelSeriesName();
            if (spreadDataSeries.IsAutoName)
            {
                return null;
            }
            if (!string.IsNullOrWhiteSpace(spreadDataSeries.NameFormula))
            {
                name.ReferenceFormula = spreadDataSeries.NameFormula;
                return name;
            }
            if (!string.IsNullOrWhiteSpace(spreadDataSeries.Name))
            {
                name.TextValue = spreadDataSeries.Name;
            }
            return name;
        }

        internal static IExcelSeriesValue ToExcelSeriesValue(this SpreadDataSeries spreadDataSeries)
        {
            if (spreadDataSeries == null)
            {
                return null;
            }
            ExcelSeriesValue value2 = new ExcelSeriesValue();
            if (!string.IsNullOrWhiteSpace(spreadDataSeries.ValueFormula))
            {
                value2.ReferenceFormula = spreadDataSeries.ValueFormula;
                if ((spreadDataSeries.Formatter != null) && (spreadDataSeries.Formatter.FormatString != "General"))
                {
                    value2.FormatCode = spreadDataSeries.Formatter.FormatString;
                }
                return value2;
            }
            value2.NumericLiterals = new NumericDataLiterals();
            if ((spreadDataSeries.Formatter != null) && (spreadDataSeries.Formatter.FormatString != "General"))
            {
                value2.FormatCode = spreadDataSeries.Formatter.FormatString;
            }
            for (int i = 0; i < spreadDataSeries.Values.Count; i++)
            {
                double num2 = spreadDataSeries.Values[i];
                ExcelNumberPoint point = new ExcelNumberPoint
                {
                    Index = i,
                    Value = ((double)num2).ToString((IFormatProvider)CultureInfo.InvariantCulture)
                };
                value2.NumericLiterals.NumberPoints.Add(point);
            }
            return value2;
        }

        internal static IExcelChartTitle ToExcelSpreadChartTitle(this ChartTitle chartTitle)
        {
            if (chartTitle == null)
            {
                return null;
            }
            ExcelChartTitle title = new ExcelChartTitle();
            if (chartTitle.RichText != null)
            {
                title.RichTextTitle = chartTitle.RichText;
            }
            else if (!string.IsNullOrWhiteSpace(chartTitle.Text) && (chartTitle.Text != ResourceStrings.DefaultChartTitle))
            {
                if (!string.IsNullOrEmpty(chartTitle.TextFormula))
                {
                    title.TitleFormula = chartTitle.TextFormula;
                }
                else
                {
                    RichText text = new RichText();
                    TextParagraph paragraph = new TextParagraph();
                    if (chartTitle.FontSize > 0.0)
                    {
                        paragraph.FontSize = new double?(UnitHelper.PixelToPoint(chartTitle.FontSize));
                    }
                    paragraph.Bold = new bool?(FontWeightsIsBold(chartTitle.FontWeight));
                    paragraph.Italics = new bool?(chartTitle.FontStyle == FontStyle.Italic);
                    if (!string.IsNullOrWhiteSpace(chartTitle.ForegroundThemeColor))
                    {
                        SolidFillFormat format = new SolidFillFormat
                        {
                            Color = chartTitle.ForegroundThemeColor.GetExcelChartThemeColor()
                        };
                        paragraph.FillFormat = format;
                    }
                    else if (chartTitle.Foreground != null)
                    {
                        paragraph.FillFormat = chartTitle.Foreground.ToExcelFillFormat();
                    }
                    TextRun run = new TextRun
                    {
                        Text = chartTitle.Text
                    };
                    paragraph.TextRuns.Add(run);
                    text.TextParagraphs.Add(paragraph);
                    title.RichTextTitle = text;
                }
            }
            title.Layout = chartTitle.Layout.ToExcelLayout();
            title.ShapeFormat = chartTitle.GetExcelChartForamt();
            title.TextForamt = chartTitle.TextForamt;
            title.Overlay = chartTitle.Overlay;
            return title;
        }

        internal static IExcelStockChart ToExcelStokeChart(this SpreadChart chart)
        {
            if (chart == null)
            {
                return null;
            }
            if (chart.ChartType != SpreadChartType.StockHighLowOpenClose)
            {
                return null;
            }
            IExcelStockChart excelChart = null;
            if ((chart.DataSeries != null) && (chart.DataSeries.Count > 0))
            {
                SpreadDataSeries series = chart.DataSeries[0];
                if (series is SpreadOpenHighLowCloseSeries)
                {
                    SpreadOpenHighLowCloseSeries openHighLowCloseSeries = series as SpreadOpenHighLowCloseSeries;
                    ExcelChartType stockHighLowClose = ExcelChartType.StockHighLowClose;
                    if (!openHighLowCloseSeries.IsOpenSeriesAutomatic)
                    {
                        stockHighLowClose = ExcelChartType.StockOpenHighLowClose;
                    }
                    excelChart = new ExcelStockChart(stockHighLowClose);
                    SetExcelStokeChartSettings(chart, excelChart, openHighLowCloseSeries, DataSeriesCounter.Index);
                }
                excelChart.DropLine = chart.DropLine;
                excelChart.HighLowLine = chart.HighLowLine;
                if (excelChart.HighLowLine == null)
                {
                    excelChart.HighLowLine = new ExcelChartLines();
                }
                excelChart.UpDownBars = chart.UpDownDarsSettings.ToExcelUpDownBars();
            }
            return excelChart;
        }

        internal static IExcelTrendLine ToExcelTrendLine(this TrendLine trendLine)
        {
            if (trendLine == null)
            {
                return null;
            }
            ExcelTrendLine line = new ExcelTrendLine
            {
                Name = trendLine.Name,
                Backward = trendLine.Backward,
                Forward = trendLine.Forward,
                DisplayEquation = trendLine.DisplayEquation,
                DisplayRSquaredValue = trendLine.DisplayRSquaredValue,
                Period = trendLine.Period,
                TrendlineType = (ExcelTrendLineType)trendLine.TrendlineType,
                Intercept = trendLine.Intercept
            };
            if (trendLine.TrendLineLabel != null)
            {
                ExcelTrendLineLabel label = new ExcelTrendLineLabel
                {
                    Layout = trendLine.TrendLineLabel.Layout.ToExcelLayout()
                };
                if (trendLine.TrendLineLabel.Formatter != null)
                {
                    IFormatter formatter = trendLine.TrendLineLabel.Formatter;
                    label.NumberFormat = formatter.FormatString;
                }
                if (!string.IsNullOrWhiteSpace(trendLine.TrendLineLabel.Text))
                {
                    label.TextStringReference = trendLine.TrendLineLabel.Text;
                }
                else if (trendLine.TrendLineLabel.RichText != null)
                {
                    label.RichText = trendLine.TrendLineLabel.RichText;
                }
                line.TrendLineLabel = label;
            }
            line.Order = trendLine.Order;
            if (trendLine._styleInfo != null)
            {
                line.Foramt = trendLine.StyleInfo.ToExcelChartFormat();
            }
            return line;
        }

        internal static ExcelUpDownBars ToExcelUpDownBars(this UpDownBars upDownBars)
        {
            if (upDownBars == null)
            {
                return null;
            }
            return new ExcelUpDownBars { GapWidth = upDownBars.GapWidth, UpBars = upDownBars.UpBarsStyle.ToExcelChartFormat(), DownBars = upDownBars.DownBarsStyle.ToExcelChartFormat() };
        }

        internal static ViewIn3D ToExcelViewIn3D(this View3DSettings view3DSetting)
        {
            if (view3DSetting == null)
            {
                return null;
            }
            return new ViewIn3D { RotationX = view3DSetting.RotationX, RotationY = view3DSetting.RotationY, DepthPercent = view3DSetting.DepthPercent, HeightPercent = view3DSetting.HeightPercent, Perspective = view3DSetting.Perspective, RightAngleAxes = view3DSetting.RightAngleAxes };
        }

        internal static IExcelWall ToExcelWall(this Wall wall)
        {
            if (wall == null)
            {
                return null;
            }
            return new ExcelWall { Format = wall.GetExcelChartForamt(), Thickness = wall.Thickness };
        }

        internal static Axis ToSpreadAxis(this IExcelChartAxis excelAxis, Axis axis, Workbook workbook)
        {
            if (excelAxis == null)
            {
                return null;
            }
            if (axis == null)
            {
                axis = new Axis();
            }
            SetAxis(excelAxis, workbook, axis);
            return axis;
        }

        internal static SpreadBubbleSeries ToSpreadBubbleSeries(this IExcelChartSeriesBase excelSeries, Workbook workbook)
        {
            if (excelSeries == null)
            {
                return null;
            }
            SpreadBubbleSeries xyDataSeries = new SpreadBubbleSeries();
            SetSpreadXYDataSeries(xyDataSeries, excelSeries, workbook);
            ExcelBubbleSeries series2 = excelSeries as ExcelBubbleSeries;
            if (series2.BubbleSize != null)
            {
                IExcelSeriesValue bubbleSize = series2.BubbleSize;
                if (!string.IsNullOrWhiteSpace(bubbleSize.ReferenceFormula))
                {
                    xyDataSeries.SizeFormula = bubbleSize.ReferenceFormula;
                    if (!string.IsNullOrWhiteSpace(bubbleSize.FormatCode) && (bubbleSize.FormatCode != "General"))
                    {
                        xyDataSeries.SizeFormatter = new GeneralFormatter(bubbleSize.FormatCode);
                    }
                    return xyDataSeries;
                }
                if ((bubbleSize.NumericLiterals == null) || (bubbleSize.NumericLiterals.NumberPoints == null))
                {
                    return xyDataSeries;
                }
                int num = bubbleSize.NumericLiterals.NumberPoints.Count;
                for (int i = 0; i < num; i++)
                {
                    xyDataSeries.SizeValues.Add(0.0);
                }
                foreach (ExcelNumberPoint point in bubbleSize.NumericLiterals.NumberPoints)
                {
                    string formatCode = point.FormatCode;
                    if (string.IsNullOrWhiteSpace(formatCode))
                    {
                        formatCode = bubbleSize.NumericLiterals.FormatCode;
                    }
                    if (string.IsNullOrWhiteSpace(formatCode))
                    {
                        formatCode = bubbleSize.FormatCode;
                    }
                    if (string.IsNullOrWhiteSpace(formatCode))
                    {
                        formatCode = "General";
                    }
                    double num3 = 0.0;
                    if (double.TryParse(point.Value, (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out num3))
                    {
                        xyDataSeries.SizeValues[point.Index] = num3;
                    }
                    else
                    {
                        xyDataSeries.SizeValues[point.Index] = 0.0;
                    }
                }
            }
            return xyDataSeries;
        }

        internal static Legend ToSpreadChartLegend(this IExcelChartLegend legend, Workbook workbook)
        {
            if (legend == null)
            {
                return null;
            }
            Legend result = new Legend();
            SetSpreadChartElementStyle(result, legend.ShapeFormat, workbook);
            result.OverlapChart = legend.Overlay;
            result.Layout = legend.Layout.ToSpreadLayout();
            result.TextFormat = legend.TextFormat;
            if (legend.TextFormat != null)
            {
                result.Foreground = RichTextUtility.GetRichTextFill(legend.TextFormat.TextParagraphs, workbook);
                double? richTextFontSize = RichTextUtility.GetRichTextFontSize(legend.TextFormat.TextParagraphs);
                if (richTextFontSize.HasValue && richTextFontSize.HasValue)
                {
                    result.FontSize = richTextFontSize.Value;
                }
                FontFamily richTextFamily = RichTextUtility.GetRichTextFamily(legend.TextFormat.TextParagraphs);
                if (richTextFamily != null)
                {
                    result.FontFamily = richTextFamily;
                }
                result.FontStyle = RichTextUtility.GetRichTextFontStyle(legend.TextFormat.TextParagraphs);
                result.FontWeight = RichTextUtility.GetRichTextFontWeight(legend.TextFormat.TextParagraphs);
            }
            result.LegendEntries = legend.LegendEntries;
            switch (legend.Position)
            {
                case ExcelLegendPositon.Left:
                    result.Alignment = LegendAlignment.MiddleLeft;
                    result.Orientation = Orientation.Vertical;
                    break;

                case ExcelLegendPositon.Right:
                    result.Alignment = LegendAlignment.MiddleRight;
                    result.Orientation = Orientation.Vertical;
                    break;

                case ExcelLegendPositon.Top:
                    result.Alignment = LegendAlignment.TopCenter;
                    result.Orientation = Orientation.Horizontal;
                    break;

                case ExcelLegendPositon.Bottom:
                    result.Alignment = LegendAlignment.BottomCenter;
                    result.Orientation = Orientation.Horizontal;
                    break;

                case ExcelLegendPositon.TopRight:
                    result.Alignment = LegendAlignment.TopRight;
                    result.Orientation = Orientation.Horizontal;
                    break;
            }
            return result;
        }

        internal static ChartSymbolStyleInfo ToSpreadChartSymbolStyleInfo(this IExcelChartFormat format, Workbook workbook, SpreadDataSeries dataSeries)
        {
            if ((format == null) || (workbook == null))
            {
                return null;
            }
            ChartSymbolStyleInfo info = new ChartSymbolStyleInfo(dataSeries);
            object fillBrush = GetFillBrush(format, workbook);
            if (fillBrush is string)
            {
                info.FillThemeColor = (string)(fillBrush as string);
            }
            else if (fillBrush is Brush)
            {
                info.Fill = fillBrush as Brush;
            }
            else if (IsNoFill(format))
            {
                info.IsAutomaticFill = false;
            }
            else if (format.FillFormat == null)
            {
                info.IsAutomaticFill = true;
            }
            info.FillDrawingColorSettings = GetSpreadDrawingColorSettings(format.FillFormat);
            object stokeBrush = GetStokeBrush(format, workbook);
            if (stokeBrush is string)
            {
                info.StrokeThemeColor = (string)(stokeBrush as string);
            }
            else if (stokeBrush is Brush)
            {
                info.Stroke = stokeBrush as Brush;
            }
            else if (IsLineNoFill(format))
            {
                info.IsAutomaticStroke = false;
            }
            else if ((format.LineFormat != null) && (format.LineFormat.FillFormat == null))
            {
                info.IsAutomaticStroke = true;
            }
            if (format.LineFormat != null)
            {
                info.StrokeDrawingColorSettings = GetSpreadDrawingColorSettings(format.LineFormat.FillFormat);
                info.StrokeDashType = GetStrokeDashType(format.LineFormat.LineDashType);
            }
            double width = 0.0;
            if ((format != null) && (format.LineFormat != null))
            {
                width = format.LineFormat.Width;
            }
            info.StrokeThickness = width;
            return info;
        }

        internal static ChartTitle ToSpreadChartTitle(this IExcelChartTitle excelChartTitle, Workbook workbook)
        {
            if (excelChartTitle == null)
            {
                return null;
            }
            ChartTitle chartElement = new ChartTitle();
            if (!string.IsNullOrWhiteSpace(excelChartTitle.TitleFormula))
            {
                chartElement.TextFormula = "=" + excelChartTitle.TitleFormula;
            }
            else if (excelChartTitle.RichTextTitle != null)
            {
                chartElement.RichText = excelChartTitle.RichTextTitle;
            }
            SetSpreadChartElementStyle(chartElement, excelChartTitle.ShapeFormat, workbook);
            chartElement.Layout = excelChartTitle.Layout.ToSpreadLayout();
            chartElement.TextForamt = excelChartTitle.TextForamt;
            chartElement.Overlay = excelChartTitle.Overlay;
            return chartElement;
        }

        internal static SpreadChartType ToSpreadChartType(this ExcelChartType chartType)
        {
            switch (chartType)
            {
                case ExcelChartType.ColumnClustered:
                    return SpreadChartType.ColumnClustered;

                case ExcelChartType.ColumnClustered3D:
                case ExcelChartType.ColumnStacked3D:
                case ExcelChartType.ColumnStacked100Percent3D:
                case ExcelChartType.Column3D:
                case ExcelChartType.BarClustered3D:
                case ExcelChartType.BarStacked3D:
                case ExcelChartType.BarStacked100Percent3D:
                case ExcelChartType.Line3D:
                case ExcelChartType.Pie3D:
                case ExcelChartType.ExplodedPie3D:
                case ExcelChartType.Area3D:
                case ExcelChartType.AreaStacked3D:
                case ExcelChartType.AreaStacked100Percent3D:
                case ExcelChartType.CylinderColumnClustered:
                case ExcelChartType.CylinderColumnStacked:
                case ExcelChartType.CylinderColumnStacked100Percent:
                case ExcelChartType.CylinderBarStacked:
                case ExcelChartType.CylinderBarStacked100Percent:
                case ExcelChartType.CylinderColumn3D:
                case ExcelChartType.ConeColumnClustered:
                case ExcelChartType.ConeColumnStacked:
                case ExcelChartType.ConeColumnStacked100Percent:
                case ExcelChartType.ConeBarClustered:
                case ExcelChartType.ConeBarStacked:
                case ExcelChartType.ConeBarStacked100Percent:
                case ExcelChartType.ConeColumn3D:
                case ExcelChartType.PyramidColumnClustered:
                case ExcelChartType.PyramidColumnStacked:
                case ExcelChartType.PyramidColumnStacked100Percent:
                case ExcelChartType.PyramidBarClustered:
                case ExcelChartType.PyramidBarStacked:
                case ExcelChartType.PyramidBarStacked100Percent:
                case ExcelChartType.PyramidColumn3D:
                    return SpreadChartType.None;

                case ExcelChartType.ColumnStacked:
                    return SpreadChartType.ColumnStacked;

                case ExcelChartType.ColumnStacked100Percent:
                    return SpreadChartType.ColumnStacked100pc;

                case ExcelChartType.BarClustered:
                    return SpreadChartType.BarClustered;

                case ExcelChartType.BarStacked:
                    return SpreadChartType.BarStacked;

                case ExcelChartType.BarStacked100Percent:
                    return SpreadChartType.BarStacked100pc;

                case ExcelChartType.Line:
                    return SpreadChartType.Line;

                case ExcelChartType.LineWithMarkers:
                    return SpreadChartType.LineWithMarkers;

                case ExcelChartType.LineStacked:
                    return SpreadChartType.LineStacked;

                case ExcelChartType.LineStackedWithMarkers:
                    return SpreadChartType.LineStackedWithMarkers;

                case ExcelChartType.LineStacked100Percent:
                    return SpreadChartType.LineStacked100pc;

                case ExcelChartType.LineStacked100PercentWithMarkers:
                    return SpreadChartType.LineStacked100pcWithMarkers;

                case ExcelChartType.Pie:
                    return SpreadChartType.Pie;

                case ExcelChartType.ExplodedPie:
                    return SpreadChartType.PieExploded;

                case ExcelChartType.PieOfPie:
                case ExcelChartType.BarOfPie:
                    return SpreadChartType.None;

                case ExcelChartType.Scatter:
                    return SpreadChartType.Scatter;

                case ExcelChartType.ScatterLines:
                    return SpreadChartType.ScatterLines;

                case ExcelChartType.ScatterLinesWithMarkers:
                    return SpreadChartType.ScatterLinesWithMarkers;

                case ExcelChartType.ScatterLinesSmooth:
                    return SpreadChartType.ScatterLinesSmoothed;

                case ExcelChartType.ScatterLinesSmoothWithMarkers:
                    return SpreadChartType.ScatterLinesSmoothedWithMarkers;

                case ExcelChartType.Area:
                    return SpreadChartType.Area;

                case ExcelChartType.AreaStacked:
                    return SpreadChartType.AreaStacked;

                case ExcelChartType.AreaStacked100Percent:
                    return SpreadChartType.AreaStacked100pc;

                case ExcelChartType.Doughunt:
                    return SpreadChartType.PieDoughnut;

                case ExcelChartType.DoughuntExploded:
                    return SpreadChartType.PieExplodedDoughnut;

                case ExcelChartType.Radar:
                    return SpreadChartType.Radar;

                case ExcelChartType.RadarWithMarkers:
                    return SpreadChartType.RadarWithMarkers;

                case ExcelChartType.FilledRadar:
                    return SpreadChartType.RadarFilled;

                case ExcelChartType.Bubble:
                    return SpreadChartType.Bubble;

                case ExcelChartType.Bubble3D:
                    return SpreadChartType.Bubble;

                case ExcelChartType.StockHighLowClose:
                case ExcelChartType.StockOpenHighLowClose:
                    return SpreadChartType.StockHighLowOpenClose;
            }
            return SpreadChartType.None;
        }

        internal static View3DSettings ToSpreadChartView3D(this ViewIn3D viewIn3D)
        {
            if (viewIn3D == null)
            {
                return null;
            }
            return new View3DSettings { RotationX = viewIn3D.RotationX, RotationY = viewIn3D.RotationY, DepthPercent = viewIn3D.DepthPercent, HeightPercent = viewIn3D.HeightPercent, Perspective = viewIn3D.Perspective, RightAngleAxes = viewIn3D.RightAngleAxes };
        }

        internal static DataLabelSettings ToSpreadDataLabelSettings(this IExcelDataLabels dataLabels, Workbook workbook)
        {
            if (dataLabels == null)
            {
                return new DataLabelSettings();
            }
            DataLabelSettings settings2 = new DataLabelSettings();
            if (dataLabels.ShowCategoryName)
            {
                settings2.ShowCategoryName = dataLabels.ShowCategoryName;
            }
            if (dataLabels.ShowValue)
            {
                settings2.ShowValue = dataLabels.ShowValue;
            }
            if (dataLabels.ShowSeriesName)
            {
                settings2.ShowSeriesName = dataLabels.ShowSeriesName;
            }
            if (dataLabels.ShowLegendKey)
            {
                settings2.IncludeLegendKey = dataLabels.ShowLegendKey;
            }
            if (dataLabels.ShowLeaderLines)
            {
                settings2.IncludeLeaderLines = dataLabels.ShowLeaderLines;
            }
            if (dataLabels.Separator != settings2.Separator)
            {
                settings2.Separator = dataLabels.Separator;
            }
            if (dataLabels.ShowPercentage)
            {
                settings2.ShowPercent = dataLabels.ShowPercentage;
            }
            if (dataLabels.ShowBubbleSize)
            {
                settings2.ShowBubbleSize = dataLabels.ShowBubbleSize;
            }
            settings2.Position = (Dt.Cells.Data.DataLabelPosition)dataLabels.Position;
            settings2.DataLabelList = dataLabels.DataLabelList;
            settings2.TextFormat = dataLabels.TextFormat;
            settings2.NumberFormatLinked = dataLabels.NumberFormatLinked;
            return settings2;
        }

        internal static DataMarker ToSpreadDataMarker(this ExcelDataMarker dataMarker, Workbook workbook)
        {
            if (dataMarker == null)
            {
                return null;
            }
            DataMarker marker = new DataMarker();
            switch (dataMarker.MarkerStyle)
            {
                case DataPointMarkStyle.None:
                    marker.ExcelMarkerType = Dt.Cells.Data.MarkerType.None;
                    break;

                case DataPointMarkStyle.Circle:
                    marker.ExcelMarkerType = Dt.Cells.Data.MarkerType.Circle;
                    break;

                case DataPointMarkStyle.Dash:
                    marker.ExcelMarkerType = Dt.Cells.Data.MarkerType.Dot;
                    break;

                case DataPointMarkStyle.Diamond:
                    marker.ExcelMarkerType = Dt.Cells.Data.MarkerType.Diamond;
                    break;

                case DataPointMarkStyle.Dot:
                    marker.ExcelMarkerType = Dt.Cells.Data.MarkerType.Dot;
                    break;

                case DataPointMarkStyle.Plus:
                    marker.ExcelMarkerType = Dt.Cells.Data.MarkerType.Cross;
                    break;

                case DataPointMarkStyle.Square:
                    marker.ExcelMarkerType = Dt.Cells.Data.MarkerType.Box;
                    break;

                case DataPointMarkStyle.Star:
                    marker.ExcelMarkerType = Dt.Cells.Data.MarkerType.Star4;
                    break;

                case DataPointMarkStyle.Triangle:
                    marker.ExcelMarkerType = Dt.Cells.Data.MarkerType.Triangle;
                    break;

                case DataPointMarkStyle.X:
                    marker.ExcelMarkerType = Dt.Cells.Data.MarkerType.DiagonalCross;
                    break;
            }
            if (dataMarker.MarkerStyle == DataPointMarkStyle.None)
            {
                marker.MarkerSize = new Windows.Foundation.Size(0.0, 0.0);
            }
            else
            {
                marker.MarkerSize = new Windows.Foundation.Size((double)dataMarker.MarkerSize, (double)dataMarker.MarkerSize);
            }
            marker.StyleInfo = dataMarker.Format.ToSpreadFloatingObjectStyleInfo(workbook);
            return marker;
        }

        internal static SpreadDataSeries ToSpreadDataSeries(this IExcelChartSeriesBase excelSeries, Workbook workbook)
        {
            if (excelSeries == null)
            {
                return null;
            }
            SpreadDataSeries spreadDataSeries = new SpreadDataSeries();
            SetDataSeries(spreadDataSeries, excelSeries, workbook);
            return spreadDataSeries;
        }

        internal static SpreadDataSeries ToSpreadDataSeries(this IExcelLineSeries excelSeries, Workbook workbook)
        {
            SpreadDataSeries spreadDataSeries = ((IExcelChartSeriesBase)excelSeries).ToSpreadDataSeries(workbook);
            if (spreadDataSeries != null)
            {
                spreadDataSeries.SetDataMarker(excelSeries.Marker, workbook);
                spreadDataSeries.ChartType = SpreadChartType.Line;
            }
            return spreadDataSeries;
        }

        internal static DataTableSettings ToSpreadDataTableSettings(this IExcelChartDataTable dataTable)
        {
            if (dataTable == null)
            {
                return null;
            }
            return new DataTableSettings { ShowHorizontalBorder = dataTable.ShowHorizontalBorder, ShowVerticalBorder = dataTable.ShowVerticalBorder, ShowOutlineBorder = dataTable.ShowOutlineBorder, ShowLegendKeys = dataTable.ShowLegendKeys };
        }

        internal static DoubleSeriesCollection ToSpreadDoubleColleciton(this NumericDataLiterals NumericLiterals)
        {
            if (NumericLiterals == null)
            {
                return null;
            }
            DoubleSeriesCollection seriess = new DoubleSeriesCollection();
            for (int i = 0; i < NumericLiterals.NumberPoints.Count; i++)
            {
                seriess.Add(0.0);
            }
            foreach (ExcelNumberPoint point in NumericLiterals.NumberPoints)
            {
                double num2 = 0.0;
                double.TryParse(point.Value, (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out num2);
                seriess[point.Index] = num2;
            }
            return seriess;
        }

        internal static SpreadDrawingColorSettings ToSpreadDrawingColorSettings(ExcelDrawingColorSettings excelColorSettings)
        {
            if (excelColorSettings == null)
            {
                return null;
            }
            return new SpreadDrawingColorSettings { alpha = excelColorSettings.Alpha, shade = excelColorSettings.Shade, tint = excelColorSettings.Tint, hue = excelColorSettings.Hue, hueOff = excelColorSettings.HueOff, hueMod = excelColorSettings.HueMod, sat = excelColorSettings.Sat, satOff = excelColorSettings.SatOff, satMod = excelColorSettings.SatMod, lum = excelColorSettings.Lum, lumOff = excelColorSettings.LumOff, lumMod = excelColorSettings.LumMod };
        }

        internal static ErrorBars ToSpreadErrorBars(this IExcelErrorBars errorBars, Workbook workbook)
        {
            if (errorBars == null)
            {
                return null;
            }
            ErrorBars bars = new ErrorBars
            {
                ErrorBarType = (ErrorBarType)errorBars.ErrorBarType,
                ErrorBarValueType = (ErrorBarValueType)errorBars.ErrorBarValueType,
                ErrorBarDireciton = (ErrorBarDireciton)errorBars.ErrorBarDireciton,
                NoEndCap = errorBars.NoEndCap
            };
            if (errorBars.Minus != null)
            {
                bars.Minus = errorBars.Minus.ToSpreadDoubleColleciton();
            }
            else
            {
                bars.MinusReferenceFormula = errorBars.MinusReferenceFormula;
            }
            if (errorBars.Plus != null)
            {
                bars.Plus = errorBars.Plus.ToSpreadDoubleColleciton();
            }
            else
            {
                bars.PlusReferenceFormula = errorBars.PlusReferenceFormula;
            }
            if ((errorBars.Minus != null) && !string.IsNullOrWhiteSpace(errorBars.Minus.FormatCode))
            {
                bars.MinusFormatter = new GeneralFormatter(errorBars.Minus.FormatCode);
            }
            else
            {
                bars.MinusFormatter = new GeneralFormatter("General");
            }
            if ((errorBars.Plus != null) && !string.IsNullOrWhiteSpace(errorBars.Plus.FormatCode))
            {
                bars.PlusFormatter = new GeneralFormatter(errorBars.Plus.FormatCode);
            }
            else
            {
                bars.PlusFormatter = new GeneralFormatter("General");
            }
            bars.Value = errorBars.Value;
            if (errorBars.ErrorBarsFormat != null)
            {
                object fill = GetFill(errorBars.ErrorBarsFormat.FillFormat, workbook);
                if (fill is string)
                {
                    bars.FillThemeColor = (string)(fill as string);
                }
                else if (fill is Brush)
                {
                    bars.Fill = fill as Brush;
                }
                else if (IsNoFill(errorBars.ErrorBarsFormat))
                {
                    bars.IsAutomaticFill = false;
                }
                else if (errorBars.ErrorBarsFormat.FillFormat == null)
                {
                    bars.IsAutomaticFill = true;
                }
                object stokeBrush = GetStokeBrush(errorBars.ErrorBarsFormat, workbook);
                if (stokeBrush is string)
                {
                    bars.StrokeThemeColor = (string)(stokeBrush as string);
                }
                else if (stokeBrush is Brush)
                {
                    bars.Stroke = stokeBrush as Brush;
                }
                else if (IsLineNoFill(errorBars.ErrorBarsFormat))
                {
                    bars.IsAutomaticStroke = false;
                }
                else if ((errorBars.ErrorBarsFormat.LineFormat != null) && (errorBars.ErrorBarsFormat.LineFormat.FillFormat == null))
                {
                    bars.IsAutomaticStroke = true;
                }
                if ((errorBars.ErrorBarsFormat != null) && (errorBars.ErrorBarsFormat.LineFormat != null))
                {
                    bars.StrokeThickness = errorBars.ErrorBarsFormat.LineFormat.Width;
                }
            }
            return bars;
        }

        internal static FloatingObjectStyleInfo ToSpreadFloatingObjectStyleInfo(this IExcelChartFormat format, Workbook workbook)
        {
            if ((format == null) || (workbook == null))
            {
                return null;
            }
            FloatingObjectStyleInfo info = new FloatingObjectStyleInfo();
            object fillBrush = GetFillBrush(format, workbook);
            if (fillBrush is string)
            {
                info.FillThemeColor = (string)(fillBrush as string);
            }
            else if (fillBrush is Brush)
            {
                info.Fill = fillBrush as Brush;
            }
            info.FillDrawingColorSettings = (ExcelDrawingColorSettings)GetSpreadDrawingColorSettings(format.FillFormat);
            object stokeBrush = GetStokeBrush(format, workbook);
            if (stokeBrush is string)
            {
                info.StrokeThemeColor = (string)(stokeBrush as string);
            }
            else if (stokeBrush is Brush)
            {
                info.Stroke = stokeBrush as Brush;
            }
            if (format.LineFormat != null)
            {
                info.StrokeDrawingColorSettings = (ExcelDrawingColorSettings)GetSpreadDrawingColorSettings(format.LineFormat.FillFormat);
                info.StrokeDashType = GetStrokeDashType(format.LineFormat.LineDashType);
            }
            double width = 0.0;
            if ((format != null) && (format.LineFormat != null))
            {
                width = format.LineFormat.Width;
            }
            info.StrokeThickness = width;
            return info;
        }

        internal static Dt.Cells.Data.Layout ToSpreadLayout(this Dt.Xls.Chart.Layout layout)
        {
            if (layout == null)
            {
                return null;
            }
            Dt.Cells.Data.Layout layout2 = new Dt.Cells.Data.Layout();
            if (layout.ManualLayout != null)
            {
                ManualLayout layout3 = new ManualLayout
                {
                    Target = (LayoutTarget)layout.ManualLayout.Target,
                    HeightMode = (LayoutMode)layout.ManualLayout.HeightMode,
                    WidthMode = (LayoutMode)layout.ManualLayout.WidthMode,
                    LeftMode = (LayoutMode)layout.ManualLayout.LeftMode,
                    TopMode = (LayoutMode)layout.ManualLayout.TopMode,
                    Height = layout.ManualLayout.Height,
                    Width = layout.ManualLayout.Width,
                    Left = layout.ManualLayout.Left,
                    Top = layout.ManualLayout.Top
                };
                layout2.ManualLayout = layout3;
            }
            return layout2;
        }

        internal static TrendLine ToSpreadTrendLine(this IExcelTrendLine excelTrendLine, Workbook workbook)
        {
            if (excelTrendLine == null)
            {
                return null;
            }
            TrendLine line = new TrendLine
            {
                Name = excelTrendLine.Name,
                Backward = excelTrendLine.Backward,
                Forward = excelTrendLine.Forward,
                DisplayEquation = excelTrendLine.DisplayEquation,
                DisplayRSquaredValue = excelTrendLine.DisplayRSquaredValue,
                Period = excelTrendLine.Period,
                TrendlineType = (TrendLineType)excelTrendLine.TrendlineType,
                Order = excelTrendLine.Order,
                Intercept = excelTrendLine.Intercept
            };
            if (excelTrendLine.TrendLineLabel != null)
            {
                TrendLineLabel label = new TrendLineLabel
                {
                    Layout = excelTrendLine.TrendLineLabel.Layout.ToSpreadLayout()
                };
                string numberFormat = excelTrendLine.TrendLineLabel.NumberFormat;
                if (!string.IsNullOrWhiteSpace(numberFormat))
                {
                    label.Formatter = new GeneralFormatter(numberFormat);
                }
                if (!string.IsNullOrWhiteSpace(excelTrendLine.TrendLineLabel.TextStringReference))
                {
                    label.Text = excelTrendLine.TrendLineLabel.TextStringReference;
                }
                else if (excelTrendLine.TrendLineLabel.RichText != null)
                {
                    label.RichText = excelTrendLine.TrendLineLabel.RichText;
                }
                line.TrendLineLabel = label;
            }
            if (excelTrendLine.Foramt != null)
            {
                FloatingObjectStyleInfo info = excelTrendLine.Foramt.ToSpreadFloatingObjectStyleInfo(workbook);
                line._styleInfo = info;
            }
            return line;
        }

        internal static UpDownBars ToSpreadUpDonwbars(this ExcelUpDownBars upDownBars, Workbook workbook)
        {
            if (upDownBars == null)
            {
                return null;
            }
            return new UpDownBars { GapWidth = upDownBars.GapWidth, UpBarsStyle = upDownBars.UpBars.ToSpreadChartSymbolStyleInfo(workbook, null), DownBarsStyle = upDownBars.DownBars.ToSpreadChartSymbolStyleInfo(workbook, null) };
        }

        internal static Wall ToSpreadWall(this IExcelWall excelWall, Workbook workbook)
        {
            if (excelWall == null)
            {
                return null;
            }
            Wall chartElement = new Wall
            {
                Thickness = excelWall.Thickness
            };
            SetSpreadChartElementStyle(chartElement, excelWall.Format, workbook);
            return chartElement;
        }

        internal static SpreadXYDataSeries ToSpreadXYDataSeries(this IExcelChartSeriesBase excelSeries, Workbook workbook)
        {
            if (excelSeries == null)
            {
                return null;
            }
            SpreadXYDataSeries xyDataSeries = new SpreadXYDataSeries();
            SetSpreadXYDataSeries(xyDataSeries, excelSeries, workbook);
            return xyDataSeries;
        }

        internal static double UpdateAngle(double angle, double offsetx, double offsety)
        {
            if (offsetx <= 0.0)
            {
                if ((angle == 0.0) && (offsetx < 0.0))
                {
                    return -3.1415926535897931;
                }
                if ((offsetx < 0.0) && (offsety < 0.0))
                {
                    return (-3.1415926535897931 - angle);
                }
                if ((offsetx < 0.0) && (offsety > 0.0))
                {
                    return (3.1415926535897931 - angle);
                }
            }
            return angle;
        }

        static Windows.Foundation.Point BottomLeft
        {
            get { return new Windows.Foundation.Point(0.0, 1.0); }
        }

        static Windows.Foundation.Point BottomRight
        {
            get { return new Windows.Foundation.Point(1.0, 1.0); }
        }

        static Windows.Foundation.Point MiddleCenter
        {
            get { return new Windows.Foundation.Point(0.5, 0.5); }
        }

        static Windows.Foundation.Point TopLeft
        {
            get { return new Windows.Foundation.Point(0.0, 0.0); }
        }

        static Windows.Foundation.Point TopRight
        {
            get { return new Windows.Foundation.Point(1.0, 0.0); }
        }
    }
}

