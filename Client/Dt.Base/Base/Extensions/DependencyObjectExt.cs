#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Diagnostics.CodeAnalysis;
using Windows.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// DependencyObject扩展类
    /// </summary>
    public static class DependencyObjectExt
    {
        /// <summary>
        /// 判断依赖属性是否已设置本地值
        /// </summary>
        /// <seealso cref="M:System.Windows.DependencyObject.ReadLocalValue(System.Windows.DependencyProperty)" />
        /// <param name="source">依赖对象</param>
        /// <param name="p_property">依赖属性</param>
        /// <returns></returns>
        public static bool ExistLocalValue(this DependencyObject source, DependencyProperty p_property)
        {
            return (source.ReadLocalValue(p_property) != DependencyProperty.UnsetValue);
        }
    }
}
