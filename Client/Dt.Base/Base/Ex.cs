#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    public static class Ex
    {
        #region 上下文菜单
        /// <summary>
        /// 上下文菜单
        /// </summary>
        public static readonly DependencyProperty MenuProperty = DependencyProperty.RegisterAttached(
            "Menu",
            typeof(Menu),
            typeof(Ex),
            new PropertyMetadata(null, OnContextMenuChanged));

        /// <summary>
        /// 获取指定元素的上下文菜单
        /// </summary>
        public static Menu GetMenu(FrameworkElement element)
        {
            return (Menu)element.GetValue(MenuProperty);
        }

        /// <summary>
        /// 设置指定元素的上下文菜单
        /// </summary>
        public static void SetMenu(FrameworkElement element, Menu value)
        {
            element.SetValue(MenuProperty, value);
        }

        static void OnContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = d as FrameworkElement;
            if (element == null)
                AtKit.Throw("上下文菜单的目标为空！");

            if (e.OldValue is Menu oldMenu)
                oldMenu.UnloadContextMenu();

            if (e.NewValue is Menu newMenu)
                newMenu.InitContextMenu(element);

            if (d is IMenuHost cm)
                cm.UpdateContextMenu();
        }
        #endregion

        #region Lv多选状态
        /// <summary>
        /// 双向绑定 Lv.SelectionMode <=> Mi.IsChecked，附加到Mi上
        /// </summary>
        public static readonly DependencyProperty LvMultiSelectProperty =
            DependencyProperty.RegisterAttached(
                "LvMultiSelect",
                typeof(Lv),
                typeof(Ex),
                new PropertyMetadata(null, OnMultiSelectChanged));

        static void OnMultiSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                Binding bind = new Binding
                {
                    Path = new PropertyPath("SelectionMode"),
                    Converter = new ToggleSelectionModeConverter(),
                    Source = e.NewValue,
                    Mode = BindingMode.TwoWay,
                };
                ((Mi)d).SetBinding(Mi.IsCheckedProperty, bind);
            }
        }

        /// <summary>
        /// 获取Mi要绑定的Lv
        /// </summary>
        public static Lv GetLvMultiSelect(this Mi d)
        {
            return (Lv)d.GetValue(LvMultiSelectProperty);
        }

        /// <summary>
        /// 设置Mi要绑定的Lv
        /// </summary>
        public static void SetLvMultiSelect(this Mi d, Lv value)
        {
            d.SetValue(LvMultiSelectProperty, value);
        }
        #endregion
    }

    /// <summary>
    /// SelectionMode多选/单选 -> bool
    /// </summary>
    class ToggleSelectionModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is SelectionMode mode)
                return mode == SelectionMode.Multiple;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isMulti)
                return isMulti ? SelectionMode.Multiple : SelectionMode.Single;
            return SelectionMode.Single;
        }
    }

}
