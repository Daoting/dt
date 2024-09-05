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
    /// Markdown富文本格
    /// </summary>
    public partial class CMarkdown : FvCell
    {
        #region 构造方法
        public CMarkdown()
        {
            DefaultStyleKey = typeof(CMarkdown);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 保存事件
        /// </summary>
        public event Action<CMarkdown> Saved;
        #endregion

        protected override void OnApplyCellTemplate()
        {
            var btn = (Button)GetTemplateChild("BtnEdit");
            btn.Click += OnShowDlg;
        }

        void OnShowDlg(object sender, RoutedEventArgs e)
        {
            var dlg = new MarkdownDlg(this);
            if (!Kit.IsPhoneUI)
            {
                dlg.Height = Kit.ViewHeight - 140;
                dlg.Width = Math.Min(900, Kit.ViewWidth - 200);
            }
            dlg.ShowDlg();
        }

        internal void OnSaved()
        {
            Saved?.Invoke(this);
        }

        internal string CurrentText
        {
            get
            {
                if (ValBinding.Source is ICell cell)
                    return cell.GetVal<string>();
                return "";
            }
        }

        internal Task<bool> SaveText(string p_txt)
        {
            Val = p_txt;
            OnSaved();
            return Task.FromResult(true);
        }
    }
}