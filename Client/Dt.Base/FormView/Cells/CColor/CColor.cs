﻿#region 文件描述
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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
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
        /// <param name="p_color"></param>
        public void SelectColor(Color p_color)
        {
            Type type = (Type)ValBinding.ConverterParameter;
            if (type == typeof(string))
            {
#if WIN
                Val = p_color.ToString();
#else
                Val = ColorToStr(p_color);
#endif
            }
            else if (type == typeof(Color))
            {
                Val = p_color;
            }
            else if (type == typeof(SolidColorBrush) || type == typeof(Brush))
            {
                Val = new SolidColorBrush(p_color);
            }
            LoadColorUI(new SolidColorBrush(p_color));
        }

        protected override void OnApplyCellTemplate()
        {
            _grid = (Grid)GetTemplateChild("Grid");
#if WIN
            // TextBlock可复制
            _grid.AddHandler(TappedEvent, new TappedEventHandler(OnShowDlg), true);
#else
            _grid.Tapped += OnShowDlg;
#endif
        }

        protected override void SetValBinding()
        {
            object data = Val;
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
                    else
                    {
                        LoadColorUI(Res.BlackBrush);
                    }
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

        public override void Destroy()
        {
            if (_grid != null)
            {
#if WIN
                _grid.RemoveHandler(TappedEvent, new TappedEventHandler(OnShowDlg));
#else
                _grid.Tapped -= OnShowDlg;
#endif
            }
            
            if (_dlg != null)
            {
                _dlg.Destroy();
                _dlg = null;
            }
        }

        void LoadColorUI(SolidColorBrush p_brush)
        {
            ColorBrush = p_brush;
#if WIN
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
                _dlg = new ColorDlg { Owner = this, Title = "选择颜色" };
                if (!Kit.IsPhoneUI)
                {
                    _dlg.WinPlacement = DlgPlacement.TargetBottomLeft;
                    _dlg.PlacementTarget = _grid;
                    _dlg.ClipElement = _grid;
                    _dlg.Height = 300;
                    _dlg.Width = 330;
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