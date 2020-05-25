#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-05-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Dt.Core;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 富文本格
    /// </summary>
    public partial class CHtml : FvCell
    {
        #region 构造方法
        public CHtml()
        {
            DefaultStyleKey = typeof(CHtml);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 保存事件
        /// </summary>
        public event EventHandler Saved;
        #endregion

        protected override void OnApplyCellTemplate()
        {
            var btn = (Button)GetTemplateChild("BtnEdit");
            btn.Click += OnShowDlg;
        }

        void OnShowDlg(object sender, RoutedEventArgs e)
        {
            if (ReadOnlyBinding)
            {
                var dlg = new HtmlViewDlg();
                if (!AtSys.IsPhoneUI)
                {
                    dlg.Height = SysVisual.ViewHeight - 140;
                    dlg.Width = Math.Min(800, SysVisual.ViewWidth - 200);
                }
                dlg.ShowDlg(this);
            }
            else
            {
                var dlg = new HtmlEditDlg();
                if (!AtSys.IsPhoneUI)
                {
                    dlg.ShowWinVeil = true;
                    dlg.Height = SysVisual.ViewHeight - 140;
                    dlg.Width = Math.Min(900, SysVisual.ViewWidth - 200);
                }
                dlg.ShowDlg(this);
            }
        }

        internal void OnSaved()
        {
            Saved?.Invoke(this, EventArgs.Empty);
        }
    }
}