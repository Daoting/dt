#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
#endregion

namespace Dt.Base.FormView
{
    public sealed partial class HtmlEditDlg : Dlg
    {
        const string _insertVideo = "<video src=\"../../fsm/{0}\" poster=\"../../fsm/{1}\" preload=\"none\" width=\"640\" height=\"360\" controls=\"controls\"></video>";
        const string _insertImg = "../../fsm/{0}";
        CHtml _owner;
        bool _saved;

        public HtmlEditDlg(CHtml p_owner)
        {
            InitializeComponent();

            _owner = p_owner;
            _wv.Source = new Uri($"{AtSys.Stub.ServerUrl.TrimEnd('/')}/pub/editor/default.html");
        }

        public async void ShowDlg()
        {
            Show();

            // WebView事件无法捕捉到初始化html的时机，延时
            await Task.Delay(500);
            var val = _owner.GetVal();
            await _wv.InvokeScriptAsync("setHtml", new string[] { (val != null) ? val.ToString() : "" });
        }

        async void OnSave(object sender, Mi e)
        {
            var html = await _wv.InvokeScriptAsync("getHtml", null);
            _owner.SetVal(html);
            _owner.OnSaved();
            Close(true);
            _saved = true;
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
                var obj = _owner.GetVal();
                string val = (obj == null) ? null : obj.ToString();
                var html = await _wv.InvokeScriptAsync("getHtml", null);
                if ((string.IsNullOrEmpty(val) && string.IsNullOrEmpty(html))
                    || val == html)
                    return true;

                return await AtKit.Confirm("关闭将丢失已修改的内容，确认要关闭？");
            }
            return true;
        }
    }
}