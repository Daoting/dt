#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using System.Reflection;
using System.Text;
using Windows.System;
#endregion

namespace Dt.Base
{
    public partial class PickDlg : Dlg
    {
        CPick _owner;
        TextBox _tb;
        Border _tbBorder;

        public PickDlg(CPick p_owner)
        {
            InitializeComponent();

            _owner = p_owner;
            InitStyle();
            LoadSearchBox();
            InitLv();
            Loaded += OnDlgLoaded;
        }

        void OnDlgLoaded(object sender, RoutedEventArgs e)
        {
            if (!Kit.IsPhoneUI)
            {
                if (_owner.Lv.CurrentViewMode != ViewMode.Table)
                    Width = _owner.CurrentWidth;
                _tbBorder.Width = _owner.CurrentWidth - 2;
            }
            
            bool suc = _tb.Focus(FocusState.Programmatic);
            _tb.SelectAll();
        }
        
        protected override void OnClosed(bool p_result)
        {
            base.OnClosed(p_result);
            RemoveLv();
        }
        
        void OnSelectOK(Mi e)
        {
            _owner.OnPicking();
        }

        void OnItemDoubleClick(object e)
        {
            _owner.OnPicking();
        }

        void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            var lv = _owner.Lv;
            switch (e.Key)
            {
                case VirtualKey.Enter:
                    if (_tb.Text.Trim() != "")
                    {
                        if (lv.Visibility == Visibility.Visible
                            && lv.SelectionMode == SelectionMode.Single
                            && lv.SelectedItem != null)
                        {
                            // 选择
                            _owner.OnPicking();
                        }
                        else
                        {
                            // 查询
                            LoadLv();
                            _owner.OnSearch(_tb.Text.Trim());
                            bool suc = _tb.Focus(FocusState.Programmatic);
                        }
                    }
                    break;

                case VirtualKey.Up:
                    if (lv.Visibility == Visibility.Visible
                        && lv.SelectionMode == SelectionMode.Single
                        && lv.Data != null
                        && lv.Data.Count > 0)
                    {
                        int index = lv.SelectedIndex;
                        if (index == -1)
                            lv.SelectedIndex = 0;
                        else if (index > 0)
                            lv.SelectedIndex = index - 1;
                        lv.ScrollInto(lv.SelectedIndex);
                    }
                    break;

                case VirtualKey.Down:
                    if (lv.Visibility == Visibility.Visible
                        && lv.SelectionMode == SelectionMode.Single
                        && lv.Data != null
                        && lv.Data.Count > 0)
                    {
                        int index = lv.SelectedIndex;
                        if (index == -1)
                            lv.SelectedIndex = 0;
                        else if (index < lv.Data.Count - 1)
                            lv.SelectedIndex = index + 1;
                        lv.ScrollInto(lv.SelectedIndex);
                    }
                    break;

                default:
                    // 按其它键清除选择，以便再次查询
                    lv.SelectedIndex = -1;
                    break;
            }
        }

        void InitStyle()
        {
            HideTitleBar = true;
            if (Kit.IsPhoneUI)
            {
                // 全面屏底部易误点
                _grid.Margin = new Thickness(0, 0, 0, 40);
            }
            else
            {
                BorderThickness = new Thickness();
                Background = Res.TransparentBrush;
                // 不向下层对话框传递Press事件
                AllowRelayPress = false;
            }
        }
        
        void LoadSearchBox()
        {
            _tb = new TextBox();
            _tb.SetBinding(TextBox.PlaceholderTextProperty, new Binding { Path = new PropertyPath("Placeholder"), Source = _owner, Mode = BindingMode.OneWay });

            // win上KeyUp事件有怪异：Tab跳两格、CList选择后跳两格
            // 手机上KeyDown事件不触发！！！
#if WIN || DOTNET
            _tb.KeyDown += OnKeyDown;
#else
            _tb.KeyUp += OnKeyDown;
#endif

            if (Kit.IsPhoneUI)
            {
                Grid grid = new Grid
                {
                    Background = Res.主蓝,
                    Height = 50,
                    Margin = new Thickness(0, 0, 0, 10),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    },
                };
                var btn = new Button
                {
                    Content = "\uE010",
                    Style = Res.浅字符按钮,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Width = 50,
                };
                btn.Click += (s, e) => Close();
                grid.Children.Add(btn);

                _tb.BorderThickness = new Thickness(0);
                _tb.Margin = new Thickness(0, 5, 10, 5);
                Grid.SetColumn(_tb, 1);
                grid.Children.Add(_tb);

                _grid.Children.Add(grid);
            }
            else
            {
                _tbBorder = new Border { Background = Res.默认背景, BorderThickness = new Thickness(), HorizontalAlignment = HorizontalAlignment.Left };
                _tb.BorderThickness = new Thickness(0);
                _tbBorder.Child = _tb;
                _grid.Children.Add(_tbBorder);
            }
        }

        void InitLv()
        {
            Lv lv = _owner.Lv;
            lv.Background = Res.默认背景;
            if (!Kit.IsPhoneUI)
            {
                lv.BorderThickness = new Thickness(1);
                lv.BorderBrush = Res.浅灰2;
                // 抢焦点
                lv.GotFocus += (s, e) => _tb.Focus(FocusState.Programmatic);
            }

            Mi mi = new Mi { ID = "确定", Icon = Icons.正确 };
            mi.Click += OnSelectOK;
            lv.Toolbar.Items.Add(mi);

            if (lv.SelectionMode == SelectionMode.Single)
            {
                lv.ItemDoubleClick += OnItemDoubleClick;
            }

            Grid.SetRow(lv, 1);
        }

        void LoadLv()
        {
            var lv = _owner.Lv;
            if (!_grid.Children.Contains(lv))
                _grid.Children.Add(lv);
        }

        void RemoveLv()
        {
            var lv = _owner.Lv;
            if (_grid.Children.Contains(lv))
                _grid.Children.Remove(lv);
            lv.Data = null;
        }
    }
}
