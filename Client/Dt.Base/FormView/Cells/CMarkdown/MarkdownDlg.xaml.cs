#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.Base.FormView
{
    public sealed partial class MarkdownDlg : Dlg
    {
        CMarkdown _owner;
        bool _saved;

        public MarkdownDlg(CMarkdown p_owner)
        {
            InitializeComponent();
            _owner = p_owner;
        }

        public void ShowDlg()
        {
            if (!Kit.IsPhoneUI)
            {
                Height = Kit.ViewHeight - 140;
                Width = Math.Min(900, Kit.ViewWidth - 200);
                ShowVeil = true;
            }
            
            if (_owner.ReadOnlyBinding)
            {
                Menu.Visibility = Visibility.Collapsed;
                _box.IsReadOnly = true;
            }
            else
            {
                Menu.Visibility = Visibility.Visible;
                _box.IsReadOnly = false;
            }
            Show();
            _box.SetText(_owner.CurrentText);
        }

        protected override void OnClosed(bool p_result)
        {
            _box.Close();
        }
        
        async void OnSave(Mi e)
        {
            var html = await _box.GetText();
            bool suc = await _owner.SaveText(html);
            if (suc)
            {
                _saved = true;
                Close(true);
            }
        }
        
        protected override async Task<bool> OnClosing(bool p_result)
        {
            if (!_saved)
            {
                var html = await _box.GetText();
                var cur = _owner.CurrentText.Replace("\r\n", "\n").Replace("\r", "\n");
                if (cur == html)
                    return true;

                return await Kit.Confirm("关闭将丢失已修改的内容，确认要关闭？");
            }
            return true;
        }
    }
}