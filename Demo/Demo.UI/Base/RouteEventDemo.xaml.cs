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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Input;
#endregion

namespace Demo.UI
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
            _bdParent.RightTapped += OnRoutedRightTapped;
            _bdParent.DoubleTapped += OnRoutedDoubleTapped;

            _bdChild.PointerEntered += OnRoutedPointerEntered;
            _bdChild.PointerExited += OnRoutedPointerExited;
            _bdChild.PointerMoved += OnRoutedPointerMoved;
            _bdChild.PointerPressed += OnRoutedPointerPressed;
            _bdChild.PointerReleased += OnRoutedPointerReleased;
            _bdChild.Tapped += OnRoutedTapped;
            _bdChild.RightTapped += OnRoutedRightTapped;
            _bdChild.DoubleTapped += OnRoutedDoubleTapped;

            _bdAddParent.AddHandler(PointerEnteredEvent, new PointerEventHandler(PointerEnteredHandler), true);
            _bdAddParent.AddHandler(PointerExitedEvent, new PointerEventHandler(PointerExitedHandler), true);
            _bdAddParent.AddHandler(PointerMovedEvent, new PointerEventHandler(PointerMovedHandler), true);
            _bdAddParent.AddHandler(PointerPressedEvent, new PointerEventHandler(PointerPressedHandler), true);
            _bdAddParent.AddHandler(PointerReleasedEvent, new PointerEventHandler(PointerReleasedHandler), true);
            _bdAddParent.AddHandler(TappedEvent, new TappedEventHandler(TappedHandler), true);
            _bdAddParent.AddHandler(RightTappedEvent, new RightTappedEventHandler(RightTappedHandler), true);
            _bdAddParent.AddHandler(DoubleTappedEvent, new DoubleTappedEventHandler(DoubleTappedHandler), true);

            _bdAddChild.PointerEntered += PointerEnteredHandler;
            _bdAddChild.PointerExited += PointerExitedHandler;
            _bdAddChild.PointerMoved += PointerMovedHandler;
            _bdAddChild.PointerPressed += PointerPressedHandler;
            _bdAddChild.PointerReleased += PointerReleasedHandler;
            _bdAddChild.Tapped += TappedHandler;
            _bdAddChild.RightTapped += RightTappedHandler;
            _bdAddChild.DoubleTapped += DoubleTappedHandler;

            _bdHold.Holding += _bdHold_Holding;
        }

        #region 父子事件路由
        void OnRoutedPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbRoute.Text += $"\r\nEntered{GetName(sender)}";
            if (sender == _bdChild)
                e.Handled = _cbHandled.IsChecked;
        }

        void OnRoutedPointerExited(object sender, PointerRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbRoute.Text += $"\r\nExited{GetName(sender)}";
            if (sender == _bdChild)
                e.Handled = _cbHandled.IsChecked;
        }

        void OnRoutedPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_lastEventIsMoved)
                return;

            _tbRoute.Text += $"\r\nMoved{GetName(sender)}";
            if (sender == _bdChild)
                e.Handled = _cbHandled.IsChecked;
            _lastEventIsMoved = true;
        }

        void OnRoutedPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbRoute.Text += $"\r\nPressed{GetName(sender)}";
            if (sender == _bdChild)
                e.Handled = _cbHandled.IsChecked;
        }

        void OnRoutedPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbRoute.Text += $"\r\nReleased{GetName(sender)}";
            if (sender == _bdChild)
                e.Handled = _cbHandled.IsChecked;
        }

        void OnRoutedTapped(object sender, TappedRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbRoute.Text += $"\r\nTapped{GetName(sender)}";
            if (sender == _bdChild)
                e.Handled = _cbHandled.IsChecked;
        }

        void OnRoutedRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbRoute.Text += $"\r\nRightTapped{GetName(sender)}";
            if (sender == _bdChild)
                e.Handled = _cbHandled.IsChecked;
        }

        void OnRoutedDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbRoute.Text += $"\r\nDoubleTapped{GetName(sender)}";
            if (sender == _bdChild)
                e.Handled = _cbHandled.IsChecked;
        }
        #endregion

        #region AddHandler
        void OnHandler(object sender, PointerRoutedEventArgs e, string p_event)
        {
            _lastEventIsMoved = false;
            _tbAdd.Text += $"\r\n{p_event}{GetName(sender)}";
            if (sender == _bdAddChild)
                e.Handled = _cbAdd.IsChecked;
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

            _tbAdd.Text += $"\r\nMoved{GetName(sender)}";
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
            _tbAdd.Text += $"\r\nTapped{GetName(sender)}";
            if (sender == _cbAdd)
                e.Handled = _cbAdd.IsChecked;
        }

        void RightTappedHandler(object sender, RightTappedRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbAdd.Text += $"\r\nRightTapped{GetName(sender)}";
            if (sender == _cbAdd)
                e.Handled = _cbAdd.IsChecked;
        }

        void DoubleTappedHandler(object sender, DoubleTappedRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbAdd.Text += $"\r\nDoubleTapped{GetName(sender)}";
            if (sender == _cbAdd)
                e.Handled = _cbAdd.IsChecked;
        }
        #endregion

        #region 单事件
        void OnSinglePointerEntered(object sender, PointerRoutedEventArgs args)
        {
            _lastEventIsMoved = false;
            _tbSingle.Text += "\r\nEntered";
        }

        void OnSinglePointerExited(object sender, PointerRoutedEventArgs args)
        {
            _lastEventIsMoved = false;
            _tbSingle.Text += "\r\nExited";
        }

        void OnSinglePointerMoved(object sender, PointerRoutedEventArgs args)
        {
            if (_lastEventIsMoved)
                return;

            _tbSingle.Text += "\r\nMoved";
            _lastEventIsMoved = true;
        }

        void OnSinglePointerPressed(object sender, PointerRoutedEventArgs args)
        {
            _lastEventIsMoved = false;
            _tbSingle.Text += "\r\nPressed";
        }

        void OnSinglePointerReleased(object sender, PointerRoutedEventArgs args)
        {
            _lastEventIsMoved = false;
            _tbSingle.Text += "\r\nReleased";
        }

        void OnSingleTapped(object sender, TappedRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbSingle.Text += "\r\nTapped";
        }

        void OnSingleRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbSingle.Text += "\r\nRightTapped";
        }

        void OnSingleDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            _lastEventIsMoved = false;
            _tbSingle.Text += "\r\nDoubleTapped";
        }
        #endregion

        void _bdHold_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == HoldingState.Started)
                _tbHold.Text += "\r\nHolding:" + DateTime.Now.ToString("mi:ss");
        }

        static string GetName(object element)
        {
            if (element is FrameworkElement fe
                && !string.IsNullOrWhiteSpace(fe.Name))
            {
                return fe.Name.EndsWith("Child") ? "(Child)" : "(Parent)";
            }
            return "(null)";
        }

        void OnClear(Mi e)
        {
            if (_tbSingle.Text != "")
                Log.Debug(_tbSingle.Text);
            if (_tbRoute.Text != "")
                Log.Debug(_tbRoute.Text);
            if (_tbAdd.Text != "")
                Log.Debug(_tbAdd.Text);

            _tbSingle.Text = "";
            _tbRoute.Text = "";
            _tbAdd.Text = "";
            _tbHold.Text = "";
        }
    }
}