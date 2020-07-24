#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Contains the extension methods.
    /// </summary>
    internal static class Extensions
    {
        static bool _areHandlersSuspended;

        /// <summary>
        /// Determines whether the events are suspended for the specified object.
        /// </summary>
        /// <param name="depObj">
        /// The DependencyObject.
        /// </param>
        /// <returns>
        /// <c>true</c> if the events are suspended, otherwise, returns false.
        /// </returns>
        public static bool AreHandlersSuspended(this DependencyObject depObj)
        {
            return _areHandlersSuspended;
        }

        public static void ResumeAllHandlers(this DependencyObject depObj)
        {
            _areHandlersSuspended = false;
        }

        /// <summary>
        /// Determines whether the value should be set without raising events.
        /// </summary>
        /// <param name="depObj">
        /// The DependencyObject.
        /// </param>
        /// <param name="dp">
        /// The DependencyProperty.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetValueNoCallBack(this DependencyObject depObj, DependencyProperty dp, object value)
        {
            try
            {
                _areHandlersSuspended = true;
                depObj.SetValue(dp, value);
            }
            catch
            {
            }
            finally
            {
                _areHandlersSuspended = false;
            }
        }

        public static void SuspendAllHandlers(this DependencyObject depObj)
        {
            _areHandlersSuspended = true;
        }

        public static void TypeSafeSetStyle(this FrameworkElement element, Style style)
        {
            if (((element != null) && (style != null)) && (style.TargetType == element.GetType()))
            {
                element.Style = style;
            }
        }
    }
}

