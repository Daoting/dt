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
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 附件依赖项属性
    /// </summary>
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
            Throw.IfNull(element, "上下文菜单的目标为空！");

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
        /// 双向绑定 Lv.SelectionMode 和 Mi.IsChecked，附加到Mi上
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

        #region 标签类型
        /// <summary>
        /// 附加标签的类型名称，包括命名空间，不同程序集引用时需要提供程序集名称，不提供按调用方所在的程序集
        /// </summary>
        public static readonly DependencyProperty TagClsProperty = DependencyProperty.RegisterAttached(
            "TagCls",
            typeof(string),
            typeof(Ex),
            new PropertyMetadata(null));

        /// <summary>
        /// 获取附加标签的类型名称
        /// </summary>
        public static string GetTagCls(this DependencyObject element)
        {
            return (string)element.GetValue(TagClsProperty);
        }

        /// <summary>
        /// 设置附加标签的类型名称
        /// </summary>
        public static void SetTagCls(this DependencyObject element, string value)
        {
            element.SetValue(TagClsProperty, value);
        }

        /// <summary>
        /// 内部保存标签实例
        /// </summary>
        static readonly DependencyProperty TagObjProperty = DependencyProperty.RegisterAttached(
            "TagObj",
            typeof(object),
            typeof(Ex),
            new PropertyMetadata(null));

        /// <summary>
        /// 根据附加标签的类型名称创建类型实例
        /// </summary>
        /// <param name="element"></param>
        /// <param name="p_newObj">是否每次调用都实例化新对象</param>
        /// <returns></returns>
        public static object GetTagClsObj(this DependencyObject element, bool p_newObj = false)
        {
            Throw.IfNull(element);
            string name = GetTagCls(element);
            if (string.IsNullOrEmpty(name))
                return null;

            object obj = null;
            if (!p_newObj)
            {
                // 可复用实例
                obj = element.GetValue(TagObjProperty);
                if (obj != null)
                    return obj;
            }

            // 不可替换成GetClsType()！
            if (!name.Contains(","))
            {
                // 未提供程序集名称时，按调用类型所在的程序集
                var mth = new StackTrace().GetFrame(1).GetMethod();
                string str = mth.ReflectedType.AssemblyQualifiedName;
                name = name + str.Substring(str.IndexOf(','));
            }
            Type tp = Type.GetType(name, false);

            if (tp != null)
            {
                obj = Activator.CreateInstance(tp);
                if (!p_newObj)
                    element.SetValue(TagObjProperty, obj);
            }
            return obj;
        }

        /// <summary>
        /// 根据附加标签的类型名称获取类型
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Type GetTagClsType(this DependencyObject element)
        {
            Throw.IfNull(element);
            string name = GetTagCls(element);
            if (string.IsNullOrEmpty(name))
                return null;

            if (!name.Contains(","))
            {
                // 未提供程序集名称时，按调用类型所在的程序集
                var mth = new StackTrace().GetFrame(1).GetMethod();
                string str = mth.ReflectedType.AssemblyQualifiedName;
                name = name + str.Substring(str.IndexOf(','));
            }
            return Type.GetType(name, false);
        }
        #endregion

        #region Win停靠在两侧时的宽度
        internal static readonly DependencyProperty SplitWidthProperty =
            DependencyProperty.RegisterAttached(
                "SplitWidth",
                typeof(double),
                typeof(Ex),
                new PropertyMetadata(0.0));

        /// <summary>
        /// 获取Win停靠在两侧时的宽度
        /// </summary>
        public static double GetSplitWidth(this Win d)
        {
            return (double)d.GetValue(SplitWidthProperty);
        }

        /// <summary>
        /// 设置Win停靠在两侧时的宽度
        /// </summary>
        public static void SetSplitWidth(this Win d, double value)
        {
            d.SetValue(SplitWidthProperty, value);
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
