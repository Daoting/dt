#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
#endregion

namespace Dt.App.Pub
{
    public sealed partial class EditPostDlg : Dlg
    {
        const string _insertVideo = "<video src=\"../../fsm/{0}\" poster=\"../../fsm/{1}\" preload=\"none\" width=\"640\" height=\"360\" controls=\"controls\"></video>";
        const string _insertImg = "../../fsm/{0}";
        static Repo<Post> _repo = new Repo<Post>();
        Post _post;
        bool _saved;

        public EditPostDlg()
        {
            InitializeComponent();
            _wv.Source = new Uri($"{AtSys.Stub.ServerUrl.TrimEnd('/')}/pub/editor/default.html");
        }

        public async void ShowDlg(long p_id)
        {
            if (!AtSys.IsPhoneUI)
            {
                ShowWinVeil = true;
                Height = SysVisual.ViewHeight - 140;
                Width = Math.Min(900, SysVisual.ViewWidth - 200);
            }
            Show();

            _post = await _repo.Get("文章-编辑内容", new { id = p_id });

            // WebView事件无法捕捉到初始化html的时机，延时
            await Task.Delay(500);
            await _wv.InvokeScriptAsync("setHtml", new string[] { _post.Content });
        }

        async void OnSave(object sender, Mi e)
        {
            _post["Content"] = await _wv.InvokeScriptAsync("getHtml", null);
            if (_post.IsChanged)
            {
                var ret = await AtPublish.SavePost(_post);
                if (!string.IsNullOrEmpty(ret))
                {
                    AtKit.Warn("保存失败：" + ret);
                    return;
                }

                _post.AcceptChanges();
                AtKit.Msg("保存成功");
            }
            _saved = true;
            Close(true);
        }

        async void OnInsertImg(object sender, Mi e)
        {
            await _wv.InvokeScriptAsync("insertImage", new string[] { string.Format(_insertImg, "photo/1.jpg") });
        }

        async void OnInsertVideo(object sender, Mi e)
        {
            await _wv.InvokeScriptAsync("insertVideo", new string[] { string.Format(_insertVideo, "photo/mov.mp4", "photo/mov-t.jpg") });
        }

        protected override async Task<bool> OnClosing()
        {
            if (!_saved)
            {
                var html = await _wv.InvokeScriptAsync("getHtml", null);
                if (html != _post.Content)
                    return await AtKit.Confirm("关闭将丢失已修改的内容，确认要关闭？");
            }
            return true;
        }
    }
}
