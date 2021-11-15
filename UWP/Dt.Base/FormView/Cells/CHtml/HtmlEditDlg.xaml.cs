#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.Base.FormView
{
    public sealed partial class HtmlEditDlg : Dlg
    {
        const string _insertVideo = "<video src=\"../../../fsm/{0}\" poster=\"../../../fsm/{1}\" preload=\"none\" width=\"640\" height=\"360\" controls=\"controls\"></video>";
        const string _insertImg = "../../../fsm/{0}";
        IHtmlEditHost _host;
        bool _saved;

        public HtmlEditDlg()
        {
            InitializeComponent();

            _wv.Source = new Uri($"{Kit.Stub.ServerUrl}/pub/editor/html/default.html");
            if (Type.GetType(FileItem.SelectFileDlgType) == null)
            {
                _menu.Hide("图片", "视频");
            }
        }

        public async void ShowDlg(IHtmlEditHost p_host)
        {
            _host = p_host;
            if (!Kit.IsPhoneUI)
            {
                Height = Kit.ViewHeight - 140;
                Width = Math.Min(900, Kit.ViewWidth - 200);
                ShowVeil = true;
            }
            Show();

            if (!string.IsNullOrEmpty(_host.CurrentHtml))
            {
                // WebView事件无法捕捉到初始化html的时机，延时
                await Task.Delay(500);
                await _wv.InvokeScriptAsync("setHtml", new string[] { _host.CurrentHtml });
            }
        }

        async void OnSave(object sender, Mi e)
        {
            var html = await _wv.InvokeScriptAsync("getHtml", null);
            bool suc = await _host.SaveHtml(html);
            if (suc)
            {
                _saved = true;
                Close(true);
            }
        }

        async void OnInsertImg(object sender, Mi e)
        {
            var dlg = (ISelectFileDlg)Activator.CreateInstance(Type.GetType(FileItem.SelectFileDlgType));
            if (await dlg.Show(true, FileItem.ImageExt))
            {
                foreach (var file in dlg.SelectedFiles)
                {
                    int index = file.IndexOf("\",");
                    if (index > 2)
                        await _wv.InvokeScriptAsync("insertImage", new string[] { string.Format(_insertImg, file.Substring(2, index - 2)) });
                }
            }
        }

        async void OnInsertVideo(object sender, Mi e)
        {
            var dlg = (ISelectFileDlg)Activator.CreateInstance(Type.GetType(FileItem.SelectFileDlgType));
            if (await dlg.Show(true, FileItem.VideoExt))
            {
                foreach (var file in dlg.SelectedFiles)
                {
                    int index = file.IndexOf("\",");
                    if (index > 2)
                    {
                        string id = file.Substring(2, index - 2);
                        string thumb = id + "-t.jpg";
                        await _wv.InvokeScriptAsync("insertVideo", new string[] { string.Format(_insertVideo, id, thumb) });
                    }
                }
            }
        }

        protected override async Task<bool> OnClosing(bool p_result)
        {
            if (!_saved)
            {
                var html = await _wv.InvokeScriptAsync("getHtml", null);
                if (_host.CurrentHtml == html)
                    return true;

                return await Kit.Confirm("关闭将丢失已修改的内容，确认要关闭？");
            }
            return true;
        }
    }
}