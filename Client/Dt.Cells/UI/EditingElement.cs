#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents an edit control.
    /// </summary>
    public partial class EditingElement : TextBox
    {
        internal CellPresenterBase Owner;
        private ScrollViewer scrollView;
        /// <summary>
        /// Represents current editor status.
        /// </summary>
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", (Type) typeof(EditorStatus), (Type) typeof(EditingElement), new PropertyMetadata(EditorStatus.Ready));

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public EditingElement()
        {
            base.DefaultStyleKey = typeof(EditingElement);
            base.Padding = new Windows.UI.Xaml.Thickness(0.0);
            this.Status = EditorStatus.Ready;
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate. In simplest terms, this means the method is called just before a UI element displays in your app. Override this method to influence the default post-template logic of a class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.scrollView = base.GetTemplateChild("ContentElement") as ScrollViewer;
        }

        /// <summary>
        /// The F2 key toggles between Edit and Enter status.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);
            bool alt = false;
            bool ctrl = false;
            bool shift = false;
            KeyboardHelper.GetMetaKeyState(out shift, out ctrl, out alt);
            if ((!alt && !ctrl) && (!shift && (e.Key == VirtualKey.F2)))
            {
                if (this.Status == EditorStatus.Edit)
                {
                    this.Status = EditorStatus.Enter;
                }
                else
                {
                    this.Status = EditorStatus.Edit;
                }
            }
        }

        /// <summary>
        /// Represents current editor status.
        /// </summary>
        public EditorStatus Status
        {
            get { return  (EditorStatus) base.GetValue(StatusProperty); }
            internal set { base.SetValue(StatusProperty, value); }
        }

        internal ScrollViewer TextView
        {
            get { return  this.scrollView; }
        }
    }
}

