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
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a filter dropdown dialog's item base control. 
    /// </summary>
    public abstract partial class DropDownItemBaseControl : ContentControl
    {
        private bool _isSelected;
        /// <summary>
        /// Indicates the command parameter dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", (Type)typeof(object), (Type)typeof(DropDownItemBaseControl), new PropertyMetadata(null, new PropertyChangedCallback(DropDownItemBaseControl.OnCommandParameterChanged)));
        /// <summary>
        /// Indicates a command dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", (Type)typeof(ICommand), (Type)typeof(DropDownItemBaseControl), new PropertyMetadata(null, new PropertyChangedCallback(DropDownItemBaseControl.OnCommandChanged)));
        /// <summary>
        /// Indicates a show icon dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register("ShowIcon", (Type)typeof(Visibility), (Type)typeof(DropDownItemBaseControl), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(DropDownItemBaseControl.OnShowIconChangedChunk)));

        protected DropDownItemBaseControl()
        {
        }

        /// <summary>
        /// Determines whether this the command can be executed on the control.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the command can executed; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanExecuteCommand()
        {
            return ((this.Command != null) && this.Command.CanExecute(this.CommandParameter));
        }

        /// <summary>
        /// Executes the command on the control.
        /// </summary>
        public virtual void ExecuteCommand()
        {
            if ((this.Command != null) && this.Command.CanExecute(this.CommandParameter))
            {
                this.Command.Execute(this.CommandParameter);
            }
        }

        private void HandleCanExecuteChanged(object sender, EventArgs e)
        {
            this.UpdateIsEnabled();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.UpdateVisualState(true);
        }

        private void OnCommandChanged(ICommand oldValue, ICommand newValue)
        {
            if (oldValue != null)
            {
                oldValue.CanExecuteChanged -= HandleCanExecuteChanged;
            }
            if (newValue != null)
            {
                newValue.CanExecuteChanged += HandleCanExecuteChanged;
            }
            this.UpdateIsEnabled();
        }

        private static void OnCommandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((DropDownItemBaseControl)o).OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        private void OnCommandParameterChanged(object oldValue, object newValue)
        {
            this.UpdateIsEnabled();
        }

        private static void OnCommandParameterChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((DropDownItemBaseControl)o).OnCommandParameterChanged(e.OldValue, e.NewValue);
        }

        internal virtual void OnIsSelectedChanged()
        {
        }

        internal virtual void OnShowIconChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnShowIconChangedChunk(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as DropDownItemBaseControl).OnShowIconChanged(e);
        }

        internal virtual void SelectChild(bool forward)
        {
        }

        private void UpdateIsEnabled()
        {
            this.UpdateVisualState(true);
        }

        /// <summary>
        /// Updates the state of the control.
        /// </summary>
        /// <param name="useTransitions">if set to <c>true</c> to transition between states; otherwise <c>false</c>.</param>
        protected virtual void UpdateVisualState(bool useTransitions)
        {
        }

        internal virtual bool CanSelect
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the command to the control.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)base.GetValue(CommandProperty); }
            set { base.SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command parameter.
        /// </summary>
        public object CommandParameter
        {
            get { return base.GetValue(CommandParameterProperty); }
            set { base.SetValue(CommandParameterProperty, value); }
        }

        internal bool IsSelected
        {
            get
            {
                if (!this.CanSelect)
                {
                    return false;
                }
                return this._isSelected;
            }
            set
            {
                if (this._isSelected != value)
                {
                    this._isSelected = value;
                    this.OnIsSelectedChanged();
                }
            }
        }

        internal ColumnDropDownList ParentDropDownList { get; set; }

        /// <summary>
        /// Gets or sets the show icon image on the item.
        /// </summary>
        public Visibility ShowIcon
        {
            get { return (Visibility)base.GetValue(ShowIconProperty); }
            set { base.SetValue(ShowIconProperty, value); }
        }
    }
}

