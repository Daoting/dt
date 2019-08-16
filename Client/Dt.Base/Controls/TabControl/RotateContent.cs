#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 支持内容旋转
    /// </summary>
    public partial class RotateContent : ContentControl
    {
        /// <summary>
        /// 内容的旋转方式
        /// </summary>
        public static readonly DependencyProperty RotateProperty = DependencyProperty.Register(
            "Rotate",
            typeof(ContentRotate),
            typeof(RotateContent),
            new PropertyMetadata(ContentRotate.Horizontal, OnRotateChanged));

        static void OnRotateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotateContent)d).InvalidateMeasure();
        }

        public RotateContent()
        {
            DefaultStyleKey = typeof(ContentControl);
        }

        /// <summary>
        /// 获取设置内容的旋转方式
        /// </summary>
        public ContentRotate Rotate
        {
            get { return (ContentRotate)GetValue(RotateProperty); }
            set { SetValue(RotateProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Rotate != ContentRotate.Horizontal)
            {
                return Swap(base.MeasureOverride(Swap(availableSize)));
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size result;
            RenderTransformOrigin = new Point(0.0, 0.0);

            if (Rotate != ContentRotate.Horizontal)
            {
                result = Swap(base.ArrangeOverride(Swap(finalSize)));
            }
            else
            {
                result = base.ArrangeOverride(finalSize);
            }

            TransformGroup renderTransform = new TransformGroup();
            if (Rotate == ContentRotate.RotatedTop)
            {
                RotateTransform tran = new RotateTransform();
                tran.Angle = -90.0;
                tran.CenterY = 0.0;
                tran.CenterX = 0.0;
                renderTransform.Children.Add(tran);

                TranslateTransform translate = new TranslateTransform();
                translate.Y = result.Height;
                renderTransform.Children.Add(translate);
            }
            else if (Rotate == ContentRotate.RotatedBottom)
            {
                RotateTransform tran = new RotateTransform();
                tran.Angle = 90.0;
                tran.CenterY = 0.0;
                tran.CenterX = 0.0;
                renderTransform.Children.Add(tran);

                TranslateTransform translate = new TranslateTransform();
                translate.X = result.Width;
                renderTransform.Children.Add(translate);
            }
            RenderTransform = renderTransform;
            return result;
        }

        Size Swap(Size p_size)
        {
            return new Size(p_size.Height, p_size.Width);
        }
    }

    /// <summary>
    /// 内容旋转方式
    /// </summary>
    public enum ContentRotate
    {
        /// <summary>
        /// 保持水平
        /// </summary>
        Horizontal,

        /// <summary>
        /// 旋转-90度
        /// </summary>
        RotatedTop,

        /// <summary>
        /// 旋转90度
        /// </summary>
        RotatedBottom
    }
}