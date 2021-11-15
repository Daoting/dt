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
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a filter dropdown dialog control.
    /// </summary>
    public partial class ColumnDropDownList : ItemsControl
    {
        int _selectedItemIndex = -1;
        /// <summary>
        /// Indicates the show icon bar dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowIconBarProperty = DependencyProperty.Register("ShowIconBar", (Type)typeof(Visibility), (Type)typeof(ColumnDropDownList), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Creates a new instance of the <see cref="T:ColumnDropDownList" /> class.
        /// </summary>
        public ColumnDropDownList()
        {
            base.DefaultStyleKey = typeof(ColumnDropDownList);
        }

        internal void Close()
        {
            if (Popup != null)
            {
                Popup.Close();
                Popup = null;
            }
        }

        internal void DeselectItem(DropDownItemControl item)
        {
            if (item != null)
            {
                item.IsSelected = false;
            }
            SelectedItemIndex = -1;
        }

        /// <summary>
        /// Invoked when the <see cref="E:System.Windows.UIElement.KeyDown" /> event is received.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (!e.Handled)
            {
                switch (e.Key)
                {
                    case VirtualKey.Left:
                    case VirtualKey.Up:
                        goto Label_0046;

                    case VirtualKey.Right:
                    case VirtualKey.Down:
                        goto Label_0056;

                    case VirtualKey.Escape:
                        Close();
                        e.Handled = true;
                        break;

                    case VirtualKey.Tab:
                        if (CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift) == CoreVirtualKeyStates.None)
                        {
                            goto Label_0056;
                        }
                        goto Label_0046;

                    case VirtualKey.Enter:
                        if (SelectedItemIndex >= 0)
                        {
                            DropDownItemBaseControl control = base.ContainerFromIndex(SelectedItemIndex) as DropDownItemBaseControl;
                            if (control != null)
                            {
                                if (!control.CanExecuteCommand())
                                {
                                    break;
                                }
                                control.ExecuteCommand();
                            }
                        }
                        Close();
                        e.Handled = true;
                        break;
                }
            }
            goto Label_00C3;
        Label_0046:
            SelectNextItem(false);
            e.Handled = true;
            goto Label_00C3;
        Label_0056:
            SelectNextItem(true);
            e.Handled = true;
        Label_00C3:
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            DropDownItemBaseControl control = item as DropDownItemBaseControl;
            if (control != null)
            {
                control.ParentDropDownList = this;
            }
        }

        internal void SelectItem(DropDownItemControl item)
        {
            int index = base.IndexFromContainer(item);
            if (index >= 0)
            {
                SelectItem(index, true);
            }
        }

        bool SelectItem(int index, bool forward)
        {
            DropDownItemBaseControl control = base.ContainerFromIndex(index) as DropDownItemBaseControl;
            if ((control == null) || !control.CanSelect)
            {
                return false;
            }
            if (SelectedItemIndex >= 0)
            {
                DropDownItemBaseControl control2 = base.ContainerFromIndex(SelectedItemIndex) as DropDownItemBaseControl;
                if (control2 != null)
                {
                    control2.IsSelected = false;
                }
            }
            control.IsSelected = true;
            control.SelectChild(forward);
            SelectedItemIndex = index;
            return true;
        }

        void SelectNextItem(bool forward)
        {
            int selectedItemIndex = forward ? -1 : base.Items.Count;
            if (SelectedItemIndex >= 0)
            {
                selectedItemIndex = SelectedItemIndex;
            }
            int index = selectedItemIndex;
            do
            {
                index = ((index + base.Items.Count) + (forward ? 1 : -1)) % base.Items.Count;
            }
            while (!SelectItem(index, forward) && (index != selectedItemIndex));
        }

        internal PopupHelper Popup { get; set; }

        internal int SelectedItemIndex
        {
            get { return _selectedItemIndex; }
            set
            {
                if (_selectedItemIndex != value)
                {
                    _selectedItemIndex = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value to show icon bar.
        /// </summary>
        public Visibility ShowIconBar
        {
            get { return (Visibility)base.GetValue(ShowIconBarProperty); }
            set { base.SetValue(ShowIconBarProperty, value); }
        }
    }
}

