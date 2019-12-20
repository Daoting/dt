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
        private Dt.Cells.Data.ChartTitle _chartTitle;
        private SpreadChartBaseView _parentView;
        private TextBlock _titleContent;
        private Rectangle _titleFrame;

        public ChartTitleView(Dt.Cells.Data.ChartTitle chartTitle, SpreadChartBaseView parentView)
        {
            this._chartTitle = chartTitle;
            this._parentView = parentView;
            this._titleFrame = new Rectangle();
            base.Children.Add(this._titleFrame);
            this._titleContent = new TextBlock();
            this._titleContent.TextAlignment = Windows.UI.Xaml.TextAlignment.Center;
            base.Children.Add(this._titleContent);
            this.UpdateChartTitle();
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            this._titleFrame.Arrange(new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            this._titleContent.Arrange(new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            return base.ArrangeOverride(finalSize);
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            this._titleFrame.Measure(availableSize);
            this._titleContent.Measure(availableSize);
            return base.MeasureOverride(availableSize);
        }

        internal void UpdateChartTitle()
        {
            if (this.ChartTitle != null)
            {
                this._titleFrame.StrokeDashArray =StrokeDashHelper.GetStrokeDashes(this.ChartTitle.StrokeDashType);
                double strokeThickness = this.ChartTitle.StrokeThickness;
                if (strokeThickness > 0.0)
                {
                    if ((this._parentView != null) && (this._parentView.ParentViewport != null))
                    {
                        strokeThickness *= this._parentView.ParentViewport.Sheet.ZoomFactor;
                    }
                    this._titleFrame.StrokeThickness = strokeThickness;
                }
                else
                {
                    this._titleFrame.StrokeThickness = 0.0;
                }
                Brush actualStroke = this.ChartTitle.ActualStroke;
                if (actualStroke != null)
                {
                    this._titleFrame.Stroke = actualStroke;
                }
                else
                {
                    this._titleFrame.Stroke = new SolidColorBrush(Colors.Transparent);
                }
                Brush actualFill = this.ChartTitle.ActualFill;
                if (actualFill != null)
                {
                    this._titleFrame.Fill = actualFill;
                }
                else
                {
                    this._titleFrame.Fill = new SolidColorBrush(Colors.Transparent);
                }
                this._titleContent.Text = this.ChartTitle.Text;
                FontFamily actualFontFamily = this.ChartTitle.ActualFontFamily;
                if (actualFontFamily == null)
                {
                    actualFontFamily = Utility.DefaultFontFamily;
                }
                this._titleContent.FontFamily = actualFontFamily;
                double actualFontSize = this.ChartTitle.ActualFontSize;
                if ((this._parentView.ParentViewport != null) && (this._parentView.ParentViewport.Sheet != null))
                {
                    actualFontSize *= this._parentView.ParentViewport.Sheet.ZoomFactor;
                }
                if (actualFontSize > 0.0)
                {
                    this._titleContent.FontSize = actualFontSize;
                }
                this._titleContent.FontStretch = this.ChartTitle.ActualFontStretch;
                this._titleContent.FontStyle = this.ChartTitle.ActualFontStyle;
                this._titleContent.FontWeight = this.ChartTitle.ActualFontWeight;
                Brush actualForeground = this.ChartTitle.ActualForeground;
                if (actualForeground != null)
                {
                    this._titleContent.Foreground = actualForeground;
                }
                else
                {
                    this._titleContent.ClearValue(TextBlock.ForegroundProperty);
                }
            }
            else
            {
                this._titleFrame.Stroke = null;
                this._titleFrame.Fill = null;
                this._titleContent.ClearValue(TextBlock.TextProperty);
            }
        }

        public Dt.Cells.Data.ChartTitle ChartTitle
        {
            get { return  this._chartTitle; }
            set
            {
                this._chartTitle = value;
                this.UpdateChartTitle();
            }
        }
    }
}

