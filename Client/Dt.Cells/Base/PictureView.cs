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
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal partial class PictureView : Panel
    {
        private Rectangle _backgroundRect;
        private Image _image;
        private Picture _picture;

        public PictureView(Picture picture, GcViewport parentViewport)
        {
            Action action = null;
            this._picture = picture;
            this.ParentViewport = parentViewport;
            if (action == null)
            {
                action = delegate
                {
                    this._backgroundRect = new Rectangle();
                    base.Children.Add(this._backgroundRect);
                    this._image = new Image();
                    base.Children.Add(this._image);
                };
            }
            Dt.Cells.Data.UIAdaptor.InvokeSync(action);
            this.SyncPictureView();
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
#if UWP
            this._image.Arrange(new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
#endif
            this._backgroundRect.Arrange(new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            return base.ArrangeOverride(finalSize);
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
#if UWP
            this._image.Measure(availableSize);
#endif
            this._backgroundRect.Measure(availableSize);
            return base.MeasureOverride(availableSize);
        }

        internal void RefreshPictureView()
        {
            this.SyncPictureView();
        }

        private void SyncPictureView()
        {
            Action action = null;
            Action action2 = null;
            if (this._picture.ImageSource != null)
            {
                this._image.Source = this._picture.ImageSource;
            }
            else if (this._picture.UriSource != null)
            {
                if (action == null)
                {
                    action = delegate
                    {
                        this._image.Source = new BitmapImage(this._picture.UriSource);
                    };
                }
                Dt.Cells.Data.UIAdaptor.InvokeSync(action);
            }
            Brush actualFill = this._picture.ActualFill;
            if (actualFill != null)
            {
                this._backgroundRect.Fill = actualFill;
            }
            else
            {
                if (action2 == null)
                {
                    action2 = delegate
                    {
                        this._backgroundRect.Fill = new SolidColorBrush(Colors.Transparent);
                    };
                }
                Dt.Cells.Data.UIAdaptor.InvokeSync(action2);
            }
            this._backgroundRect.Stroke = this._picture.ActualStroke;
            double num = this._picture.StrokeThickness * this.ParentViewport.Sheet.ZoomFactor;
            if (num > 0.0)
            {
                this._backgroundRect.StrokeThickness = num;
            }
            else
            {
                this._backgroundRect.StrokeThickness = 0.0;
            }
            this._backgroundRect.StrokeDashArray = Dt.Cells.Data.StrokeDashHelper.GetStrokeDashes(this._picture.StrokeDashType);
            this._image.Stretch = this._picture.PictureStretch;
        }

        public GcViewport ParentViewport { get; private set; }
    }
}

