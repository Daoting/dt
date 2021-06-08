#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Diagnostics;
using Dt.Base;
using Dt.Core;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Sample
{
    public sealed partial class RouteEventDemo : Win
    {
        bool _lastEventIsMoved = false;

        public RouteEventDemo()
        {
            InitializeComponent();

            _bdSingle.PointerEntered += OnSinglePointerEntered;
            _bdSingle.PointerExited += OnSinglePointerExited;
            _bdSingle.PointerMoved += OnSinglePointerMoved;
            _bdSingle.PointerPressed += OnSinglePointerPressed;
            _bdSingle.PointerReleased += OnSinglePointerReleased;
            _bdSingle.Tapped += OnSingleTapped;
            _bdSingle.RightTapped += OnSingleRightTapped;
            _bdSingle.DoubleTapped += OnSingleDoubleTapped;

            _bdParent.PointerEntered += OnRoutedPointerEntered;
            _bdParent.PointerExited += OnRoutedPointerExited;
            _bdParent.PointerMoved += OnRoutedPointerMoved;
            _bdParent.PointerPressed += OnRoutedPointerPressed;
            _bdParent.PointerReleased += OnRoutedPointerReleased;
            _bdParent.Tapped += OnRoutedTapped;
            _bdParent.DoubleTapped += OnRoutedDoubleTapped;

            _bdChild.PointerEntered += OnRoutedPointerEntered;
            _bdChild.PointerExited += OnRoutedPointerExited;
            _bdChild.PointerMoved += OnRoutedPointerMoved;
            _bdChild.PointerPressed += OnRoutedPointerPressed;
            _bdChild.PointerReleased += OnRoutedPointerReleased;
            _bdChild.Tapped += OnRoutedTapped;
            _bdChild.DoubleTapped += OnRoutedDoubleTapped;

            _bdAdd.AddHandler(PointerEnteredEvent, new PointerEventHandler(PointerEnteredHandler), true);
            _bdAdd.AddHandler(PointerExitedEvent, new PointerEventHandler(PointerExitedHandler), true);
            _bdAdd.AddHandler(PointerMovedEvent, new PointerEventHandler(PointerMovedHandler), true);
            _bdAdd.AddHandler(PointerPressedEvent, new PointerEventHandler(PointerPressedHandler), true);
            _bdAdd.AddHandler(PointerReleasedEvent, new PointerEventHandler(PointerReleasedHandler), true);
            _bdAdd.AddHandler(TappedEvent, new TappedEventHandler(TappedHandler), true);
            _bdAdd.AddHandler(DoubleTappedEvent, new DoubleTappedEventHandler(DoubleTappedHandler), true);

            _bdAddChild.PointerEntered += PointerEnteredHandler;
            _bdAddChild.PointerExited += PointerExitedHandler;
            _bdAddChild.PointerMoved += PointerMovedHandler;
            _bdAddChild.PointerPressed += PointerPressedHandler;
            _bdAddChild.PointerReleased += PointerReleasedHandler;
            _bdAddChild.Tapped += TappedHandler;
            _bdAddChild.DoubleTapped += DoubleTappedHandler;

            _bdHold.Holding += _bdHold_Holding;
        }

        #region 父子事件路由
        void OnRoutedPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbRoute.Text += $"\r\nEntered, sender={GetName(sender)}";
            if (sender == _bdChild)
                e.Handled = _cbHandled.IsOn;
        }

        void OnRoutedPointerExited(object sender, PointerRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbRoute.Text += $"\r\nExited, sender={GetName(sender)}";
            if (sender == _bdChild)
                e.Handled = _cbHandled.IsOn;
        }

        void OnRoutedPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_lastEventIsMoved)
                return;

            _tbRoute.Text += $"\r\nMoved, sender={GetName(sender)}";
            if (sender == _bdChild)
                e.Handled = _cbHandled.IsOn;
            _lastEventIsMoved = true;
        }

        void OnRoutedPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbRoute.Text = $"Pressed, sender={GetName(sender)}";
            if (sender == _bdChild)
                e.Handled = _cbHandled.IsOn;
        }

        void OnRoutedPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbRoute.Text += $"\r\nReleased, sender={GetName(sender)}";
            if (sender == _bdChild)
                e.Handled = _cbHandled.IsOn;
        }

        void OnRoutedTapped(object sender, TappedRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbRoute.Text += $"\r\nTapped, sender={GetName(sender)}";
            if (sender == _bdChild)
                e.Handled = _cbHandled.IsOn;
        }

        void OnRoutedDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbRoute.Text += $"\r\nDoubleTapped, sender={GetName(sender)}";
            if (sender == _bdChild)
                e.Handled = _cbHandled.IsOn;
        }
        #endregion

        #region AddHandler
        void OnHandler(object sender, PointerRoutedEventArgs e, string p_event)
        {
            _lastEventIsMoved = false;
            if (p_event != "Pressed")
                _tbAdd.Text += $"\r\n{p_event}, sender={GetName(sender)}";
            else
                _tbAdd.Text = $"{p_event}, sender={GetName(sender)}";
            if (sender == _bdAddChild)
                e.Handled = _cbAdd.IsOn;
        }

        void PointerEnteredHandler(object sender, PointerRoutedEventArgs e)
        {
            OnHandler(sender, e, "Entered");
        }

        void PointerExitedHandler(object sender, PointerRoutedEventArgs e)
        {
            OnHandler(sender, e, "Exited");
        }

        void PointerMovedHandler(object sender, PointerRoutedEventArgs e)
        {
            if (_lastEventIsMoved)
                return;

            _tbAdd.Text += $"\r\nMoved, sender={GetName(sender)}";
            _lastEventIsMoved = true;
        }

        void PointerPressedHandler(object sender, PointerRoutedEventArgs e)
        {
            OnHandler(sender, e, "Pressed");
        }

        void PointerReleasedHandler(object sender, PointerRoutedEventArgs e)
        {
            OnHandler(sender, e, "Released");
        }

        void TappedHandler(object sender, TappedRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbAdd.Text += $"\r\nTapped, sender={GetName(sender)}";
            if (sender == _cbAdd)
                e.Handled = _cbAdd.IsOn;
        }

        void DoubleTappedHandler(object sender, DoubleTappedRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbAdd.Text += $"\r\nDoubleTapped, sender={GetName(sender)}";
            if (sender == _cbAdd)
                e.Handled = _cbAdd.IsOn;
        }
        #endregion

        #region 单事件
        void OnSinglePointerEntered(object sender, PointerRoutedEventArgs args)
        {
            _lastEventIsMoved = false;
            _tbSingle.Text += " Entered";
        }

        void OnSinglePointerExited(object sender, PointerRoutedEventArgs args)
        {
            _lastEventIsMoved = false;
            _tbSingle.Text += " Exited";
        }

        void OnSinglePointerMoved(object sender, PointerRoutedEventArgs args)
        {
            if (_lastEventIsMoved)
                return;

            _tbSingle.Text += " Moved";
            _lastEventIsMoved = true;
        }

        void OnSinglePointerPressed(object sender, PointerRoutedEventArgs args)
        {
            _lastEventIsMoved = false;
            _tbSingle.Text = "Pressed";
        }

        void OnSinglePointerReleased(object sender, PointerRoutedEventArgs args)
        {
            _lastEventIsMoved = false;
            _tbSingle.Text += " Released";
        }

        void OnSingleTapped(object sender, TappedRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbSingle.Text += " Tapped";
        }

        void OnSingleRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbSingle.Text += " RightTapped";
        }

        void OnSingleDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbSingle.Text += " DoubleTapped";
        }
        #endregion

        void _bdHold_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
                _tbHold.Text = "Holding:" + DateTime.Now.ToString("ss");
        }

        static string GetName(object element)
        {
            if (element == null)
                return "<null>";
            if (element is FrameworkElement fe)
                return string.IsNullOrWhiteSpace(fe.Name) ? fe.ToString() : fe.Name;
            return element.ToString();
        }

        void OnTbKeyDown(object sender, KeyRoutedEventArgs e)
        {
            Kit.Msg(e.Key.ToString());
        }
    }
}