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
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal partial class ChartTitleView : Panel
    {
        Dt.Cells.Data.ChartTitle _chartTitle;
        SpreadChartBaseView _parentView;
        TextBlock _titleContent;
        Rectangle _titleFrame;

        public ChartTitleView(Dt.Cells.Data.ChartTitle chartTitle, SpreadChartBaseView parentView)
        {
            _chartTitle = chartTitle;
            _parentView = parentView;
            _titleFrame = new Rectangle();
            base.Children.Add(_titleFrame);
            _titleContent = new TextBlock();
            _titleContent.TextAlignment = Windows.UI.Xaml.TextAlignment.Center;
            base.Children.Add(_titleContent);
            UpdateChartTitle();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _titleFrame.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            _titleContent.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _titleFrame.Measure(availableSize);
            _titleContent.Measure(availableSize);
            return base.MeasureOverride(availableSize);
        }

        internal void UpdateChartTitle()
        {
            if (ChartTitle != null)
            {
                _titleFrame.StrokeDashArray =StrokeDashHelper.GetStrokeDashes(ChartTitle.StrokeDashType);
                double strokeThickness = ChartTitle.StrokeThickness;
                if (strokeThickness > 0.0)
                {
                    if ((_parentView != null) && (_parentView.ParentViewport != null))
                    {
                        strokeThickness *= _parentView.ParentViewport.Excel.ZoomFactor;
                    }
                    _titleFrame.StrokeThickness = strokeThickness;
                }
                else
                {
                    _titleFrame.StrokeThickness = 0.0;
                }
                Brush actualStroke = ChartTitle.ActualStroke;
                if (actualStroke != null)
                {
                    _titleFrame.Stroke = actualStroke;
                }
                else
                {
                    _titleFrame.Stroke = new SolidColorBrush(Colors.Transparent);
                }
                Brush actualFill = ChartTitle.ActualFill;
                if (actualFill != null)
                {
                    _titleFrame.Fill = actualFill;
                }
                else
                {
                    _titleFrame.Fill = new SolidColorBrush(Colors.Transparent);
                }
                _titleContent.Text = ChartTitle.Text;
                FontFamily actualFontFamily = ChartTitle.ActualFontFamily;
                if (actualFontFamily == null)
                {
                    actualFontFamily = Utility.DefaultFontFamily;
                }
                _titleContent.FontFamily = actualFontFamily;
                double actualFontSize = ChartTitle.ActualFontSize;
                if ((_parentView.ParentViewport != null) && (_parentView.ParentViewport.Excel != null))
                {
                    actualFontSize *= _parentView.ParentViewport.Excel.ZoomFactor;
                }
                if (actualFontSize > 0.0)
                {
                    _titleContent.FontSize = actualFontSize;
                }
                _titleContent.FontStretch = ChartTitle.ActualFontStretch;
                _titleContent.FontStyle = ChartTitle.ActualFontStyle;
                _titleContent.FontWeight = ChartTitle.ActualFontWeight;
                Brush actualForeground = ChartTitle.ActualForeground;
                if (actualForeground != null)
                {
                    _titleContent.Foreground = actualForeground;
                }
                else
                {
                    _titleContent.ClearValue(TextBlock.ForegroundProperty);
                }
            }
            else
            {
                _titleFrame.Stroke = null;
                _titleFrame.Fill = null;
                _titleContent.ClearValue(TextBlock.TextProperty);
            }
        }

        public Dt.Cells.Data.ChartTitle ChartTitle
        {
            get { return  _chartTitle; }
            set
            {
                _chartTitle = value;
                UpdateChartTitle();
            }
        }
    }
}

