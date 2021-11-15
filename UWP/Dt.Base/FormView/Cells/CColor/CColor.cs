#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Globalization;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 颜色选择格
    /// 未使用绑定，采用直接取值/赋值的方式
    /// </summary>
    public partial class CColor : FvCell
    {
        #region 静态内容
        /// <summary>
        /// 选择的画刷
        /// </summary>
        public readonly static DependencyProperty ColorBrushProperty = DependencyProperty.Register(
            "ColorBrush",
            typeof(SolidColorBrush),
            typeof(CColor),
            new PropertyMetadata(null));

        /// <summary>
        /// 颜色描述
        /// </summary>
        public readonly static DependencyProperty DescProperty = DependencyProperty.Register(
            "Desc",
            typeof(string),
            typeof(CColor),
            new PropertyMetadata(null));
        #endregion

        Grid _grid;
        ColorDlg _dlg;

        public CColor()
        {
            DefaultStyleKey = typeof(CColor);
        }

        /// <summary>
        /// 获取设置选择的画刷
        /// </summary>
        public SolidColorBrush ColorBrush
        {
            get { return (SolidColorBrush)GetValue(ColorBrushProperty); }
            set { SetValue(ColorBrushProperty, value); }
        }

        /// <summary>
        /// 获取设置颜色描述
        /// </summary>
        public string Desc
        {
            get { return (string)GetValue(DescProperty); }
            set { SetValue(DescProperty, value); }
        }

        /// <summary>
        /// 设置选择结果
        /// </summary>
        /// <param name="p_brush"></param>
        public void SelectColor(SolidColorBrush p_brush)
        {
            Type type = (Type)ValBinding.ConverterParameter;
            if (type == typeof(string))
            {
#if UWP
                SetVal(p_brush.Color.ToString());
#else
                SetVal(ColorToStr(p_brush.Color));
#endif
            }
            else if (type == typeof(Color))
            {
                SetVal(p_brush.Color);
            }
            else if (type == typeof(SolidColorBrush) || type == typeof(Brush))
            {
                SetVal(p_brush);
            }
            LoadColorUI(p_brush);
        }

        protected override void OnApplyCellTemplate()
        {
            _grid = (Grid)GetTemplateChild("Grid");
#if UWP
            // TextBlock可复制
            _grid.AddHandler(TappedEvent, new TappedEventHandler(OnShowDlg), true);
#else
            _grid.Tapped += OnShowDlg;
#endif
        }

        protected override void SetValBinding()
        {
            object data = GetVal();
            if (data != null)
            {
                Type type = (Type)ValBinding.ConverterParameter;
                if (type == typeof(string))
                {
                    string strColor = (string)data;
                    if (strColor.StartsWith("#") && strColor.Length == 9)
                    {
                        try
                        {
                            Color color = Color.FromArgb(
                                byte.Parse(strColor.Substring(1, 2), NumberStyles.HexNumber),
                                byte.Parse(strColor.Substring(3, 2), NumberStyles.HexNumber),
                                byte.Parse(strColor.Substring(5, 2), NumberStyles.HexNumber),
                                byte.Parse(strColor.Substring(7, 2), NumberStyles.HexNumber));
                            LoadColorUI(new SolidColorBrush(color));
                        }
                        catch { }
                    }
                    LoadColorUI(Res.BlackBrush);
                }
                else if (type == typeof(Color))
                {
                    LoadColorUI(new SolidColorBrush((Color)data));
                }
                else if (type == typeof(SolidColorBrush))
                {
                    LoadColorUI((SolidColorBrush)data);
                }
            }
            else
            {
                ClearValue(ColorBrushProperty);
                ClearValue(DescProperty);
            }
        }

        protected override bool SetFocus()
        {
            if (_grid != null)
                OnShowDlg(null, null);
            return true;
        }

        void LoadColorUI(SolidColorBrush p_brush)
        {
            ColorBrush = p_brush;
#if UWP
            Desc = p_brush.Color.ToString();
#else
            Desc = ColorToStr(p_brush.Color);
#endif
        }

        void OnShowDlg(object sender, TappedRoutedEventArgs e)
        {
            if (ReadOnlyBinding)
                return;

            if (_dlg != null && _dlg.IsOpened)
            {
                _dlg.BringToTop();
                return;
            }

            if (_dlg == null)
            {
                if (Kit.IsPhoneUI)
                {
                    _dlg = new ColorDlg { Owner = this, Title = "选择颜色" };
                }
                else
                {
                    _dlg = new ColorDlg
                    {
                        Owner = this,
                        WinPlacement = DlgPlacement.TargetBottomLeft,
                        PlacementTarget = _grid,
                        ClipElement = _grid,
                        HideTitleBar = true,
                        Resizeable = false,
                        Height = 300,
                        Width = _grid.ActualWidth,
                    };
                }
                // 不向下层对话框传递Press事件
                _dlg.AllowRelayPress = false;
            }
            _dlg.Show();
        }

        static string ColorToStr(Color p_color)
        {
            StringBuilder sb = new StringBuilder("#");
            sb.Append(p_color.A == 0 ? "00" : System.Convert.ToString(p_color.A, 16));
            sb.Append(p_color.R == 0 ? "00" : System.Convert.ToString(p_color.R, 16));
            sb.Append(p_color.G == 0 ? "00" : System.Convert.ToString(p_color.G, 16));
            sb.Append(p_color.B == 0 ? "00" : System.Convert.ToString(p_color.B, 16));
            return sb.ToString();
        }
    }
}