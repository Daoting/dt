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
    public sealed partial class HtmlViewDlg : Dlg
    {
        public HtmlViewDlg()
        {
            InitializeComponent();
            _wv.Source = new Uri($"{Kit.Stub.ServerUrl}/pub/editor/html/readonly.html");
        }

        public async void ShowDlg(CHtml p_owner)
        {
            Title = p_owner.Title;
            Show();

            // WebView事件无法捕捉到初始化html的时机，延时
            await Task.Delay(500);
            var val = p_owner.GetVal();
            await _wv.InvokeScriptAsync("setHtml", new string[] { (val != null) ? val.ToString() : "" });
        }
    }
}