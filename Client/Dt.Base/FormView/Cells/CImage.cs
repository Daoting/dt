#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 图像格
    /// </summary>
    public partial class CImage : FvCell
    {
        #region 成员变量
        readonly FileList _fl;
        #endregion

        #region 构造方法
        public CImage()
        {
            DefaultStyleKey = typeof(CImage);

            _fl = new FileList();
            _fl.MaxFileCount = 1;
            // 确保无图像时保证高度
            _fl.MinHeight = _fl.ImageHeight;
            // 凑左上边框1
            _fl.Margin = new Thickness(-1, -1, 0, 0);

            // 自动行高
            RowSpan = -1;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置图像的显示高度，默认82，0表示和宽度相同
        /// </summary>
        [CellParam("图像高度")]
        public double ImageHeight
        {
            get { return _fl.ImageHeight; }
            set
            {
                if (value >= 0 && value != _fl.ImageHeight)
                {
                    _fl.MinHeight = value;
                    _fl.ImageHeight = value;
                }
            }
        }

        /// <summary>
        /// 获取设置图像边距，默认6
        /// </summary>
        public Thickness ImagePadding
        {
            get { return _fl.ImagePadding; }
            set { _fl.ImagePadding = value; }
        }

        /// <summary>
        /// 获取设置要上传的固定卷名，默认null表示上传到普通卷
        /// </summary>
        [CellParam("固定卷名")]
        public string FixedVolume
        {
            get { return _fl.FixedVolume; }
            set { _fl.FixedVolume = value; }
        }
        #endregion

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var grid = (Grid)GetTemplateChild("Grid");
            grid.Children.Add(_fl);

            var btn = (Button)GetTemplateChild("BtnAdd");
            if (btn != null)
            {
                btn.Click -= OnAddImage;
                btn.Click += OnAddImage;
            }
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (Owner != null)
                Owner.OnCellClick(this);
        }

        protected override void SetValBinding()
        {
            _fl.SetBinding(FileList.DataProperty, ValBinding);
        }
        #endregion


        void OnAddImage(object sender, RoutedEventArgs e)
        {
            _fl.AddImage();
        }
    }
}