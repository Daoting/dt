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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// bool格
    /// </summary>
    public partial class CBool : FvCell
    {
        #region 静态内容
        /// <summary>
        /// 是否显示为开关
        /// </summary>
        public static readonly DependencyProperty IsSwitchProperty = DependencyProperty.Register(
            "IsSwitch",
            typeof(bool),
            typeof(CBool),
            new PropertyMetadata(false, OnIsSwitchChanged));

        /// <summary>
        /// True时的值
        /// </summary>
        public static readonly DependencyProperty TrueValProperty = DependencyProperty.Register(
            "TrueVal",
            typeof(object),
            typeof(CBool),
            new PropertyMetadata(true));

        /// <summary>
        /// False时的值
        /// </summary>
        public static readonly DependencyProperty FalseValProperty = DependencyProperty.Register(
            "FalseVal",
            typeof(object),
            typeof(CBool),
            new PropertyMetadata(false));

        static void OnIsSwitchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CBool c = (CBool)d;
            if (c._panel != null)
                c.LoadBoolUI();
        }
        #endregion

        public CBool()
        {
            DefaultStyleKey = typeof(CBool);
        }

        /// <summary>
        /// 获取设置是否显示为开关，默认false为选择框
        /// </summary>
        [CellParam("是否为开关")]
        public bool IsSwitch
        {
            get { return (bool)GetValue(IsSwitchProperty); }
            set { SetValue(IsSwitchProperty, value); }
        }

        /// <summary>
        /// 获取设置True时的值
        /// </summary>
        [CellParam("True时的值")]
        public object TrueVal
        {
            get { return GetValue(TrueValProperty); }
            set { SetValue(TrueValProperty, value); }
        }

        /// <summary>
        /// 获取设置False时的值
        /// </summary>
        [CellParam("False时的值")]
        public object FalseVal
        {
            get { return GetValue(FalseValProperty); }
            set { SetValue(FalseValProperty, value); }
        }

        protected override IFvCall DefaultMiddle => new BoolBoxConverter();

        protected override void OnApplyCellTemplate()
        {
            LoadBoolUI();
        }

        protected override void SetValBinding()
        {
            if (_panel.Child is CheckBox cb)
                cb.SetBinding(CheckBox.IsCheckedProperty, ValBinding);
            else if (_panel.Child is ToggleSwitch ts)
                ts.SetBinding(ToggleSwitch.IsOnProperty, ValBinding);
        }
        
        void LoadBoolUI()
        {
            if (IsSwitch)
            {
                ToggleSwitch ts = new ToggleSwitch { MinWidth = 100, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
                Binding bind = new Binding { Path = new PropertyPath("ReadOnlyBinding"), Converter = new BoolToggleConverter(), Source = this };
                ts.SetBinding(ToggleSwitch.IsEnabledProperty, bind);
                if (_isLoaded)
                    ts.SetBinding(ToggleSwitch.IsOnProperty, ValBinding);
                _panel.Child = ts;
            }
            else
            {
                CheckBox cb = new CheckBox { MinWidth = 30, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                if (!ShowTitle)
                {
                    // 不显示标题时
                    cb.Content = Title;
                    cb.HorizontalAlignment = HorizontalAlignment.Left;
                    cb.Margin = new Thickness(10, 0, 10, 0);
                    _panel.Background = Res.浅灰1;
                }
                Binding bind = new Binding { Path = new PropertyPath("ReadOnlyBinding"), Converter = new BoolToggleConverter(), Source = this };
                cb.SetBinding(CheckBox.IsEnabledProperty, bind);
                if (_isLoaded)
                    cb.SetBinding(CheckBox.IsCheckedProperty, ValBinding);
                _panel.Child = cb;
            }
        }
    }

    class BoolBoxConverter : IFvCall
    {
        public object Get(Mid m)
        {
            return object.Equals(m.Val, ((CBool)m.Cell).TrueVal);
        }

        public object Set(Mid m)
        {
            var cell = (CBool)m.Cell;
            return (bool)m.Val ? cell.TrueVal : cell.FalseVal;
        }
    }
}