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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 图标选择格
    /// 未使用绑定，采用直接取值/赋值的方式
    /// </summary>
    public partial class CIcon : FvCell
    {
        #region 静态内容
        /// <summary>
        /// 图标字符
        /// </summary>
        public readonly static DependencyProperty IconCharProperty = DependencyProperty.Register(
            "IconChar",
            typeof(string),
            typeof(CIcon),
            new PropertyMetadata(null));

        /// <summary>
        /// 图标描述
        /// </summary>
        public readonly static DependencyProperty DescProperty = DependencyProperty.Register(
            "Desc",
            typeof(string),
            typeof(CIcon),
            new PropertyMetadata(null));
        #endregion

        Grid _grid;
        IconDlg _dlg;

        public CIcon()
        {
            DefaultStyleKey = typeof(CIcon);
        }

        /// <summary>
        /// 获取设置图标字符
        /// </summary>
        public string IconChar
        {
            get { return (string)GetValue(IconCharProperty); }
            set { SetValue(IconCharProperty, value); }
        }

        /// <summary>
        /// 获取设置图标描述
        /// </summary>
        public string Desc
        {
            get { return (string)GetValue(DescProperty); }
            set { SetValue(DescProperty, value); }
        }

        /// <summary>
        /// 设置选择结果
        /// </summary>
        /// <param name="p_icon"></param>
        public void SelectIcon(Icons p_icon)
        {
            Type type = (Type)ValBinding.ConverterParameter;
            if (type == typeof(Icons) || type == typeof(int))
                SetVal(p_icon);
            else if (type == typeof(string))
                SetVal(p_icon.ToString());
            LoadIconUI(p_icon);
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
                if (type == typeof(Icons) || type == typeof(int))
                    LoadIconUI((Icons)data);
                else if (type == typeof(string))
                    LoadIconUI(Res.ParseIcon((string)data));
            }
            else
            {
                ClearValue(IconCharProperty);
                ClearValue(DescProperty);
            }
        }

        protected override bool SetFocus()
        {
            if (_grid != null)
                OnShowDlg(null, null);
            return true;
        }

        /// <summary>
        /// 格的显示内容
        /// </summary>
        /// <param name="p_icon"></param>
        void LoadIconUI(Icons p_icon)
        {
            IconChar = Res.GetIconChar(p_icon);
            string hex = Convert.ToString(0xE000 + (int)p_icon, 16).ToUpper();
            Desc = $"{p_icon} ({hex})";
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
                    _dlg = new IconDlg { Owner = this };
                }
                else
                {
                    _dlg = new IconDlg
                    {
                        Owner = this,
                        WinPlacement = DlgPlacement.TargetBottomLeft,
                        PlacementTarget = _grid,
                        ClipElement = _grid,
                        Height = 300,
                        Width = _grid.ActualWidth,
                    };
                }
                // 不向下层对话框传递Press事件
                _dlg.AllowRelayPress = false;
            }
            _dlg.Show();
        }
    }
}