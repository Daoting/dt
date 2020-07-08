#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using Windows.Devices.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    public static class Attached
    {
        #region Cursor
        /// <summary>
        /// 光标附加依赖项属性
        /// </summary>
        public static readonly DependencyProperty CursorProperty =
            DependencyProperty.RegisterAttached(
                "Cursor",
                typeof(CoreCursorType),
                typeof(Attached),
                new PropertyMetadata(CoreCursorType.Arrow, OnCursorChanged));

        /// <summary>
        /// 获取光标附加属性
        /// </summary>
        public static CoreCursorType GetCursor(this DependencyObject d)
        {
            return (CoreCursorType)d.GetValue(CursorProperty);
        }

        /// <summary>
        /// 设置光标附加属性
        /// </summary>
        public static void SetCursor(this DependencyObject d, CoreCursorType value)
        {
            d.SetValue(CursorProperty, value);
        }

        /// <summary>
        /// 切换光标处理
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        static void OnCursorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CoreCursorType oldCursor = (CoreCursorType)e.OldValue;
            CoreCursorType newCursor = (CoreCursorType)d.GetValue(CursorProperty);

            if (oldCursor == CoreCursorType.Arrow)
            {
                var handler = new CursorDisplayHandler();
                handler.Attach((UIElement)d);
                SetCursorDisplayHandler(d, handler);
            }
            else
            {
                var handler = GetCursorDisplayHandler(d);
                if (newCursor == CoreCursorType.Arrow)
                {
                    handler.Detach();
                    SetCursorDisplayHandler(d, null);
                }
                else
                {
                    handler.UpdateCursor();
                }
            }
        }

        /// <summary>
        /// 光标显示Handler，内部用
        /// </summary>
        public static readonly DependencyProperty CursorDisplayHandlerProperty =
            DependencyProperty.RegisterAttached(
                "CursorDisplayHandler",
                typeof(CursorDisplayHandler),
                typeof(Attached),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取光标Handler
        /// </summary>
        public static CursorDisplayHandler GetCursorDisplayHandler(DependencyObject d)
        {
            return (CursorDisplayHandler)d.GetValue(CursorDisplayHandlerProperty);
        }

        /// <summary>
        /// 设置光标Handler
        /// </summary>
        public static void SetCursorDisplayHandler(DependencyObject d, CursorDisplayHandler value)
        {
            d.SetValue(CursorDisplayHandlerProperty, value);
        }
        #endregion
    }

    /// <summary>
    /// 光标实例
    /// </summary>
    public static class Cursors
    {
        static readonly CoreCursor _defaultCursor = new CoreCursor(CoreCursorType.Arrow, 0);
        static readonly Dictionary<CoreCursorType, CoreCursor> _cursors;
        static Cursors()
        {
            _defaultCursor = new CoreCursor(CoreCursorType.Arrow, 0);
            _cursors = new Dictionary<CoreCursorType, CoreCursor>();
            _cursors[CoreCursorType.Arrow] = _defaultCursor;
            _cursors[CoreCursorType.Cross] = new CoreCursor(CoreCursorType.Cross, 0);
            _cursors[CoreCursorType.Hand] = new CoreCursor(CoreCursorType.Hand, 0);
            _cursors[CoreCursorType.Help] = new CoreCursor(CoreCursorType.Help, 0);
            _cursors[CoreCursorType.IBeam] = new CoreCursor(CoreCursorType.IBeam, 0);
            _cursors[CoreCursorType.SizeAll] = new CoreCursor(CoreCursorType.SizeAll, 0);
            _cursors[CoreCursorType.SizeNortheastSouthwest] = new CoreCursor(CoreCursorType.SizeNortheastSouthwest, 0);
            _cursors[CoreCursorType.SizeNorthSouth] = new CoreCursor(CoreCursorType.SizeNorthSouth, 0);
            _cursors[CoreCursorType.SizeNorthwestSoutheast] = new CoreCursor(CoreCursorType.SizeNorthwestSoutheast, 0);
            _cursors[CoreCursorType.SizeWestEast] = new CoreCursor(CoreCursorType.SizeWestEast, 0);
            _cursors[CoreCursorType.UniversalNo] = new CoreCursor(CoreCursorType.UniversalNo, 0);
            _cursors[CoreCursorType.UpArrow] = new CoreCursor(CoreCursorType.UpArrow, 0);
            _cursors[CoreCursorType.Wait] = new CoreCursor(CoreCursorType.Wait, 0);
        }

        /// <summary>
        /// 获取光标实例
        /// </summary>
        /// <param name="p_curType"></param>
        /// <returns></returns>
        public static CoreCursor GetCursor(CoreCursorType p_curType)
        {
            return _cursors[p_curType];
        }

        /// <summary>
        /// 默认光标
        /// </summary>
        public static CoreCursor DefaultCursor
        {
            get { return _defaultCursor; }
        }
    }

    /// <summary>
    /// 切换光标处理
    /// </summary>
    public class CursorDisplayHandler
    {
        UIElement _elem;
        bool _isHovering;

        /// <summary>
        /// 附加鼠标进入离开事件
        /// </summary>
        /// <param name="p_elem"></param>
        public void Attach(UIElement p_elem)
        {
            _elem = p_elem;
            _elem.PointerEntered += OnPointerEntered;
            _elem.PointerExited += OnPointerExited;
        }

        /// <summary>
        /// 移除鼠标事件
        /// </summary>
        public void Detach()
        {
            _elem.PointerEntered -= OnPointerEntered;
            _elem.PointerExited -= OnPointerExited;
            if (_isHovering)
            {
                Window.Current.CoreWindow.PointerCursor = Cursors.DefaultCursor;
            }
        }

        /// <summary>
        /// 更新光标
        /// </summary>
        internal void UpdateCursor()
        {
            if (_isHovering)
            {
                Window.Current.CoreWindow.PointerCursor = Cursors.GetCursor(_elem.GetCursor());
            }
        }

        /// <summary>
        /// 进入时更新光标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                _isHovering = true;
                Window.Current.CoreWindow.PointerCursor = Cursors.GetCursor(_elem.GetCursor());
            }
        }

        /// <summary>
        /// 离开时重置默认光标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                _isHovering = false;
                Window.Current.CoreWindow.PointerCursor = Cursors.DefaultCursor;
            }
        }
    }
}
