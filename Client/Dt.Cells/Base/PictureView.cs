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
        Rectangle _backgroundRect;
        Image _image;
        Picture _picture;

        public PictureView(Picture picture, GcViewport parentViewport)
        {
            Action action = null;
            _picture = picture;
            ParentViewport = parentViewport;
            if (action == null)
            {
                action = delegate
                {
                    _backgroundRect = new Rectangle();
                    base.Children.Add(_backgroundRect);
                    _image = new Image();
                    base.Children.Add(_image);
                };
            }
            Dt.Cells.Data.UIAdaptor.InvokeSync(action);
            SyncPictureView();
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
#if UWP
            _image.Arrange(new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
#endif
            _backgroundRect.Arrange(new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            return base.ArrangeOverride(finalSize);
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
#if UWP
            _image.Measure(availableSize);
#endif
            _backgroundRect.Measure(availableSize);
            return base.MeasureOverride(availableSize);
        }

        internal void RefreshPictureView()
        {
            SyncPictureView();
        }

        void SyncPictureView()
        {
            Action action = null;
            Action action2 = null;
            if (_picture.ImageSource != null)
            {
                _image.Source = _picture.ImageSource;
            }
            else if (_picture.UriSource != null)
            {
                if (action == null)
                {
                    action = delegate
                    {
                        _image.Source = new BitmapImage(_picture.UriSource);
                    };
                }
                Dt.Cells.Data.UIAdaptor.InvokeSync(action);
            }
            Brush actualFill = _picture.ActualFill;
            if (actualFill != null)
            {
                _backgroundRect.Fill = actualFill;
            }
            else
            {
                if (action2 == null)
                {
                    action2 = delegate
                    {
                        _backgroundRect.Fill = new SolidColorBrush(Colors.Transparent);
                    };
                }
                Dt.Cells.Data.UIAdaptor.InvokeSync(action2);
            }
            _backgroundRect.Stroke = _picture.ActualStroke;
            double num = _picture.StrokeThickness * ParentViewport.Sheet.ZoomFactor;
            if (num > 0.0)
            {
                _backgroundRect.StrokeThickness = num;
            }
            else
            {
                _backgroundRect.StrokeThickness = 0.0;
            }
            _backgroundRect.StrokeDashArray = Dt.Cells.Data.StrokeDashHelper.GetStrokeDashes(_picture.StrokeDashType);
            _image.Stretch = _picture.PictureStretch;
        }

        public GcViewport ParentViewport { get; private set; }
    }
}

