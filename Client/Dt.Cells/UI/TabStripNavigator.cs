#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a <see cref="T:GrapeCity.Windows.SpreadSheet.UI.GcSpreadSheet" /> tab strip navigation control.
    /// </summary>
    public partial class TabStripNavigator : Control
    {
        string[] buttonNames = new string[] { "First", "Previous", "Next", "Last" };

        internal event EventHandler<NavigationButtonClickEventArgs> ButtonClick;

        /// <summary>
        /// Create a new instance of the class.
        /// </summary>
        public TabStripNavigator()
        {
            base.DefaultStyleKey = typeof(TabStripNavigator);
        }

        /// <summary>
        /// Is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate()" />, when overridden in a derived class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            foreach (string str in buttonNames)
            {
                ButtonBase templateChild = base.GetTemplateChild(str) as ButtonBase;
                if (templateChild != null)
                {
                    templateChild.IsTabStop = false;
                    templateChild.Click += OnButtonClick;
                }
            }
        }

        void OnButtonClick(object sender, RoutedEventArgs e)
        {
            string name = ((ButtonBase)sender).Name;
            if ("First".Equals(name))
            {
                RaiseButtonClick(new NavigationButtonClickEventArgs(ButtonType.First));
            }
            else if ("Previous".Equals(name))
            {
                RaiseButtonClick(new NavigationButtonClickEventArgs(ButtonType.Previous));
            }
            else if ("Next".Equals(name))
            {
                RaiseButtonClick(new NavigationButtonClickEventArgs(ButtonType.Next));
            }
            else if ("Last".Equals(name))
            {
                RaiseButtonClick(new NavigationButtonClickEventArgs(ButtonType.Last));
            }
        }

        void RaiseButtonClick(NavigationButtonClickEventArgs e)
        {
            if (ButtonClick != null)
            {
                ButtonClick(this, e);
            }
        }
    }
}

