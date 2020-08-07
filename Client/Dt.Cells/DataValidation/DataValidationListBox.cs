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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DataValidationListBox : ListBox
    {
        /// <summary>
        /// Indicates the command parameter dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", (Type) typeof(object), (Type) typeof(DataValidationListBox), new PropertyMetadata(null));
        /// <summary>
        /// Indicates a command dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", (Type) typeof(ICommand), (Type) typeof(DataValidationListBox), new PropertyMetadata(null));

        /// <summary>
        /// Creates a new instance of the <see cref="T:AutoFilterListBox" /> class.
        /// </summary>
        public DataValidationListBox()
        {
            base.DefaultStyleKey = typeof(DataValidationListBox);
            base.SelectedValuePath = "Value";
            DataValidationListBox box = this;
            box.SelectionChanged += DataValidationListBox_SelectionChanged;
        }

        /// <summary>
        /// Determines whether this the command can be executed on the control.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the command can executed; otherwise, <c>false</c>.
        /// </returns>
        public bool CanExecuteCommand()
        {
            return ((Command != null) && Command.CanExecute(CommandParameter));
        }

        internal void Close()
        {
            if (Popup != null)
            {
                Popup.Close();
                Popup = null;
            }
        }

        void DataValidationListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            base.ScrollIntoView(base.SelectedItem);
        }

        /// <summary>
        /// Executes the command on the control.
        /// </summary>
        public void ExecuteCommand()
        {
            if ((Command != null) && Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);
            }
        }

        /// <summary>
        /// Creates or identifies the element used to display a specified item.
        /// </summary>
        /// <returns>
        /// An  DataValidationListBoxItem.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DataValidationListBoxItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own ItemContainer.
        /// </summary>
        /// <param name="item">Specified item.</param>
        /// <returns>
        /// <c>true</c> if the item is its own ItemContainer; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is DataValidationListBoxItem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Size size = base.MeasureOverride(constraint);
            ProcessFocus();
            return size;
        }

        /// <summary>
        /// Responds to the <see cref="E:System.Windows.UIElement.KeyDown" /> event.
        /// </summary>
        /// <param name="e">Provides data for <see cref="T:System.Windows.Input.KeyEventArgs" />.</param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (((e.Key != VirtualKey.Up) || (base.SelectedIndex != 0)) && ((e.Key != VirtualKey.Down) || (base.SelectedIndex != (base.Items.Count - 1))))
            {
                if (e.Key == VirtualKey.Escape)
                {
                    Close();
                }
                else
                {
                    if (((e.Key == VirtualKey.Enter) || (e.Key == VirtualKey.Space)) && ((base.SelectedIndex >= 0) || ((base.SelectedIndex == -1) && (base.SelectedValue == null))))
                    {
                        PerformSelectionChanged();
                    }
                    base.OnKeyDown(e);
                }
            }
        }

        internal void PerformSelectionChanged()
        {
            Close();
            if ((Command != null) && CanExecuteCommand())
            {
                if (base.SelectedItem is DataValidationListItem)
                {
                    CommandParameter = (base.SelectedItem as DataValidationListItem).Value;
                }
                else
                {
                    CommandParameter = base.SelectedItem;
                }
                ExecuteCommand();
            }
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            DataValidationListBoxItem item2 = element as DataValidationListBoxItem;
            if (item2 != null)
            {
                item2.DataValidationListBox = this;
            }
        }

        void ProcessFocus()
        {
            if ((base.SelectedIndex < 0) && (base.Items.Count > 0))
            {
                base.SelectedIndex = 0;
            }
            if ((base.SelectedIndex >= 0) && (base.SelectedIndex < base.Items.Count))
            {
                ListBoxItem targetElement = base.ContainerFromIndex(base.SelectedIndex) as ListBoxItem;
                if (targetElement == null)
                {
                    base.ScrollIntoView(base.SelectedItem);
                    targetElement = base.ContainerFromIndex(base.SelectedIndex) as ListBoxItem;
                }
                if ((targetElement != null) && !ElementTreeHelper.IsKeyboardFocusWithin(targetElement))
                {
                    targetElement.Focus(FocusState.Programmatic);
                }
            }
        }

        void ProcessGotFocus(RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Gets or sets the command to the control.
        /// </summary>
        public ICommand Command
        {
            get { return  (ICommand) base.GetValue(CommandProperty); }
            set { base.SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command parameter.
        /// </summary>
        public object CommandParameter
        {
            get { return  base.GetValue(CommandParameterProperty); }
            set { base.SetValue(CommandParameterProperty, value); }
        }

        internal PopupHelper Popup { get; set; }
    }
}

