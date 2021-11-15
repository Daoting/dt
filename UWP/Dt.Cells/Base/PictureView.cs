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

        public PictureView(Picture picture, CellsPanel parentViewport)
        {
            _picture = picture;
            ParentViewport = parentViewport;
            _backgroundRect = new Rectangle();
            Children.Add(_backgroundRect);
            _image = new Image();
            Children.Add(_image);
            SyncPictureView();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
#if UWP
            _image.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
#endif
            _backgroundRect.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size availableSize)
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
            if (_picture.ImageSource != null)
            {
                _image.Source = _picture.ImageSource;
            }
            else if (_picture.UriSource != null)
            {
                _image.Source = new BitmapImage(_picture.UriSource);
            }
            Brush actualFill = _picture.ActualFill;
            if (actualFill != null)
            {
                _backgroundRect.Fill = actualFill;
            }
            else
            {
                _backgroundRect.Fill = new SolidColorBrush(Colors.Transparent);
            }
            _backgroundRect.Stroke = _picture.ActualStroke;
            double num = _picture.StrokeThickness * ParentViewport.Excel.ZoomFactor;
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

        public CellsPanel ParentViewport { get; private set; }
    }
}

