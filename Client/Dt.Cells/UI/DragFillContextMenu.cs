#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a drag fill context menu control used to apply the automatic fill type.
    /// </summary>
    public partial class DragFillContextMenu : ContentControl
    {
        AutoFillType[] _items;
        StackPanel _rootGrid;
        DragFillContextMenuItem _selectedItem;
        AutoFillType _selectedType;

        /// <summary>
        /// Occurs when the selected automatic fit item has changed.
        /// </summary>
        public event EventHandler SelectedAutoFitTypeChanged;

        /// <summary>
        /// Creates a new instance of the <see cref="T:DragFillContextMenu" /> class.
        /// </summary>
        /// <param name="items">The automatic fill items.</param>
        /// <param name="defaultType">The default checked automatic fill item.</param>
        public DragFillContextMenu(AutoFillType[] items, AutoFillType defaultType)
        {
            base.DefaultStyleKey = typeof(DragFillContextMenu);
            _rootGrid = new StackPanel();
            base.Content = _rootGrid;
            DragFillContextMenu menu = this;
            menu.Loaded += DragFillContextMenu_Loaded; _selectedType = defaultType;
            Items = items;
            DragFillContextMenu menu2 = this;
            menu2.PointerPressed += OnDragFillContextMenuPointerPressed;
        }

        /// <summary>
        /// XamlTyp用，hdt
        /// </summary>
        public DragFillContextMenu()
        { }

        void CheckedItemChenged(object sender, EventArgs e)
        {
            foreach (DragFillContextMenuItem item in _rootGrid.Children)
            {
                if (item != sender)
                {
                    item.IsChecked = false;
                }
                else
                {
                    _selectedItem = item;
                }
            }
            OnSelectedItemChanged();
        }

        void DragFillContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
        }

        static string GetAutoFitTypeString(AutoFillType type)
        {
            switch (type)
            {
                case AutoFillType.CopyCells:
                    return ResourceStrings.UIFill_CopyCells;

                case AutoFillType.FillSeries:
                    return ResourceStrings.UIFill_FillSeries;

                case AutoFillType.FillFormattingOnly:
                    return ResourceStrings.UIFill_FillFormattingOnly;

                case AutoFillType.FillWithoutFormatting:
                    return ResourceStrings.UIFill_FillWithOutFormatting;
            }
            return ResourceStrings.UIFill_None;
        }

        void OnDragFillContextMenuPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
        }

        void OnSelectedItemChanged()
        {
            if (SelectedAutoFitTypeChanged != null)
            {
                SelectedAutoFitTypeChanged(this, EventArgs.Empty);
            }
        }

        void UpdateItems()
        {
            foreach (DragFillContextMenuItem item in _rootGrid.Children)
            {
                item.Checked -= new EventHandler(CheckedItemChenged);
            }
            _rootGrid.Children.Clear();
            foreach (AutoFillType type in Items)
            {
                DragFillContextMenuItem item2 = new DragFillContextMenuItem(GetAutoFitTypeString(type), type);
                _rootGrid.Children.Add(item2);
                item2.Checked += new EventHandler(CheckedItemChenged);
            }
            UpdateSelectedItem();
        }

        void UpdateSelectedItem()
        {
            string autoFitTypeString = "";
            foreach (AutoFillType type in Items)
            {
                if (type == _selectedType)
                {
                    autoFitTypeString = GetAutoFitTypeString(type);
                    break;
                }
            }
            if (string.IsNullOrEmpty(autoFitTypeString))
            {
                autoFitTypeString = GetAutoFitTypeString(Items[0]);
            }
            foreach (DragFillContextMenuItem item in _rootGrid.Children)
            {
                if (item.Text == autoFitTypeString)
                {
                    item.IsChecked = true;
                    _selectedItem = item;
                }
                else
                {
                    item.IsChecked = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the automatic fill items.
        /// </summary>
        /// <value>
        /// The automatic fill items.
        /// </value>
        public AutoFillType[] Items
        {
            get { return _items; }
            set
            {
                _items = value;
                UpdateItems();
            }
        }

        /// <summary>
        /// Gets or sets the selected automatic fill item.
        /// </summary>
        /// <value>
        /// The selected automatic fill item.
        /// </value>
        public AutoFillType SelectedAutoFitType
        {
            get { return _selectedItem.FillType; }
            set
            {
                _selectedType = value;
                UpdateSelectedItem();
            }
        }
    }
}

