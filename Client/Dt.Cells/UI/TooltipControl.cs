#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a control that creates a pop-up window that displays information
    /// for an element in the UI.
    /// </summary>
    public partial class TooltipControl : Control
    {
        /// <summary>
        /// Identifies the <see cref="P:TooltipControl.Text" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="P:TooltipControl.Text" /> dependency property.
        /// </value>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", (Type) typeof(string), (Type) typeof(TooltipControl), new PropertyMetadata(""));

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TooltipControl" /> class.
        /// </summary>
        public TooltipControl()
        {
            base.DefaultStyleKey = typeof(TooltipControl);
        }

        /// <summary>
        /// Gets or sets the text that displayed on the tool-tip.
        /// </summary>
        /// <value>
        /// The text displayed on the tool-tip.
        /// </value>
        public string Text
        {
            get { return  (string) ((string) base.GetValue(TextProperty)); }
            set { base.SetValue(TextProperty, value); }
        }
    }
}

