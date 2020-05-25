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
        CHtml _owner;
        bool _saved;

        public HtmlEditDlg()
        {
            InitializeComponent();
            _wv.Source = new Uri($"{AtSys.Stub.ServerUrl.TrimEnd('/')}/pub/editor/default.html");
        }

        public async void ShowDlg(CHtml p_owner)
        {
            _owner = p_owner;
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