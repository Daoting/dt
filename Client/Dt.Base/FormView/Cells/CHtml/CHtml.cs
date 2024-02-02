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
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 富文本格
    /// </summary>
    public partial class CHtml : FvCell, IHtmlEditHost
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
        public event Action<CHtml> Saved;
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
                if (!Kit.IsPhoneUI)
                {
                    dlg.Height = Kit.ViewHeight - 140;
                    dlg.Width = Math.Min(800, Kit.ViewWidth - 200);
                }
                dlg.ShowDlg(this);
            }
            else
            {
                var dlg = new HtmlEditDlg();
                if (!Kit.IsPhoneUI)
                {
                    dlg.Height = Kit.ViewHeight - 140;
                    dlg.Width = Math.Min(900, Kit.ViewWidth - 200);
                }
                dlg.ShowDlg(this);
            }
        }

        internal void OnSaved()
        {
            Saved?.Invoke(this);
        }

        #region IHtmlEditHost
        string IHtmlEditHost.CurrentHtml
        {
            get
            {
                if (ValBinding.Source is ICell cell)
                    return cell.GetVal<string>();
                return "";
            }
        }

        Task<bool> IHtmlEditHost.SaveHtml(string p_html)
        {
            Val = p_html;
            OnSaved();
            return Task.FromResult(true);
        }
        #endregion
    }
}