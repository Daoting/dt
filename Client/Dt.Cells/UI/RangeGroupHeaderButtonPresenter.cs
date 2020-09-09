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
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a <see cref="T:GrapeCity.Windows.SpreadSheet.UI.GcSpreadSheet" /> range group header button
    /// that is used to expand or collapse all the groups in the same level.
    /// </summary>
    public partial class GroupHeaderButton : Button
    {
        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register(
            "Level",
            typeof(string),
            typeof(GroupHeaderButton),
            new PropertyMetadata("0"));

        /// <summary>
        /// Creates a new instance of the control.
        /// </summary>
        public GroupHeaderButton()
        {
            DefaultStyleKey = typeof(GroupHeaderButton);
        }

        /// <summary>
        /// Gets or sets a value that indicates the range group level.
        /// </summary>
        public string Level
        {
            get { return (string)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }
    }
}

