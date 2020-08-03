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
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents an edit control.
    /// </summary>
    public partial class EditingElement : TextBox
    {
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(
            "Status",
            typeof(EditorStatus),
            typeof(EditingElement),
            new PropertyMetadata(EditorStatus.Ready));

        public EditingElement()
        {
            BorderThickness = new Thickness(0.0);
            Padding = new Thickness(0.0, 4, 0, 4);
            Background = new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        /// Represents current editor status.
        /// </summary>
        public EditorStatus Status
        {
            get { return (EditorStatus)base.GetValue(StatusProperty); }
            internal set { base.SetValue(StatusProperty, value); }
        }
    }
}

