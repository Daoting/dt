#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2011-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 添加了中间层数据加工的属性元
    /// </summary>
    public class CoercePropertyMetadata : PropertyMetadata
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_defaultValue">应用此 PropertyMetadata 的属性的默认值</param>
        /// <param name="p_propertyChangedCallback">用于为属性更改行为进行调用的回调</param>
        /// <param name="p_coerceValueCallback">对值加工回调</param>
        public CoercePropertyMetadata(object p_defaultValue, PropertyChangedCallback p_propertyChangedCallback, CoerceValueCallback p_coerceValueCallback)
            : base(p_defaultValue, Create(p_propertyChangedCallback, p_coerceValueCallback))
        {
        }

        /// <summary>
        /// 为基类动态构造PropertyChangedCallback类型的回调方法（动态生成方法）
        /// 该方法在依赖属性系统中的属性值发生变化时调用
        /// </summary>
        /// <param name="p_propertyChangedCallback">依赖项有效属性值更改时调用的回调。</param>
        /// <param name="p_coerceValueCallback">值加工回调</param>
        /// <returns></returns>
        static PropertyChangedCallback Create(PropertyChangedCallback p_propertyChangedCallback, CoerceValueCallback p_coerceValueCallback)
        {
            return delegate(DependencyObject p_element, DependencyPropertyChangedEventArgs p_args)
            {
                // 判断是否正在执行重置回调
                if (p_element.GetIsCoercing())
                    return;

                bool shouldCallPropertyChanged = true;
                if (p_coerceValueCallback != null && !p_element.GetStopCoerce())
                {
                    // 增加中间层数据加工回调
                    object coercedValue = p_coerceValueCallback(p_element, p_args.NewValue);
                    // 若重置后的值发生变化，设置依赖项属性
                    if (!object.Equals(coercedValue, p_args.NewValue))
                    {
                        // 置正在重置标志
                        p_element.SetIsCoercing(true);
                        // 将加工后的值重新设置到当前依赖属性！！！
                        p_element.SetValue(p_args.Property, coercedValue);
                        // 置重置结束标志
                        p_element.SetIsCoercing(false);
                    }
                    shouldCallPropertyChanged = !object.Equals(coercedValue, p_args.OldValue);
                }

                if (p_propertyChangedCallback != null && shouldCallPropertyChanged)
                {
                    p_propertyChangedCallback(p_element, p_args);
                }
            };
        }
    }

    /// <summary>
    /// 注册附加依赖属性
    /// </summary>
    public static class DependencyPropertyExtensions
    {
        /// <summary>
        /// 元素的属性值是否正在进行强制设置
        /// </summary>
        internal static readonly DependencyProperty IsCoercingProperty = DependencyProperty.RegisterAttached("IsCoercing", typeof(bool), typeof(DependencyObject), null);

        internal static bool GetIsCoercing(this DependencyObject element)
        {
            return (bool)element.GetValue(IsCoercingProperty);
        }

        internal static void SetIsCoercing(this DependencyObject element, bool allowed)
        {
            element.SetValue(IsCoercingProperty, allowed);
        }

        /// <summary>
        /// 是否停止中间层数据加工
        /// </summary>
        internal static readonly DependencyProperty StopCoerceProperty = DependencyProperty.RegisterAttached("StopCoerce", typeof(bool), typeof(DependencyObject), null);

        internal static bool GetStopCoerce(this DependencyObject element)
        {
            return (bool)element.GetValue(StopCoerceProperty);
        }

        internal static void SetStopCoerce(this DependencyObject element, bool allowed)
        {
            element.SetValue(StopCoerceProperty, allowed);
        }
    }

    /// <summary>
    /// 中间层数据加工回调原型
    /// </summary>
    /// <param name="element">
    /// 该属性所在的对象。在调用该回调时，属性系统将会传递该值。
    /// </param>
    /// <param name="baseValue">该属性在尝试执行任何强制转换之前的新值。</param>
    /// <returns>强制转换后的值（采用适当的类型）。</returns>
    public delegate object CoerceValueCallback(DependencyObject element, object baseValue);
}